using UnityEngine;

public class SpeechBubble : MonoBehaviour
{
    public Transform character; // ĳ������ Transform�� �Ҵ���� ����
    public Canvas canvas;       // World Space�� ������ Canvas�� �Ҵ���� ����

    void Update()
    {
        // Speech bubble�� ĳ������ ��ġ�� �̵�
        Vector3 characterHeadPosition = character.position + Vector3.up * 2f; // ���÷� ĳ������ �Ӹ� ���� ��ġ, ����� ��ġ ���� ����
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(characterHeadPosition);
        transform.position = screenPosition;

        // ĳ������ forward ������ �������� Speech bubble�� ȸ����Ŵ
        transform.forward = character.forward;
    }
}

