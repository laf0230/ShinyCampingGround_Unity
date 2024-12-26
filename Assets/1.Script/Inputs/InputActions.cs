using UnityEngine;
using Cinemachine;

public class FreeLookCameraController : MonoBehaviour
{
    public CinemachineFreeLook freeLookCamera; // Cinemachine FreeLook ī�޶�
    public Joystick joystick; // Joystick ��ũ��Ʈ
    public bool isInvertY = false;

    void Update()
    {
        // Joystick �Է� ���� Cinemachine FreeLook ī�޶��� X �� Y �� �Է� ������ ����
        freeLookCamera.m_XAxis.Value += joystick.Horizontal * Time.deltaTime * 300f; // X �� ȸ�� �ӵ� ����
        if (isInvertY)
        {
            freeLookCamera.m_YAxis.Value -= joystick.Vertical * Time.deltaTime * 2f;
        }
        else 
        {
            freeLookCamera.m_YAxis.Value += joystick.Vertical * Time.deltaTime * 2f; // Y �� ȸ�� �ӵ� ����
        }

        // Y�� ���� ���� (0~1 ���� ����)
        freeLookCamera.m_YAxis.Value = Mathf.Clamp(freeLookCamera.m_YAxis.Value, 0f, 1f);
    }
}

