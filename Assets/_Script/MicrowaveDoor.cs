using System.Collections;
using UnityEngine;

public class MicrowaveDoor : MonoBehaviour
{
    
    [Header("Rotation Settings")]
    [SerializeField] private float openAngle = 90f; // Góc mở (thường là 90 độ xuống dưới)
    [SerializeField] private float duration = 0.6f; // Tốc độ mở
    [SerializeField] private AnimationCurve motionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // [Header("Audio")]
    // [SerializeField] private AudioSource audioSource;
    // [SerializeField] private AudioClip doorClickSFX;

    private bool _isOpen = false;
    private Coroutine _currentCoroutine;
    private Quaternion _closedRot;
    private Quaternion _openRot;

    void Start()
    {
        // Lưu trạng thái đóng ban đầu
        _closedRot = transform.localRotation;
        // Tính toán trạng thái mở: Xoay quanh trục X (Vector3.right)
        _openRot = _closedRot * Quaternion.Euler(openAngle, 0, 0);
    }

    // Hàm này gắn vào sự kiện Select Entered của XR Simple Interactable
    public void ToggleDoor()
    {
        if (_currentCoroutine != null) StopCoroutine(_currentCoroutine);
        
        _isOpen = !_isOpen;
        
        // Chọn góc đích
        Quaternion targetRot = _isOpen ? _openRot : _closedRot;
        
        // Phát âm thanh
        // if (audioSource && doorClickSFX) audioSource.PlayOneShot(doorClickSFX);

        _currentCoroutine = StartCoroutine(AnimateDoor(targetRot));
    }

    private IEnumerator AnimateDoor(Quaternion target)
    {
        Quaternion start = transform.localRotation;
        float time = 0;

        while (time < 1)
        {
            time += Time.deltaTime / duration;
            transform.localRotation = Quaternion.Lerp(start, target, motionCurve.Evaluate(time));
            yield return null;
        }
        transform.localRotation = target;
    }

}
