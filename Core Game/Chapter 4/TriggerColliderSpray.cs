using UnityEngine;

public class TriggerColliderSpray : MonoBehaviour
{
    private DraggableSpray sprayController;

    void Start()
    {
        sprayController = GetComponentInParent<DraggableSpray>();
    }
    //kalau trigger true/dicentang
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DestroyTarget"))
        {
            sprayController?.AddTarget(other.gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("DestroyTarget"))
        {
            sprayController?.AddTarget(other.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("DestroyTarget"))
        {
            sprayController?.AddTarget(collision.gameObject);
        }
    }
}
