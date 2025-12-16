using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Để dùng List

public class AirConditionerController : MonoBehaviour
{
    [System.Serializable]
    public class FlapConfig
    {
        [Header("Cấu hình Cánh Gió")]
        public string name = "Main Flap";
        public Transform pivotTransform; // Kéo pivot cánh gió vào đây

        [Tooltip("Trục xoay: (1,0,0) là trục Đỏ (X). (0,1,0) là trục Xanh (Y).")]
        public Vector3 rotationAxis = Vector3.right; // Mặc định là trục X cho điều hòa

        [Tooltip("Góc mở: Dùng số Âm (-) nếu nó mở ngược hướng bạn muốn.")]
        public float openAngle = 90f;

        // Lưu trạng thái nội bộ
        [HideInInspector] public Quaternion closedRot;
        [HideInInspector] public Quaternion openRot;
    }

    [Header("Motion Settings")]
    [SerializeField] private float duration = 1.0f; // Thời gian mở chậm rãi
    [SerializeField] private AnimationCurve motionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Flaps List")]
    [Tooltip("Bạn có thể thêm nhiều cánh gió vào đây (nút +)")]
    [SerializeField] private List<FlapConfig> flaps = new List<FlapConfig>();

    // [Header("Audio")]
    // [SerializeField] private AudioSource audioSource;
    // [SerializeField] private AudioClip beepSFX;

    private bool _isOpen = false;
    private Coroutine _moveCoroutine;

    void Start()
    {
        foreach (var flap in flaps)
        {
            InitializeFlap(flap);
        }
    }

    private void InitializeFlap(FlapConfig flap)
    {
        if (flap.pivotTransform == null) return;

        // Lưu trạng thái đóng là trạng thái hiện tại trong Editor
        flap.closedRot = flap.pivotTransform.localRotation;

        // Tính toán trạng thái mở dựa trên Trục (rotationAxis) và Góc (openAngle)
        // Công thức này đảm bảo xoay theo Local Space
        flap.openRot = flap.closedRot * Quaternion.AngleAxis(flap.openAngle, flap.rotationAxis);
    }

    // Gọi hàm này từ XR Simple Interactable
    public void ToggleAC()
    {
        if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);
        
        _isOpen = !_isOpen;
        
        // if (audioSource && beepSFX) audioSource.PlayOneShot(beepSFX);

        _moveCoroutine = StartCoroutine(AnimateFlaps(_isOpen));
    }

    private IEnumerator AnimateFlaps(bool opening)
    {
        float time = 0;
        
        // Tạo danh sách lưu vị trí start để animation mượt dù bị ngắt giữa chừng
        List<Quaternion> startRots = new List<Quaternion>();
        foreach (var flap in flaps)
        {
            if (flap.pivotTransform) 
                startRots.Add(flap.pivotTransform.localRotation);
            else 
                startRots.Add(Quaternion.identity);
        }

        while (time < 1)
        {
            time += Time.deltaTime / duration;
            float smooth = motionCurve.Evaluate(time);

            for (int i = 0; i < flaps.Count; i++)
            {
                var flap = flaps[i];
                if (flap.pivotTransform == null) continue;

                Quaternion target = opening ? flap.openRot : flap.closedRot;
                // Dùng Slerp cho chuyển động cong tự nhiên
                flap.pivotTransform.localRotation = Quaternion.Slerp(startRots[i], target, smooth);
            }
            
            yield return null;
        }

        // Đảm bảo về đích chính xác
        for (int i = 0; i < flaps.Count; i++)
        {
            var flap = flaps[i];
            if (flap.pivotTransform == null) continue;
            
            flap.pivotTransform.localRotation = opening ? flap.openRot : flap.closedRot;
        }
    }

    // --- GIZMOS: Giúp bạn nhìn thấy hướng mở trước khi Play ---
    private void OnDrawGizmos()
    {
        foreach (var flap in flaps)
        {
            DrawFlapGizmo(flap);
        }
    }

    private void DrawFlapGizmo(FlapConfig flap)
    {
        if (flap.pivotTransform == null) return;

        // Vẽ tâm
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(flap.pivotTransform.position, 0.01f);

        // Vẽ trục xoay (Màu Cyan)
        Gizmos.color = Color.cyan;
        Vector3 axisWorld = flap.pivotTransform.TransformDirection(flap.rotationAxis);
        Gizmos.DrawRay(flap.pivotTransform.position, axisWorld * 0.2f);

        // Vẽ hướng mở dự kiến (Màu Đỏ)
        Gizmos.color = Color.red;
        // Tính vector hướng sau khi xoay
        Vector3 openDir = Quaternion.AngleAxis(flap.openAngle, axisWorld) * flap.pivotTransform.forward; 
        // Nếu trục là X, ta thường quan tâm hướng Forward hoặc Up xoay đi đâu
        // Để dễ nhìn, ta vẽ một tia đại diện cho hướng "mặt" của cánh gió khi mở
        Gizmos.DrawRay(flap.pivotTransform.position, openDir * 0.3f);
    }
}