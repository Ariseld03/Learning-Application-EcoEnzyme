using UnityEngine;

public class TriggerColliderWatering : MonoBehaviour
{
    private DraggableWatering wateringController;

    void Start()
    {
        wateringController = GetComponentInParent<DraggableWatering>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SprayTarget") && wateringController != null)
        {
            wateringController.SetCurrentTarget(other.gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("SprayTarget") && wateringController != null)
        {
            wateringController.SetCurrentTarget(other.gameObject);
        }
    }
}
