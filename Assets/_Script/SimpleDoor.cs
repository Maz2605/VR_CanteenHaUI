using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;

public class FlexibleDoubleDoor : MonoBehaviour
{
    [System.Serializable]
    public class DoorConfig
    {
        public string name = "Door Wing";
        public Transform pivotTransform;
        public Vector3 rotationAxis = Vector3.up;
        public float openAngle = 90f;

        [HideInInspector] public Quaternion closedRot;
        [HideInInspector] public Quaternion openRot;
    }

    [Header("Settings")]
    [SerializeField] private float duration = 0.8f;
    [SerializeField] private AnimationCurve motionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Doors Configuration")]
    [SerializeField] private DoorConfig doorLeft;
    [SerializeField] private DoorConfig doorRight;

    // [Header("Audio")]
    // [SerializeField] private AudioSource audioSource;
    // [SerializeField] private AudioClip actionSFX;

    private bool _isOpen = false;
    private Coroutine _moveCoroutine;

    void Start()
    {
        InitializeDoor(doorLeft);
        InitializeDoor(doorRight);
    }

    private void InitializeDoor(DoorConfig door)
    {
        if (door.pivotTransform == null) return;

        door.closedRot = door.pivotTransform.localRotation;
        door.openRot = door.closedRot * Quaternion.AngleAxis(door.openAngle, door.rotationAxis);
    }

    public void ToggleDoors()
    {
        if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);
        
        _isOpen = !_isOpen;
        
        // if (audioSource && actionSFX) audioSource.PlayOneShot(actionSFX);

        _moveCoroutine = StartCoroutine(AnimateDoors(_isOpen));
    }

    private IEnumerator AnimateDoors(bool opening)
    {
        float time = 0;

        Quaternion startLeft = doorLeft.pivotTransform ? doorLeft.pivotTransform.localRotation : Quaternion.identity;
        Quaternion startRight = doorRight.pivotTransform ? doorRight.pivotTransform.localRotation : Quaternion.identity;

        Quaternion targetLeft = opening ? doorLeft.openRot : doorLeft.closedRot;
        Quaternion targetRight = opening ? doorRight.openRot : doorRight.closedRot;

        while (time < 1)
        {
            time += Time.deltaTime / duration;
            float smooth = motionCurve.Evaluate(time);

            if (doorLeft.pivotTransform)
                doorLeft.pivotTransform.localRotation = Quaternion.Slerp(startLeft, targetLeft, smooth);
            
            if (doorRight.pivotTransform)
                doorRight.pivotTransform.localRotation = Quaternion.Slerp(startRight, targetRight, smooth);
            
            yield return null;
        }

        if (doorLeft.pivotTransform) doorLeft.pivotTransform.localRotation = targetLeft;
        if (doorRight.pivotTransform) doorRight.pivotTransform.localRotation = targetRight;
    }

    private void OnDrawGizmos()
    {
        DrawPivotGizmo(doorLeft);
        DrawPivotGizmo(doorRight);
    }

    private void DrawPivotGizmo(DoorConfig door)
    {
        if (door.pivotTransform == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(door.pivotTransform.position, 0.02f);

        Gizmos.color = Color.cyan;
        Vector3 axisDir = door.pivotTransform.TransformDirection(door.rotationAxis);
        Gizmos.DrawRay(door.pivotTransform.position, axisDir * 0.5f);

        Gizmos.color = Color.red;
        Vector3 openDir = Quaternion.AngleAxis(door.openAngle, axisDir) * door.pivotTransform.forward;
        Gizmos.DrawRay(door.pivotTransform.position, openDir * 0.5f);
    }
}