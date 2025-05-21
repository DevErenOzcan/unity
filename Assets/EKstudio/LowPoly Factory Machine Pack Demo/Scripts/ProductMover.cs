using UnityEngine;

public class ProductMover : MonoBehaviour
{
    public Vector3 startPosition = new Vector3(0, 1.1f, -6f);
    public Vector3 endPosition = new Vector3(0, 1.1f, 6f);
    public float speed = 2f;

    private void Start()
    {
        // Objeyi başlangıç noktasına yerleştir
        transform.position = startPosition;
    }

    private void Update()
    {
        // Objeyi ileri taşı
        transform.position = Vector3.MoveTowards(transform.position, endPosition, speed * Time.deltaTime);

        // Bitiş noktasına ulaştıysa sıfırla
        if (Vector3.Distance(transform.position, endPosition) < 0.01f)
        {
            // Burada objeyi yok edip yeniden başlatmak istiyorsan:
            RestartMovement();
        }
    }

    void RestartMovement()
    {
        // Objeyi başa döndür
        transform.position = startPosition;
    }
}

