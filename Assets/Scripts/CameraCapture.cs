using UnityEngine;
using System.IO;

public class CameraCapture : MonoBehaviour
{
    public Transform target; // Takip edilecek obje
    public float triggerRadius = 1.3f; // (0,0,0) noktasına ne kadar yaklaşıldığında tetiklenir
    public float cooldown = 2f; // Aynı noktadan geçerken tekrar tekrar tetiklenmesini önlemek için süre
    private float lastCaptureTime = -Mathf.Infinity;
    private bool hasCaptured = false;

    void Update()
    {
        if (target == null) return;

        float distanceToOrigin = Vector3.Distance(target.position, Vector3.zero);

        if (distanceToOrigin <= triggerRadius && !hasCaptured && Time.time - lastCaptureTime > cooldown)
        {
            StartCoroutine(CaptureScreenshot());
            lastCaptureTime = Time.time;
            hasCaptured = true;
        }

        // Obje uzaklaştıysa tekrar tetiklemeye izin ver
        if (distanceToOrigin > triggerRadius)
        {
            hasCaptured = false;
        }
    }

    private System.Collections.IEnumerator CaptureScreenshot()
    {
        // Ekran görüntüsü alımı için bir frame bekle
        yield return new WaitForEndOfFrame();

        string directory = Path.Combine(Application.dataPath, "Screenshots");
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        string filename = "capture_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string filepath = Path.Combine(directory, filename);

        ScreenCapture.CaptureScreenshot(filepath);
        Debug.Log("Ekran görüntüsü alındı: " + filepath);
    }
}

