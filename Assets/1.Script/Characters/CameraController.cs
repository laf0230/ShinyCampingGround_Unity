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

    public Joystick joystick; // Joystick 스크립트
    public bool isInvertY = false;

    private void Start()
    {
        {
            // 초기 카메라 거리 설정
            UpdateCameraDistance();
        }
    }

    private void Update()
    {
        // Joystick 입력 값을 Cinemachine FreeLook 카메라의 X 및 Y 축 입력 값으로 설정
        freeLookCamera.m_XAxis.Value += joystick.Horizontal * Time.deltaTime * 100f; // X 축 회전 속도 조절
        if (isInvertY)
        {
            freeLookCamera.m_YAxis.Value -= joystick.Vertical * Time.deltaTime;
        }
        else
        {
            freeLookCamera.m_YAxis.Value += joystick.Vertical * Time.deltaTime; // Y 축 회전 속도 조절
        }

        // Y축 값을 제한 (0~1 범위 내로)
        freeLookCamera.m_YAxis.Value = Mathf.Clamp(freeLookCamera.m_YAxis.Value, 0f, 1f);

        // 휠 입력 처리
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            currentDistance -= scroll * scrollZoomSpeed;
            currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
            UpdateCameraDistance();
        }

        // 핀치 제스처 입력 처리 (모바일용)
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // 동시에 터치를 시작했는지 확인
            if (touch0.phase == TouchPhase.Began && touch1.phase == TouchPhase.Began)
            {
                isPinching = true; // 핀치 제스처 활성화
            }

            if (isPinching)
            {
                // 이전 터치 위치와 현재 터치 위치 간의 거리 차이를 계산
                float prevTouchDeltaMag = (touch0.position - touch0.deltaPosition - (touch1.position - touch1.deltaPosition)).magnitude;
                float touchDeltaMag = (touch0.position - touch1.position).magnitude;

                // 거리 차이를 이용하여 줌 조절
                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                currentDistance += deltaMagnitudeDiff * pinchingZoomSpeed * 0.01f;
                currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
                UpdateCameraDistance();
            }
        }
        else
        {
            // 터치가 끝나면 핀치 제스처 비활성화
            isPinching = false;
        }
    }

    void UpdateCameraDistance()
    {
        // 각 오르빗의 반지름 업데이트
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