using UnityEngine;
using System.Collections;
// Nếu dòng dưới bị đỏ, hãy xóa nó đi và dùng cách gọi event thủ công
using UnityEngine.XR.Interaction.Toolkit; 

public class NotebookController : MonoBehaviour
{
    [Header("Kéo Gáy Sách vào đây (Không kéo bìa)")]
    public Transform pivotPoint; 

    [Header("Cấu hình")]
    public float openAngle = -160f; // Góc mở
    public float speed = 2.0f;      // Tốc độ

    private bool _isOpen = false;
    private Quaternion _closedRot;
    private Quaternion _openRot;
    private Coroutine _routine;

    void Start()
    {
        // Tự động sửa lỗi nếu quên gán Pivot
        if (pivotPoint == null)
        {
            Debug.LogError("LỖI: Bạn chưa kéo object Gáy Sách (Pivot) vào script!");
            // Tạm thời lấy object hiện tại để không crash game
            pivotPoint = transform; 
        }

        _closedRot = pivotPoint.localRotation;
        // Mặc định xoay quanh trục Z (trục màu xanh dương)
        _openRot = _closedRot * Quaternion.Euler(0, 0, openAngle);
    }

    // Gắn hàm này vào sự kiện Select Entered
    public void ToggleNotebook()
    {
        if (_routine != null) StopCoroutine(_routine);
        _isOpen = !_isOpen;
        _routine = StartCoroutine(DoAnimation());
    }

    IEnumerator DoAnimation()
    {
        Quaternion target = _isOpen ? _openRot : _closedRot;
        while (Quaternion.Angle(pivotPoint.localRotation, target) > 0.1f)
        {
            // Dùng RotateTowards thay vì Lerp để tránh lỗi toán học
            pivotPoint.localRotation = Quaternion.RotateTowards(
                pivotPoint.localRotation, 
                target, 
                speed * 100 * Time.deltaTime
            );
            yield return null;
        }
        pivotPoint.localRotation = target;
    }
    
    private void OnDrawGizmos()
    {
        if (pivotPoint == null) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(pivotPoint.position, pivotPoint.forward * 0.2f);
    }
}