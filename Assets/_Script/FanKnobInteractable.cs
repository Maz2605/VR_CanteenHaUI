using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FanKnobInteractable : MonoBehaviour
{
    [SerializeField] private CellingFanController _targetFan;
    [SerializeField] private Transform _knobMesh;
    [SerializeField] private float _rotationAngle = 90f;

    public void OnKnobInteracted()
    {
        if (_targetFan == null) return;

        _targetFan.NextSpeed();

        if (_knobMesh != null)
        {
            _knobMesh.Rotate(0, 0, _rotationAngle); 
        }
    }
}