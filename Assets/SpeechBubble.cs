using UnityEngine;

public class SpeechBubble : MonoBehaviour
{
    public Transform character; // 캐릭터의 Transform을 할당받을 변수
    public Canvas canvas;       // World Space로 설정된 Canvas를 할당받을 변수

    void Update()
    {
        // Speech bubble을 캐릭터의 위치로 이동
        Vector3 characterHeadPosition = character.position + Vector3.up * 2f; // 예시로 캐릭터의 머리 위에 배치, 상대적 위치 조정 가능
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(characterHeadPosition);
        transform.position = screenPosition;

        // 캐릭터의 forward 방향을 기준으로 Speech bubble을 회전시킴
        transform.forward = character.forward;
    }
}

