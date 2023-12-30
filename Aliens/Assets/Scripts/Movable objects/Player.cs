using UnityEngine;

[RequireComponent(typeof(MovableObject))]
public class Player : MonoBehaviour
{
    private const string _endLevel = "EndLevel";
    private const string _wakingUpObject = "WakingUpObject";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(_endLevel))
        {
            Debug.Log("Level is end");
            EndLevel();
            return;
        }

        if (collision.CompareTag(_wakingUpObject))
        {
            Debug.Log("Wake up!");
            WakeUp();
            return;
        }
    }

    private void EndLevel()
    {

    }

    private void WakeUp()
    {

    }
}
