using UnityEngine;

public class TriggerColliderSprayPlant : MonoBehaviour
{
    private DraggableSprayPlant sprayController;

    void Start()
    {
        sprayController = GetComponentInParent<DraggableSprayPlant>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SprayTarget"))
        {
            sprayController?.SetCurrentTarget(other.gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("SprayTarget"))
        {
            sprayController?.SetCurrentTarget(other.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("DestroyTarget"))
        {
            sprayController?.SetCurrentTarget(collision.gameObject);
        }
    }
}
