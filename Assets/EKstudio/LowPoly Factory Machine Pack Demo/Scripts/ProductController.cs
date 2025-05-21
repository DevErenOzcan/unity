using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ProductController : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector3 startPosition = new Vector3(0, 1.2f, -6f);
    public Vector3 endPosition = new Vector3(0, 1.2f, 9f);
    public float speed = 6f;

    [Header("Randomization Settings")]
    public float maxRotationOffset = 5f; // Y ekseninde rastgele açı

    [Header("Camera Capture Settings")]
    public float triggerRadius = 1.3f;
    public float cooldown = 2f;
    private float lastCaptureTime = -Mathf.Infinity;
    private bool hasCaptured = false;

    private void Start()
    {
        // Objeyi başlangıç konumuna yerleştir
        transform.position = startPosition;

        // Rastgele rotasyon uygula
        ApplyRandomRotation();
    }

    private void Update()
    {
        // Objeyi hedef konuma doğru hareket ettir
        transform.position = Vector3.MoveTowards(transform.position, endPosition, speed * Time.deltaTime);

        // Kamera tetikleme kontrolü
        float distanceToOrigin = Vector3.Distance(transform.position, Vector3.zero);

        if (distanceToOrigin <= triggerRadius && !hasCaptured && Time.time - lastCaptureTime > cooldown)
        {
            StartCoroutine(CaptureAndSendScreenshot());
            lastCaptureTime = Time.time;
            hasCaptured = true;
        }

        if (distanceToOrigin > triggerRadius)
        {
            hasCaptured = false;
        }
    }

    void ApplyRandomRotation()
    {
        float randomYRot = Random.Range(-maxRotationOffset, maxRotationOffset);
        transform.rotation = Quaternion.Euler(0f, randomYRot, 0f);
    }

    private IEnumerator CaptureAndSendScreenshot()
    {
        yield return new WaitForEndOfFrame();

        // Ekran görüntüsü al
        Texture2D screenImage = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenImage.Apply();

        // PNG formatına çevir
        byte[] imageData = screenImage.EncodeToPNG();

        // Sunucuya gönder
        yield return StartCoroutine(SendImageToServer(imageData));

        // Belleği temizle
        Destroy(screenImage);
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
        }
        else
        {
            Debug.LogError("Gönderme hatası: " + www.error);
        }
    }
}

