using UnityEngine;

public class ProductMover : MonoBehaviour
{
    public Vector3 startPosition = new Vector3(0, 1.1f, -6f);
    public Vector3 endPosition = new Vector3(0, 1.1f, 6f);
    public float speed = 6f;

    [Header("Randomization Settings")]
    public float maxPositionOffset = 0.1f; // Pozisyon için maksimum rastgele sapma
    public float maxRotationOffset = 5f;   // Rotasyon için maksimum rastgele açı (Euler açıları)
    public float maxScaleOffset = 0.1f;    // Ölçek için maksimum rastgele sapma (orijinal ölçeğe göre)

    private Vector3 originalScale; // Objelerin orijinal ölçeğini tutmak için

    private void Start()
    {
        // Objeyi başlangıç noktasına yerleştir
        transform.position = startPosition;

        // Orijinal ölçeği kaydet
        originalScale = transform.localScale;

        // Objeye ilk rastgele değişiklikleri uygula
        ApplyRandomVariations();
    }

    private void Update()
    {
        // Objeyi ileri taşı
        transform.position = Vector3.MoveTowards(transform.position, endPosition, speed * Time.deltaTime);

        // Bitiş noktasına ulaştıysa sıfırla ve yeni rastgele değişiklikler uygula
        if (Vector3.Distance(transform.position, endPosition) < 0.01f)
        {
            RestartMovement();
        }
    }

    void RestartMovement()
    {
        // Objeyi başa döndür
        transform.position = startPosition;

        // Objeye yeni rastgele değişiklikleri uygula
        ApplyRandomVariations();
    }

    void ApplyRandomVariations()
    {
        // Rastgele pozisyon sapması
        float randomXPos = Random.Range(-maxPositionOffset, maxPositionOffset);
        float randomYPos = Random.Range(-maxPositionOffset, maxPositionOffset);
        float randomZPos = Random.Range(-maxPositionOffset, maxPositionOffset);
        transform.position += new Vector3(randomXPos, randomYPos, randomZPos);

        // Rastgele rotasyon (Euler açıları ile)
        float randomXRot = Random.Range(-maxRotationOffset, maxRotationOffset);
        float randomYRot = Random.Range(-maxRotationOffset, maxRotationOffset);
        float randomZRot = Random.Range(-maxRotationOffset, maxRotationOffset);
        transform.rotation *= Quaternion.Euler(randomXRot, randomYRot, randomZRot); // Mevcut rotasyona ekle

        // Rastgele ölçek
        float randomScaleFactor = Random.Range(-maxScaleOffset, maxScaleOffset);
        transform.localScale = originalScale * (1f + randomScaleFactor); // Orijinal ölçeğe göre ayarla
    }
}
