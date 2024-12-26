using UnityEngine;
using Cinemachine;

public class FreeLookCameraController : MonoBehaviour
{
    public CinemachineFreeLook freeLookCamera; // Cinemachine FreeLook 카메라
    public Joystick joystick; // Joystick 스크립트
    public bool isInvertY = false;

    void Update()
    {
        // Joystick 입력 값을 Cinemachine FreeLook 카메라의 X 및 Y 축 입력 값으로 설정
        freeLookCamera.m_XAxis.Value += joystick.Horizontal * Time.deltaTime * 300f; // X 축 회전 속도 조절
        if (isInvertY)
        {
            freeLookCamera.m_YAxis.Value -= joystick.Vertical * Time.deltaTime * 2f;
        }
        else 
        {
            freeLookCamera.m_YAxis.Value += joystick.Vertical * Time.deltaTime * 2f; // Y 축 회전 속도 조절
        }

        // Y축 값을 제한 (0~1 범위 내로)
        freeLookCamera.m_YAxis.Value = Mathf.Clamp(freeLookCamera.m_YAxis.Value, 0f, 1f);
    }
}

