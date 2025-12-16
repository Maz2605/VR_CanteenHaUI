using System.Collections;
using UnityEngine;

public class HumanNPC : MonoBehaviour
{
   [Header("References")]
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _pointA;
    [SerializeField] private Transform _pointB;

    [Header("Settings")]
    [SerializeField] private float _moveSpeed = 1.5f; 
    [SerializeField] private float _turnSpeed = 15f; // Tăng tốc độ xoay lên
    [SerializeField] private float _waitTime = 2.0f;
    
    [Header("Gravity Fix")]
    [Tooltip("Lớp layer của mặt đất (thường là Default hoặc Ground)")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundOffset = 0.0f; // Chỉnh nếu chân bị lún

    private Vector3 _currentTarget;
    private bool _isWaiting = false;

    private void Start()
    {
        if (_animator == null) _animator = GetComponent<Animator>();
        _animator.applyRootMotion = false;
        _animator.speed = 1f; 

        _currentTarget = _pointB.position;

        // --- FIX 1: RƠI TỰ DO XUỐNG ĐẤT NGAY KHI BẮT ĐẦU ---
        SnapToGround();
    }

    private void Update()
    {
        // Luôn giữ NPC dính mặt đất trong lúc di chuyển
        SnapToGround();

        if (_isWaiting) return;
        
        MoveAndRotate();
    }

    private void SnapToGround()
    {
        // Bắn 1 tia từ rốn nhân vật xuống dưới đất để tìm điểm tiếp xúc
        RaycastHit hit;
        // Bắn từ vị trí hiện tại + 1m lên cao, bắn thẳng xuống
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 10f, _groundLayer))
        {
            // Gán vị trí Y của nhân vật bằng vị trí đất + offset
            Vector3 groundPos = transform.position;
            groundPos.y = hit.point.y + _groundOffset;
            transform.position = groundPos;
        }
    }

    private void MoveAndRotate()
    {
        // Lấy vị trí đích (bỏ qua Y của đích, chỉ lấy X và Z)
        Vector3 targetFlat = new Vector3(_currentTarget.x, transform.position.y, _currentTarget.z);
        
        // Khoảng cách
        float distance = Vector3.Distance(transform.position, targetFlat);

        // --- FIX 2: KIỂM TRA ĐÍCH CHÍNH XÁC HƠN ---
        if (distance < 0.2f) // Tăng phạm vi nhận diện đích lên chút để dễ bắt
        {
            if (!_isWaiting) StartCoroutine(WaitRoutine());
            return;
        }

        // --- FIX 3: LOGIC XOAY NGƯỜI ---
        Vector3 direction = (targetFlat - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            // Xoay nhanh, mượt
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, _turnSpeed * Time.deltaTime);
        }

        // Di chuyển
        transform.position = Vector3.MoveTowards(transform.position, targetFlat, _moveSpeed * Time.deltaTime);
    }

    private IEnumerator WaitRoutine()
    {
        _isWaiting = true;

        // Đóng băng Animation
        _animator.speed = 0f; 

        yield return new WaitForSeconds(_waitTime);

        // Đảo chiều đích đến
        _currentTarget = (_currentTarget == _pointA.position) ? _pointB.position : _pointA.position;
        
        // --- FIX 4: QUAY ĐẦU NGAY LẬP TỨC ---
        // Để tránh đi lùi, ta bắt NPC nhìn về đích mới ngay khi hết giờ nghỉ
        Vector3 newDir = (_currentTarget - transform.position).normalized;
        newDir.y = 0;
        if (newDir != Vector3.zero) 
        {
            // Dòng này sẽ làm NPC quay ngoắt 180 độ ngay lập tức. 
            // Nếu muốn xoay từ từ thì comment dòng này lại, nhưng TurnSpeed phải để cao (15-20).
             transform.rotation = Quaternion.LookRotation(newDir); 
        }

        // Mở lại Animation
        _animator.speed = 1f; 
        
        _isWaiting = false;
    }
}