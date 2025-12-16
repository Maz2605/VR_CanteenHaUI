using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables; // Unity 6 namespace
using System.Collections;

[RequireComponent(typeof(XRSimpleInteractable))]
public class ConfigurableChairController : MonoBehaviour
{
    // 1. ĐỊNH NGHĨA CÁC HƯỚNG
    public enum MoveDirection
    {
        Forward,    // Hướng mặt ghế (Trục Z+)
        Backward,   // Hướng lưng ghế (Trục Z-)
        Left,       // Trái (Trục X-)
        Right,      // Phải (Trục X+)
        Up,         // Lên (Trục Y+)
        Down        // Xuống (Trục Y-)
    }

    [Header("Cấu hình Di chuyển")]
    [SerializeField] private MoveDirection direction = MoveDirection.Backward; // Mặc định lùi
    [SerializeField] private float distance = 0.6f; // Khoảng cách (mét)
    [SerializeField] private float duration = 0.5f; // Thời gian trượt
    [SerializeField] private AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // Trạng thái nội bộ
    private bool _isOpen = false;
    private bool _isMoving = false;
    private Vector3 _closedPos;
    private Vector3 _openPos;
    private XRSimpleInteractable _interactable;

    private void Awake()
    {
        _interactable = GetComponent<XRSimpleInteractable>();
        _closedPos = transform.position;
        
        // Tính toán điểm đích dựa trên hướng và khoảng cách đã chọn
        Vector3 directionVector = GetDirectionVector();
        _openPos = _closedPos + (directionVector * distance);
    }

    private void OnEnable()
    {
        if (_interactable) _interactable.selectEntered.AddListener(OnInteract);
    }

    private void OnDisable()
    {
        if (_interactable) _interactable.selectEntered.RemoveListener(OnInteract);
    }

    private void OnInteract(SelectEnterEventArgs args)
    {
        if (_isMoving) return; // Chặn spam
        ToggleChair();
    }

    [ContextMenu("Test Toggle")]
    public void ToggleChair()
    {
        if (_isMoving) return;

        _isOpen = !_isOpen;
        Vector3 target = _isOpen ? _openPos : _closedPos;
        StartCoroutine(MoveRoutine(target));
    }

    private IEnumerator MoveRoutine(Vector3 targetPos)
    {
        _isMoving = true;
        Vector3 startPos = transform.position;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            transform.position = Vector3.Lerp(startPos, targetPos, moveCurve.Evaluate(t));
            yield return null;
        }

        transform.position = targetPos;
        _isMoving = false;
    }

    // Hàm tiện ích để lấy vector hướng dựa trên Enum
    private Vector3 GetDirectionVector()
    {
        switch (direction)
        {
            case MoveDirection.Forward: return transform.forward;
            case MoveDirection.Backward: return -transform.forward;
            case MoveDirection.Left: return -transform.right;
            case MoveDirection.Right: return transform.right;
            case MoveDirection.Up: return transform.up;
            case MoveDirection.Down: return -transform.up;
            default: return -transform.forward;
        }
    }

    // --- TÍNH NĂNG DEBUG TRỰC QUAN (GIZMOS) ---
    // Vẽ đường đi của ghế ngay trong màn hình Scene để dễ căn chỉnh
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) // Chỉ vẽ khi chưa chạy game để preview
        {
            Gizmos.color = Color.green;
            Vector3 endPoint = transform.position + (GetDirectionVector() * distance);
            
            // Vẽ đường thẳng từ ghế đến điểm đích
            Gizmos.DrawLine(transform.position, endPoint);
            // Vẽ khối cầu nhỏ tại điểm đích
            Gizmos.DrawWireSphere(endPoint, 0.05f);
        }
    }
}