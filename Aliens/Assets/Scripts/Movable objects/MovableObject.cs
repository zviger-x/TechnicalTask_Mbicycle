using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(AudioSource))]
public class MovableObject : MonoBehaviour
{
    [SerializeField] private bool _enabled = true;
    [SerializeField] private float _threshold = 25f;
    [SerializeField] private float _speed = 1f;
    [SerializeField] private AudioClip _swipeSound; 

    private Vector2 _mouseInitPosition;
    private Vector2 _mouseDragPosition;
    private Vector2 _moveDirection;
    private bool _isMoving;
    private bool _canCurrentBeMoved = true;

    private Collider2D _collider;
    private AudioSource _audioSource;

    public static bool IsAnyMoving { get; private set; }
    public static Action<MovableObject> OnObjectMoved;
    private static bool _canAnyBeMoved = true;
    private static bool _blocked = false;
    private static readonly float _collidersOffset = .05f;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _collider = GetComponent<Collider2D>();
        GetComponent<Rigidbody2D>().isKinematic = true;
    }

    private void OnDestroy()
    {
        CompleteMove();
    }

    private void OnMouseDown()
    {
        if (!_enabled)
            return;

        _mouseInitPosition = Input.mousePosition;
    }

    private void OnMouseDrag()
    {
        if (!_enabled || _blocked || !_canAnyBeMoved || !_canCurrentBeMoved)
            return;

        _mouseDragPosition = Input.mousePosition;

        var offset = _mouseDragPosition - _mouseInitPosition;
        if (!_isMoving && offset.magnitude > _threshold)
        {
            _moveDirection = ConvertVectorToAxisDirection(offset);

            var newPos = GetNewPositionFromMoveDirection(transform.position, _moveDirection, out var hit);
            if (newPos != null && CheckNewPosDistanceIsCorrect(hit.point, _moveDirection))
            {
                _canCurrentBeMoved = _canAnyBeMoved = false;
                IsAnyMoving = true;
                _isMoving = true;
                StartCoroutine(MoveAnimationCoroutine((Vector2)newPos));
                PlaySwipeSound();
            }
        }
    }

    private void OnMouseUp()
    {
        if (!_enabled)
            return;

        _canCurrentBeMoved = true;
    }

    private bool CheckNewPosDistanceIsCorrect(Vector2 hitPoint, Vector2 moveDirection)
    {
        var distance = (hitPoint - (Vector2)transform.position).magnitude;
        var th = .25f;

        if (moveDirection == Vector2.up || moveDirection == Vector2.down)
        {
            var max = _collider.bounds.size.y / 2f + th;
            return distance > max;
        }

        if (moveDirection == Vector2.left || moveDirection == Vector2.right)
        {
            var max = _collider.bounds.size.x / 2f + th;
            return distance > max;
        }

        return false;
    }

    private IEnumerator MoveAnimationCoroutine(Vector2 newPos)
    {
        var time = 0f;

        while (time < 1f)
        {
            if (_blocked)
                break;

            time += _speed * 2f * Time.fixedDeltaTime;

            var lerpValue = EasingSmoothSquared(time);
            transform.position = LerpVector(transform.position, newPos, lerpValue);

            yield return new WaitForFixedUpdate();
        }

        CompleteMove();
    }

    private void CompleteMove()
    {
        IsAnyMoving = false;
        _isMoving = false;
        _canAnyBeMoved = true;
        OnObjectMoved?.Invoke(this);
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
        var selfPos = startPosition;
        var triggerOffset = new Vector2(size.x * .05f, size.y * .05f);

        if (moveDirection == Vector2.up)
        {
            hit = Physics2D.BoxCast(selfPos, sizeX, 0f, Vector2.up);
            if (hit)
            {
                if (hit.collider.isTrigger)
                {
                    var intermediatePosition = new Vector2(selfPos.x, hit.point.y - triggerOffset.y);
                    return GetNewPositionFromMoveDirection(intermediatePosition, moveDirection, out hit);
                }

                return new Vector2(selfPos.x, hit.point.y - size.y / 2f);
            }
        }

        if (moveDirection == Vector2.down)
        {
            hit = Physics2D.BoxCast(selfPos, sizeX, 0f, Vector2.down);
            if (hit)
            {
                if (hit.collider.isTrigger)
                {
                    var intermediatePosition = new Vector2(selfPos.x, hit.point.y + triggerOffset.y);
                    return GetNewPositionFromMoveDirection(intermediatePosition, moveDirection, out hit);
                }

                return new Vector2(selfPos.x, hit.point.y + size.y / 2f);
            }
        }

        if (moveDirection == Vector2.right)
        {
            hit = Physics2D.BoxCast(selfPos, sizeY, 0f, Vector2.right);
            if (hit)
            {
                if (hit.collider.isTrigger)
                {
                    var intermediatePosition = new Vector2(hit.point.x - triggerOffset.x, selfPos.y);
                    return GetNewPositionFromMoveDirection(intermediatePosition, moveDirection, out hit);
                }

                return new Vector2(hit.point.x - size.x / 2f, selfPos.y);
            }
        }

        if (moveDirection == Vector2.left)
        {
            hit = Physics2D.BoxCast(selfPos, sizeY, 0f, Vector2.left);
            if (hit)
            {
                if (hit.collider.isTrigger)
                {
                    var intermediatePosition = new Vector2(hit.point.x + triggerOffset.x, selfPos.y);
                    return GetNewPositionFromMoveDirection(intermediatePosition, moveDirection, out hit);
                }

                return new Vector2(hit.point.x + size.x / 2f, selfPos.y);
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

    private void PlaySwipeSound()
    {
        if (_swipeSound == null)
            return;

        _audioSource.PlayOneShot(_swipeSound, AudioController.Instance.IsMutedSounds ? 0f : 1f);
    }

    public static void SetBlock(bool blocked)
    {
        _blocked = blocked;
    }
}
