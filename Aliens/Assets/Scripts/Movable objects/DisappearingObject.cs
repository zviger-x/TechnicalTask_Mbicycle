using UnityEngine;

[RequireComponent(typeof(MovableObject))]
public class DisappearingObject : MonoBehaviour
{
    private const string _destroyingLight = "DestroyingLight";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(_destroyingLight))
        {
            Debug.Log("Destroying objects...");
            Destroy(collision.gameObject);
            Destroy(gameObject);
            return;
        }
    }
}
