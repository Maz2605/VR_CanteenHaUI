using UnityEngine;

public class VR_SystemConfig : MonoBehaviour
{
    [Header("Cursor Settings")]
    [SerializeField] private bool hideCursorOnStart = true;

    private void Start()
    {
        if (hideCursorOnStart)
        {
            LockAndHideCursor();
        }
    }

    /// <summary>
    /// Xử lý khi ứng dụng mất/lại tiêu điểm (Alt-Tab hoặc tháo headset)
    /// </summary>
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && hideCursorOnStart)
        {
            LockAndHideCursor();
        }
    }

    private void LockAndHideCursor()
    {
        // Ẩn hình ảnh con trỏ
        Cursor.visible = false;
        
        // Khóa vị trí con trỏ vào tâm cửa sổ ứng dụng
        Cursor.lockState = CursorLockMode.Locked;
    }
}