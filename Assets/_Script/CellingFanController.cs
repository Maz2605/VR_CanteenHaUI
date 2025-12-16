using System;
using UnityEngine;

public class CellingFanController : MonoBehaviour
{
    [SerializeField] private Transform cellingFan;
    [SerializeField] private float[] speedLevels = {0f, 200f, 400f, 600f, 800f, 1000f};
    //Độ mượt khi tăng tốc
    [SerializeField] private float acceleration = 2.0f;
    //Lực cản khi tắt
    [SerializeField] private float stoppingDrag = 80f;


    private int _currentIndex = 0;
    private float _currentSpeed = 0f;
    private float _targetSpeed = 0f;

    private void Awake()
    {
        enabled = false;
    }

    private void Update()
    {
        if (_targetSpeed == 0)
        {
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, 0f, stoppingDrag * Time.deltaTime);

            if (_currentSpeed <= 0.01f)
            {
                _currentSpeed = 0f;
                enabled = false; 
            }
        }
        else
        {
            _currentSpeed = Mathf.Lerp(_currentSpeed, _targetSpeed, acceleration * Time.deltaTime);
        }

        if (cellingFan != null)
        {
            cellingFan.Rotate(Vector3.up, _currentSpeed * Time.deltaTime);
        }
    }

    public void NextSpeed()
    {
        _currentIndex = (_currentIndex + 1) % speedLevels.Length;
        _targetSpeed = speedLevels[_currentIndex];

        if (!enabled) enabled = true;
        
        Debug.Log($"Fan Speed level: {_currentIndex} ||Fan Speed: {_targetSpeed}");
        
    }

    public void PreviousSpeed()
    {
        _currentIndex = (_currentIndex - 1) % speedLevels.Length;
        _targetSpeed = speedLevels[_currentIndex];
        
        if(!enabled) enabled = true;
        Debug.Log($"Fan Speed level: {_currentIndex} ||Fan Speed: {_targetSpeed}");
    }
}
