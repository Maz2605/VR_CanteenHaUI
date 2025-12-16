using System.Collections;
using UnityEngine;

public class SimpleSlideDoor : MonoBehaviour
{
    [System.Serializable]
    public class SlideConfig
    {
        public string name = "Door Wing";
        public Transform movingPart; // Object sẽ di chuyển

        [Tooltip("Hướng trượt (Local Space). Ví dụ: (1,0,0) là sang phải.")]
        public Vector3 slideDirection = Vector3.right;

        [Tooltip("Khoảng cách trượt (Mét)")] public float slideDistance = 1.0f;

        [HideInInspector] public Vector3 closedPos;
        [HideInInspector] public Vector3 openPos;
    }

    [Header("General Settings")] [SerializeField]
    private float duration = 1.0f;

    [SerializeField] private AnimationCurve motionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Doors Configuration")] [SerializeField]
    private SlideConfig doorLeft;

    [SerializeField] private SlideConfig doorRight;

    // [Header("Audio")] [SerializeField] private AudioSource audioSource;
    // [SerializeField] private AudioClip slideSFX;

    private bool _isOpen = false;
    private Coroutine _moveCoroutine;

    void Start()
    {
        InitDoor(doorLeft);
        InitDoor(doorRight);
    }

    private void InitDoor(SlideConfig door)
    {
        if (door.movingPart == null) return;

        // Lưu vị trí đóng (Local Position)
        door.closedPos = door.movingPart.localPosition;

        // Tính toán vị trí mở = Vị trí đóng + (Hướng * Khoảng cách)
        door.openPos = door.closedPos + (door.slideDirection.normalized * door.slideDistance);
    }

    public void ToggleDoors()
    {
        if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);

        _isOpen = !_isOpen;

        // if (audioSource && slideSFX) audioSource.PlayOneShot(slideSFX);

        _moveCoroutine = StartCoroutine(AnimateSliding(_isOpen));
    }

    private IEnumerator AnimateSliding(bool opening)
    {
        float time = 0;

        Vector3 startLeft = doorLeft.movingPart ? doorLeft.movingPart.localPosition : Vector3.zero;
        Vector3 startRight = doorRight.movingPart ? doorRight.movingPart.localPosition : Vector3.zero;

        Vector3 targetLeft = opening ? doorLeft.openPos : doorLeft.closedPos;
        Vector3 targetRight = opening ? doorRight.openPos : doorRight.closedPos;

        while (time < 1)
        {
            time += Time.deltaTime / duration;
            float smooth = motionCurve.Evaluate(time);

            if (doorLeft.movingPart)
                doorLeft.movingPart.localPosition = Vector3.Lerp(startLeft, targetLeft, smooth);

            if (doorRight.movingPart)
                doorRight.movingPart.localPosition = Vector3.Lerp(startRight, targetRight, smooth);

            yield return null;
        }

        if (doorLeft.movingPart) doorLeft.movingPart.localPosition = targetLeft;
        if (doorRight.movingPart) doorRight.movingPart.localPosition = targetRight;
    }

    // --- GIZMOS: Vẽ đường trượt để dễ setup ---
    private void OnDrawGizmos()
    {
        DrawSlideGizmo(doorLeft);
        DrawSlideGizmo(doorRight);
    }

    private void DrawSlideGizmo(SlideConfig door)
    {
        if (door.movingPart == null) return;

        // Vị trí hiện tại
        Vector3 start = door.movingPart.position;

        // Vị trí đích dự kiến (Global Space để vẽ Gizmo)
        // Lưu ý: Đây chỉ là mô phỏng, thực tế chạy theo Local
        Vector3 end = start + (door.movingPart.TransformDirection(door.slideDirection.normalized) * door.slideDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(start, Vector3.one * 0.1f); // Điểm bắt đầu

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(end, Vector3.one * 0.1f); // Điểm kết thúc

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(start, end); // Đường đi
    }
}