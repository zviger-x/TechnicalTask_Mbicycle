using UnityEngine;

[RequireComponent(typeof(MovableObject))]
public class DisappearingObject : MonoBehaviour
{
    private const string _destroyingLight = "DestroyingLight";

    [SerializeField] private GameObject _particlesPrefab;

    private void Start()
    {
        if (_particlesPrefab == null)
            return;

        Instantiate(_particlesPrefab, transform);
    }

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
