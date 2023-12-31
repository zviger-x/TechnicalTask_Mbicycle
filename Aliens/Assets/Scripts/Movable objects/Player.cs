using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MovableObject))]
public class Player : MonoBehaviour
{
    public UnityEvent OnTriggerEndLevel;
    public UnityEvent OnTriggerWakingUpObject;

    private const string _endLevel = "EndLevel";
    private const string _wakingUpObject = "WakingUpObject";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(_endLevel))
        {
            Debug.Log("Level end");
            OnTriggerEndLevel?.Invoke();
            return;
        }

        if (collision.CompareTag(_wakingUpObject))
        {
            Debug.Log("Wake up");
            OnTriggerWakingUpObject?.Invoke();
            return;
        }
    }
}
