using UnityEngine;

public class LargeContainer : MonoBehaviour
{
    public WastePourController wasteController;  // Referensi ke WastePourController
    public int totalWasteInContainer = 0; // Menyimpan jumlah sampah yang masuk

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Waste")) // Cek apakah objek yang masuk adalah sampah
        {
            totalWasteInContainer++; // Tambah jumlah sampah yang masuk
            Debug.Log("Sampah masuk! Total: " + totalWasteInContainer);

            // Panggil method OnWasteCollected dari WastePourController
            wasteController.OnWasteCollected(other.gameObject);
        }
    }

}
