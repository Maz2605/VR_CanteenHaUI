using System;
using UnityEngine;

namespace _Script
{
    public class Clock : MonoBehaviour
    {
        [Header("Clock Hand Transforms")]
        [SerializeField] private Transform hourHand;
        [SerializeField] private Transform minuteHand;
        [SerializeField] private Transform secondHand;

        [Header("Rotation Setup")]
        [SerializeField] private Vector3 rotationAxis = new Vector3(0, 0, 1);
        [SerializeField] private Vector3 handRotationOffset = new Vector3(-90, 0, 0);

        [Header("Update Settings")]
        [SerializeField] private bool useSmoothMovement = true;

        private const float DegreesPerHour = 30f;
        private const float DegreesPerMinute = 6f;
        private const float DegreesPerSecond = 6f;

        private void Awake()
        {
            if (hourHand == null || minuteHand == null || secondHand == null)
            {
                Debug.LogWarning("Một hoặc nhiều kim đồng hồ chưa được gán.", this);
                enabled = false;
            }
        }

        private void Start()
        {
            UpdateClockHands();
            
            if (!useSmoothMovement)
            {
                InvokeRepeating(nameof(UpdateClockHands), 1f, 1f);
            }
        }

        private void Update()
        {
            if (useSmoothMovement)
            {
                UpdateClockHands();
            }
        }

        private void UpdateClockHands()
        {
            DateTime time = DateTime.Now;
            
            // Tạo phép quay hiệu chỉnh từ giá trị offset
            Quaternion offsetRotation = Quaternion.Euler(handRotationOffset);

            float seconds = useSmoothMovement ? (float)time.TimeOfDay.TotalSeconds : time.Second;
            float minutes = useSmoothMovement ? (float)time.TimeOfDay.TotalMinutes : time.Minute + time.Second / 60f;
            float hours   = useSmoothMovement ? (float)time.TimeOfDay.TotalHours   : time.Hour % 12 + time.Minute / 60f;

            // Tạo các phép quay theo thời gian
            Quaternion hourTimeRotation   = Quaternion.AngleAxis(-hours * DegreesPerHour,   rotationAxis);
            Quaternion minuteTimeRotation = Quaternion.AngleAxis(-minutes * DegreesPerMinute, rotationAxis);
            Quaternion secondTimeRotation = Quaternion.AngleAxis(-seconds * DegreesPerSecond, rotationAxis);

            // Áp dụng phép quay cuối cùng: KẾT HỢP offset và thời gian
            // Phép nhân Quaternion sẽ áp dụng phép quay offset TRƯỚC, sau đó mới đến phép quay thời gian
            hourHand.localRotation   = offsetRotation * hourTimeRotation;
            minuteHand.localRotation = offsetRotation * minuteTimeRotation;
            secondHand.localRotation = offsetRotation * secondTimeRotation;
        }
    }
}