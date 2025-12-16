using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class HandSpecificAttach : MonoBehaviour
{
    public Transform leftHandGrip;
    public Transform rightHandGrip;
    
    private XRGrabInteractable _grabInteractable;

    private void Awake()
    {
        _grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        _grabInteractable.selectEntered.AddListener(OnGrab);
    }
    
    private void OnDisable()
    {
        _grabInteractable.selectEntered.RemoveListener(OnGrab);
    }
    
    private void OnGrab(SelectEnterEventArgs args)
    {
        // Kiểm tra xem cái tay vừa cầm (Interactor) là Trái hay Phải
        if (args.interactorObject.transform.CompareTag("LeftHand") || args.interactorObject.transform.name.Contains("Left"))
        {
            // Gán điểm neo thành tay trái
            _grabInteractable.attachTransform = leftHandGrip;
        }
        else if (args.interactorObject.transform.CompareTag("RightHand") || args.interactorObject.transform.name.Contains("Right"))
        {
            // Gán điểm neo thành tay phải
            _grabInteractable.attachTransform = rightHandGrip;
        }
    }
}
