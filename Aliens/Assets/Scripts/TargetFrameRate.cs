using UnityEngine;

public class TargetFrameRate : MonoBehaviour
{
    [SerializeField] private int _targetFrameRate = 60;

    void Start()
    {
        Application.targetFrameRate = _targetFrameRate;
    }
}
