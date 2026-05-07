using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class TargetIndicator : MonoBehaviour
{
    [SerializeField] private Image _targetBox;
    [SerializeField] private Image _lockOnImage;
    [SerializeField] private Image _offScreenImage;

    [SerializeField] private float _offScreenMargin = 45f;

    private Transform _target;
    private Camera _mainCamera;

    private RectTransform _rectTransform;
    private RectTransform _canvasRect;

    private Vector3 _screenCenter = Vector3.zero;

    public int Key { get; private set; }
    public bool LockedOn { get; set; }

    private Vector3 ScreenCenter
    {
        get
        {
            if (_canvasRect == null)
                return Vector3.zero;

            Rect rect = _canvasRect.rect;

            _screenCenter.x = rect.width * 0.5f;
            _screenCenter.y = rect.height * 0.5f;
            _screenCenter.z = 0f;

            return _screenCenter * _canvasRect.localScale.x;
        }
    }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        // Target destroyed or missing
        if (_target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Camera may not exist yet
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;

            if (_mainCamera == null)
                return;
        }

        // Canvas missing
        if (_canvasRect == null)
            return;

        Vector3 targetViewportPos =
            _mainCamera.WorldToViewportPoint(_target.position);

        if (TargetIsVisible(targetViewportPos))
        {
            DisplayOnScreenReticle(targetViewportPos);
        }
        else
        {
            DisplayOffScreenReticle(targetViewportPos);
        }
    }

    public void Init(Transform target, Canvas mainCanvas)
    {
        if (target == null || mainCanvas == null)
        {
            Debug.LogWarning("TargetIndicator Init failed due to null references.");
            enabled = false;
            return;
        }

        _target = target;
        Key = target.GetInstanceID();

        _canvasRect = mainCanvas.GetComponent<RectTransform>();

        _mainCamera = Camera.main;
    }

    private bool TargetIsVisible(Vector3 position)
    {
        return position.x is >= 0 and <= 1
            && position.y is >= 0 and <= 1
            && position.z >= 0;
    }

    private void DisplayOnScreenReticle(Vector3 targetViewportPos)
    {
        Vector3 position =
            _mainCamera.ViewportToScreenPoint(targetViewportPos);

        position.z = 0f;

        _rectTransform.position = position;

        if (_targetBox != null)
            _targetBox.enabled = !LockedOn;

        if (_lockOnImage != null)
            _lockOnImage.enabled = LockedOn;

        if (_offScreenImage != null)
            _offScreenImage.enabled = false;
    }

    private void DisplayOffScreenReticle(Vector3 targetViewportPos)
    {
        if (_targetBox != null)
            _targetBox.enabled = false;

        if (_lockOnImage != null)
            _lockOnImage.enabled = false;

        Vector3 indicatorPosition =
            (_mainCamera.ViewportToScreenPoint(targetViewportPos) - ScreenCenter)
            * Mathf.Sign(targetViewportPos.z);

        indicatorPosition.z = 0f;

        // Prevent divide by zero
        if (Mathf.Approximately(indicatorPosition.x, 0f))
            indicatorPosition.x = 0.01f;

        if (Mathf.Approximately(indicatorPosition.y, 0f))
            indicatorPosition.y = 0.01f;

        float x =
            (ScreenCenter.x - _offScreenMargin)
            / Mathf.Abs(indicatorPosition.x);

        float y =
            (ScreenCenter.y - _offScreenMargin)
            / Mathf.Abs(indicatorPosition.y);

        if (x < y)
        {
            float angle = Vector3.SignedAngle(
                Vector3.right,
                indicatorPosition,
                Vector3.forward
            );

            indicatorPosition.x =
                Mathf.Sign(indicatorPosition.x)
                * (_screenCenter.x - _offScreenMargin)
                * _canvasRect.localScale.x;

            indicatorPosition.y =
                Mathf.Tan(Mathf.Deg2Rad * angle)
                * indicatorPosition.x;
        }
        else
        {
            float angle = Vector3.SignedAngle(
                Vector3.up,
                indicatorPosition,
                Vector3.forward
            );

            indicatorPosition.y =
                Mathf.Sign(indicatorPosition.y)
                * (_screenCenter.y - _offScreenMargin)
                * _canvasRect.localScale.y;

            indicatorPosition.x =
                -Mathf.Tan(Mathf.Deg2Rad * angle)
                * indicatorPosition.y;
        }

        indicatorPosition += ScreenCenter;

        _rectTransform.position = indicatorPosition;

        Vector3 rotation = _rectTransform.eulerAngles;

        rotation.z = Vector3.SignedAngle(
            Vector3.up,
            indicatorPosition - ScreenCenter,
            Vector3.forward
        );

        _rectTransform.eulerAngles = rotation;

        if (_offScreenImage != null)
            _offScreenImage.enabled = true;
    }
}