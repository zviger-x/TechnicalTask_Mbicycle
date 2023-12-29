using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class MovableObject : MonoBehaviour
{
    [SerializeField] private bool _enabled = true;
    [SerializeField] private float _threshold = 25f;
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _collidersOffset = .01f;

    private Vector2 _mouseInitPosition;
    private Vector2 _mouseDragPosition;
    private Vector2 _moveDirection;
    private bool _isMoving;
    private bool _canCurrentBeMoved = true;

    private Collider2D _collider;

    private static bool _canAnyBeMoved = true;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        GetComponent<Rigidbody2D>().isKinematic = true;
    }

    private void OnMouseDown()
    {
        if (!_enabled)
            return;

        _mouseInitPosition = Input.mousePosition;
    }

    private void OnMouseDrag()
    {
        if (!_enabled || !_canAnyBeMoved || !_canCurrentBeMoved)
            return;

        _mouseDragPosition = Input.mousePosition;

        var offset = _mouseDragPosition - _mouseInitPosition;
        if (!_isMoving && offset.magnitude > _threshold)
        {
            _moveDirection = ConvertVectorToAxisDirection(offset);
            var newPos = GetNewPositionFromMoveDirection(transform.position, _moveDirection, out var hit);
            if (newPos != null)
            {
                _canCurrentBeMoved = _canAnyBeMoved = false;
                _isMoving = true;
                StartCoroutine(MoveAnimationCoroutine((Vector2)newPos, hit));
            }
        }
    }

    private void OnMouseUp()
    {
        if (!_enabled)
            return;

        _canCurrentBeMoved = true;
    }

    private void OnHitObject(RaycastHit2D hit)
    {
        Debug.Log(hit.transform.gameObject.name);
    }

    private IEnumerator MoveAnimationCoroutine(Vector2 newPos, RaycastHit2D hit)
    {
        var time = 0f;

        while (time < 1f)
        {
            time += _speed * 2.5f * Time.fixedDeltaTime;

            var lerpValue = EasingSmoothSquared(time);
            transform.position = LerpVector(transform.position, newPos, lerpValue);

            yield return new WaitForFixedUpdate();
        }

        _isMoving = false;
        _canAnyBeMoved = true;

        OnHitObject(hit);
    }

    private float EasingSmoothSquared(float x)
    {
        return x < .5f ? x * x * 2f : (1f - (1f - x) * (1f - x) * 2f);
    }

    private Vector2 LerpVector(Vector2 a, Vector2 b,  float t)
    {
        var xLerp = Mathf.Lerp(a.x, b.x, t);
        var yLerp = Mathf.Lerp(a.y, b.y, t);
        return new Vector2(xLerp, yLerp);
    }

    private Vector2? GetNewPositionFromMoveDirection(Vector2 startPosition, Vector2 moveDirection, out RaycastHit2D hit)
    {
        var size = _collider.bounds.size;
        var sizeX = new Vector2(size.x - _collidersOffset * 2f, size.y *.1f);
        var sizeY = new Vector2(size.x * .1f, size.y - _collidersOffset * 2f);
        var offset = _collidersOffset;
        var selfPos = startPosition;

        if (moveDirection == Vector2.up)
        {
            hit = Physics2D.BoxCast(selfPos, sizeX, 0f, Vector2.up);
            if (hit)
            {
                if (hit.collider.isTrigger)
                {
                    var intermediatePosition = new Vector2(selfPos.x, hit.point.y);
                    return GetNewPositionFromMoveDirection(intermediatePosition, moveDirection, out hit);
                }

                return new Vector2(selfPos.x, hit.point.y - size.y / 2f - offset);
            }
        }

        if (moveDirection == Vector2.down)
        {
            hit = Physics2D.BoxCast(selfPos, sizeX, 0f, Vector2.down);
            if (hit)
            {
                if (hit.collider.isTrigger)
                {
                    var intermediatePosition = new Vector2(selfPos.x, hit.point.y);
                    return GetNewPositionFromMoveDirection(intermediatePosition, moveDirection, out hit);
                }

                return new Vector2(selfPos.x, hit.point.y + size.y / 2f + offset);
            }
        }

        if (moveDirection == Vector2.right)
        {
            hit = Physics2D.BoxCast(selfPos, sizeY, 0f, Vector2.right);
            if (hit)
            {
                if (hit.collider.isTrigger)
                {
                    var intermediatePosition = new Vector2(hit.point.x, selfPos.y);
                    return GetNewPositionFromMoveDirection(intermediatePosition, moveDirection, out hit);
                }

                return new Vector2(hit.point.x - size.x / 2f - offset, selfPos.y);
            }
        }

        if (moveDirection == Vector2.left)
        {
            hit = Physics2D.BoxCast(selfPos, sizeY, 0f, Vector2.left);
            if (hit)
            {
                if (hit.collider.isTrigger)
                {
                    var intermediatePosition = new Vector2(hit.point.x, selfPos.y);
                    return GetNewPositionFromMoveDirection(intermediatePosition, moveDirection, out hit);
                }

                return new Vector2(hit.point.x + size.x / 2f + offset, selfPos.y);
            }
        }

        hit = new RaycastHit2D();
        return null;
    }

    private Vector2 ConvertVectorToAxisDirection(Vector2 vector)
    {
        var up = Vector2.Dot(Vector2.up, vector);
        var down = Vector2.Dot(Vector2.down, vector);
        var left = Vector2.Dot(Vector2.left, vector);
        var right = Vector2.Dot(Vector2.right, vector);

        // Checking up-right
        if (up >= 0 && right >= 0)
        {
            if (up > right)
                return Vector2.up;
            else
                return Vector2.right;
        }

        // Checking right-down
        if (right >= 0 && down >= 0)
        {
            if (right > down)
                return Vector2.right;
            else
                return Vector2.down;
        }

        // Checking left-down
        if (left >= 0 && down >= 0)
        {
            if (left > down)
                return Vector2.left;
            else
                return Vector2.down;
        }

        // Checking left-up
        if (left >= 0 && up >= 0)
        {
            if (left > up)
                return Vector2.left;
            else
                return Vector2.up;
        }

        return Vector2.zero;
    }
}