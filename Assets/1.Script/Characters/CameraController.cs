using UnityEngine;
using Cinemachine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

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

    private InputManager inputs;

    private void Awake()
    {
        inputs = GetComponentInParent<InputManager>();
    }

    private void Start()
    {
        // �ʱ� ī�޶� �Ÿ� ����
        UpdateCameraDistance();
    }

    private void Update()
    {

        // Joystick �Է� ���� Cinemachine FreeLook ī�޶��� X �� Y �� �Է� ������ ����
        freeLookCamera.m_XAxis.Value += inputs.rotation.x * Time.deltaTime * 100f; // X �� ȸ�� �ӵ� ����
        if (isInvertY)
        {
            freeLookCamera.m_YAxis.Value -= inputs.rotation.y * Time.deltaTime;
        }
        else
        {
            freeLookCamera.m_YAxis.Value += joystick.Vertical * Time.deltaTime; // Y �� ȸ�� �ӵ� ����
        }

        // Y�� ���� ���� (0~1 ���� ����)
        freeLookCamera.m_YAxis.Value = Mathf.Clamp(freeLookCamera.m_YAxis.Value, 0f, 1f);

        // �� �Է� ó�� (���콺)
        if (!IsPointerOverUIObject())
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0.0f)
            {
                currentDistance -= scroll * scrollZoomSpeed;
                currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
                UpdateCameraDistance();
            }
        }

        // ��ġ ����ó �Է� ó�� (����Ͽ�)
        if (Input.touchCount >= 2 && !IsTouchOverUIObject())
        {
            // ������ �� ���� ��ġ�� �����ɴϴ�.
            Touch touch0 = Input.GetTouch(Input.touchCount - 1);
            Touch touch1 = Input.GetTouch(Input.touchCount - 2);

            // ���� ��ġ ��ġ�� ���� ��ġ ��ġ ���� �Ÿ� ���̸� ���
            float prevTouchDeltaMag = (touch0.position - touch0.deltaPosition - (touch1.position - touch1.deltaPosition)).magnitude;
            float touchDeltaMag = (touch0.position - touch1.position).magnitude;

            // �Ÿ� ���̸� �̿��Ͽ� �� ����
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            currentDistance += deltaMagnitudeDiff * pinchingZoomSpeed * 0.01f;
            currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
            UpdateCameraDistance();
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
            freeLookCamera.m_Orbits[0].m_Radius = currentDistance / 2;
            freeLookCamera.m_Orbits[1].m_Radius = currentDistance;
            freeLookCamera.m_Orbits[2].m_Radius = currentDistance / 2;
        }
    }

    // UI ��� ������ �Է��� �߻��ߴ��� Ȯ���ϴ� �Լ� (���콺)
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }

    // UI ��� ������ ��ġ�� �߻��ߴ��� Ȯ���ϴ� �Լ� (�����)
    private bool IsTouchOverUIObject()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = touch.position;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            if (results.Count > 0)
            {
                return true; // ��ġ�� UI ��� ������ �߻�
            }
        }
        return false; // ��ġ�� UI ��� ������ �߻����� ����
    }
}

