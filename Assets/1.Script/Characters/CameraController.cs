using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public CinemachineFreeLook freeLookCamera;
    public float cameraSensitivity = 1f;
    public float scrollZoomSpeed = 10f;
    public float pinchingZoomSpeed = 1f;
    public float minDistance = 2f;
    public float maxDistance = 15f;
    public bool isPinching;

    public float currentDistance;

    public Joystick joystick; // Joystick ��ũ��Ʈ
    public bool isInvertY = false;

    private void Start()
    {
        {
            // �ʱ� ī�޶� �Ÿ� ����
            UpdateCameraDistance();
        }
    }

    private void Update()
    {
        // Joystick �Է� ���� Cinemachine FreeLook ī�޶��� X �� Y �� �Է� ������ ����
        freeLookCamera.m_XAxis.Value += joystick.Horizontal * Time.deltaTime * 100f; // X �� ȸ�� �ӵ� ����
        if (isInvertY)
        {
            freeLookCamera.m_YAxis.Value -= joystick.Vertical * Time.deltaTime;
        }
        else
        {
            freeLookCamera.m_YAxis.Value += joystick.Vertical * Time.deltaTime; // Y �� ȸ�� �ӵ� ����
        }

        // Y�� ���� ���� (0~1 ���� ����)
        freeLookCamera.m_YAxis.Value = Mathf.Clamp(freeLookCamera.m_YAxis.Value, 0f, 1f);

        // �� �Է� ó��
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            currentDistance -= scroll * scrollZoomSpeed;
            currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
            UpdateCameraDistance();
        }

        // ��ġ ����ó �Է� ó�� (����Ͽ�)
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // ���ÿ� ��ġ�� �����ߴ��� Ȯ��
            if (touch0.phase == TouchPhase.Began && touch1.phase == TouchPhase.Began)
            {
                isPinching = true; // ��ġ ����ó Ȱ��ȭ
            }

            if (isPinching)
            {
                // ���� ��ġ ��ġ�� ���� ��ġ ��ġ ���� �Ÿ� ���̸� ���
                float prevTouchDeltaMag = (touch0.position - touch0.deltaPosition - (touch1.position - touch1.deltaPosition)).magnitude;
                float touchDeltaMag = (touch0.position - touch1.position).magnitude;

                // �Ÿ� ���̸� �̿��Ͽ� �� ����
                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                currentDistance += deltaMagnitudeDiff * pinchingZoomSpeed * 0.01f;
                currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
                UpdateCameraDistance();
            }
        }
        else
        {
            // ��ġ�� ������ ��ġ ����ó ��Ȱ��ȭ
            isPinching = false;
        }
    }

    void UpdateCameraDistance()
    {
        // �� �������� ������ ������Ʈ
        for (int i = 0; i < freeLookCamera.m_Orbits.Length; i++)
        {

            // top: 4.5 & 1.5
            // freeLookCamera.m_Orbits[i].m_Radius = newDistance;

            freeLookCamera.m_Orbits[0].m_Radius = currentDistance / 2;
            freeLookCamera.m_Orbits[1].m_Radius = currentDistance;
            freeLookCamera.m_Orbits[2].m_Radius = currentDistance / 2;
        }
    }
} 