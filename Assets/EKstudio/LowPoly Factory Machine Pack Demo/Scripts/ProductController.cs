using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[System.Serializable]
public class ServerResponse
{
    public string status;
    public string result;
    public string message;
}

public class ProductController : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector3 startPosition = new Vector3(0f, 1.2f, -9f);
    public Vector3 endPosition = new Vector3(0f, 1.2f, 9f);
    public float speed = 6f;

    [Header("Camera Capture Settings")]
    public float triggerRadius = 1.3f;
    public float cooldown = 2f;
    private float lastCaptureTime = -Mathf.Infinity;
    private bool hasCaptured = false;

    private bool isDefective = false;
    private bool movingToSide = false;
    private Vector3 sideTargetPosition;
    private bool shouldRestart = false;

    private void Start()
    {
        ResetToStart();
    }

    private void Update()
    {
        if (shouldRestart)
        {
            ResetToStart();
            shouldRestart = false;
            return;
        }

        if (!movingToSide)
        {
            // Düz ileri hareket (SADECE Z EKSENİNDE)
            Vector3 targetPos = new Vector3(startPosition.x, startPosition.y, endPosition.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

            // Trigger kontrolü
            float distanceToOrigin = Vector3.Distance(new Vector3(0, transform.position.y, transform.position.z), Vector3.zero);
            if (distanceToOrigin <= triggerRadius && !hasCaptured && Time.time - lastCaptureTime > cooldown)
            {
                StartCoroutine(CaptureAndSendScreenshot());
                lastCaptureTime = Time.time;
                hasCaptured = true;
            }

            // Bant sonuna gelindiğinde yan hareket başlat
            if (Mathf.Abs(transform.position.z - endPosition.z) < 0.1f && hasCaptured)
            {
                movingToSide = true;
                sideTargetPosition = new Vector3(isDefective ? -7.08f : 7.08f, transform.position.y, endPosition.z); // Z pozisyonunu sabit tut
            }
        }
        else
        {
            // Yan hareket (SADECE X EKSENİNDE)
            Vector3 targetPos = new Vector3(sideTargetPosition.x, transform.position.y, endPosition.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

            if (Mathf.Abs(transform.position.x - sideTargetPosition.x) < 0.1f)
            {
                movingToSide = false;
                shouldRestart = true;
            }
        }
    }

    void ResetToStart()
    {
        transform.position = startPosition;
        ApplyRandomRotation();
        ApplyRandomTexture();
        hasCaptured = false;
        isDefective = false;
        movingToSide = false;
    }

    void ApplyRandomRotation()
    {
        float randomYRot = Random.Range(-5f, 5f);
        transform.rotation = Quaternion.Euler(0f, randomYRot, 0f);
    }

    void ApplyRandomTexture()
    {
        Texture2D[] textures = Resources.LoadAll<Texture2D>("Textures");

        if (textures.Length == 0)
        {
            Debug.LogWarning("Hiçbir texture bulunamadı. Lütfen 'Resources/Textures' klasörünü kontrol edin.");
            return;
        }

        Texture2D randomTexture = textures[Random.Range(0, textures.Length)];

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null && renderer.material != null)
        {
            renderer.material.mainTexture = randomTexture;
            Debug.Log("Rastgele texture uygulandı: " + randomTexture.name);
        }
        else
        {
            Debug.LogWarning("Renderer ya da materyal bulunamadı.");
        }
    }

    private IEnumerator CaptureAndSendScreenshot()
    {
        yield return new WaitForEndOfFrame();

        Texture2D screenImage = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenImage.Apply();

        byte[] imageData = screenImage.EncodeToPNG();
        Destroy(screenImage);

        yield return StartCoroutine(SendImageToServer(imageData));
    }

    private IEnumerator SendImageToServer(byte[] imageData)
    {
        string url = "http://127.0.0.1:5000/upload_from_unity";

        WWWForm form = new WWWForm();
        form.AddBinaryData("image", imageData, "screenshot.png", "image/png");

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Görüntü başarıyla gönderildi.");
            string jsonResponse = www.downloadHandler.text;
            Debug.Log("Sunucu cevabı: " + jsonResponse);

            try
            {
                ServerResponse response = JsonUtility.FromJson<ServerResponse>(jsonResponse);
                if (response != null)
                {
                    isDefective = response.result.ToLower() == "defective";
                    Debug.Log("Ürün durumu: " + (isDefective ? "Hatalı" : "Sağlam"));
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Parse hatası: " + e.Message);
                isDefective = false;
            }
        }
        else
        {
            Debug.LogError("Gönderme hatası: " + www.error);
            isDefective = false;
        }
    }
}

