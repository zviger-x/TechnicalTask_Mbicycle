using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    public Transform Target;
    public Transform Center;

    [SerializeField, Range(0f, 1f)] private float _offsetToTarget = .25f;
    [SerializeField] private float _lerpCoefficient = 5;

    private void Update()
    {
        if (Target == null || Center == null)
            return;

        var selfPos = transform.position;
        var targetPos = Target.position;
        var centerPos = Center.position;

        var tPosX = Mathf.Lerp(centerPos.x, targetPos.x, _offsetToTarget);
        var tPosY = Mathf.Lerp(centerPos.y, targetPos.y, _offsetToTarget);

        var t = _lerpCoefficient * Time.deltaTime;
        var x = Mathf.Lerp(selfPos.x, tPosX, t);
        var y = Mathf.Lerp(selfPos.y, tPosY, t);

        transform.position = new Vector3(x, y, selfPos.z);
    }
}
