using UnityEngine;

public class ProductMover : MonoBehaviour
{
    public Vector3 startPosition = new Vector3(0, 1.1f, -6f);
    public Vector3 endPosition = new Vector3(0, 1.1f, 6f);
    public float speed = 6f;

    [Header("Randomization Settings")]
    public float maxRotationOffset = 5f;   // Yalnızca Y ekseninde rastgele açı

    private void Start()
    {
        // Objeyi başlangıç noktasına yerleştir
        transform.position = startPosition;

        // Başlangıçta rastgele rotasyon uygula
        ApplyRandomRotation();
    }

    private void Update()
    {
        // Objeyi ileri taşı
        transform.position = Vector3.MoveTowards(transform.position, endPosition, speed * Time.deltaTime);

        // Bitiş noktasına ulaştıysa sıfırla ve yeni rastgele rotasyon uygula
        if (Vector3.Distance(transform.position, endPosition) < 0.01f)
        {
            RestartMovement();
        }
    }

    void RestartMovement()
    {
        // Objeyi başa döndür
        transform.position = startPosition;

        // Yalnızca rotasyonu rastgele değiştir
        ApplyRandomRotation();
    }

    void ApplyRandomRotation()
    {
        // Sadece Y ekseninde rastgele rotasyon uygula
        float randomYRot = Random.Range(-maxRotationOffset, maxRotationOffset);
        transform.rotation = Quaternion.Euler(0f, randomYRot, 0f);
    }
}
