using UnityEngine;

namespace _Script
{
    public class AirConditionerSwitch : MonoBehaviour
    {
        [SerializeField] private AirConditionerController airConditioner;
        [SerializeField] private Transform switchTransform;
        public void OnSwitch()
        {
            airConditioner.ToggleAC();
            switchTransform.Rotate(20, 0, 0);
        }
    }
}