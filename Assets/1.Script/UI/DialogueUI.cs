using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// TODO: UI와 DIalogue기능 분리
public class DialogueUI : UI
{
    public GameObject dialogue_box;
    public TextMeshProUGUI dialogue_name;
    public TextMeshProUGUI dialogue_text;
    public Button dialogueSkip;
    public Button dialogueInterectionRect;
    public Button dialogueAuto;

    public float typingSpeed = 0.05f; // 타이핑 속도 조절 변수

    public bool isSkipRequested = false;
    public bool doTest = false;
    private bool isNextDialogueRequested = false;
    private bool isTyping = false; // 타이핑 중인지 확인하는 변수
    private bool isTypeComplete = false;
    private bool isTextAuto = false;

    private void Start()
    {
        // Null 체크 추가
        if (dialogue_text == null)
        {
            Debug.LogError("dialogue_text is not assigned. Please assign a TextMeshProUGUI component in the Unity Inspector.");
            return;
        }

        // 예제 텍스트
        if (doTest)
            ActiveDialogue("Test", "테스트 텍스트입니다! 안녕하세요? <br> 하하하");

        dialogueInterectionRect.onClick.AddListener(OnNextButtonClicked);
        dialogueSkip.onClick.AddListener(OnSkipBtnClicked);
        dialogueAuto.onClick.AddListener(OnAutoTextClicked);
    }

    public void ActiveDialogue(string name, string text)
    {
        dialogue_box.SetActive(true);
        SetDialogue(name, text);
    }

    public void DisableDialogue()
    {
        isSkipRequested = false;
        isNextDialogueRequested = false;

        dialogue_box.SetActive(false);
    }

    private void SetDialogue(string name, string text)
    {
        dialogue_name.text = name;
        StartTyping(text);
    }

    public void OnSkipBtnClicked()
    {
        this.isSkipRequested = true;
        Debug.Log("Try Skip");
    }

    public bool IsSkipRequested()
    {
        return isSkipRequested;
    }

    public void OnNextButtonClicked()
    {
        if (isTyping)
        {
            SetCompleteDialogue();
        }
        else
        {
            isNextDialogueRequested = true;
        }
    }

    public bool IsNextDialogueRequested()
    {
        return isNextDialogueRequested;
    }

    // 타이핑 효과를 시작하는 메서드
    public void StartTyping(string text)
    {
        if (isTyping)
        {
            StopAllCoroutines(); // 기존의 타이핑이 진행 중이면 중지
        }

        StartCoroutine(TypeText(text));
    }

    // 타이핑 효과를 적용하는 Coroutine
    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        isTypeComplete = false; // 새로운 타이핑 시작 시 초기화
        dialogue_text.text = ""; // 텍스트를 초기화

        int index = 0;
        while (index < text.Length)
        {
            if (isTypeComplete)
            {
                // 텍스트 타이핑 이팩트 도중에 상호작용 시 전체 텍스트가 완성됩니다.
                dialogue_text.text = text;
                isTypeComplete = false;
                break;
            }
            // 현재 문자와 다음 문자 조합을 검사하여 <br> 태그를 찾습니다.
            if (text.Substring(index).StartsWith("<br>"))
            {
                // <br> 태그를 발견하면 줄바꿈을 추가
                dialogue_text.text += '\n';
                index += 4; // <br> 태그의 길이만큼 이동
            }
            else
            {
                // 일반 문자를 처리
                dialogue_text.text += text[index];
                index++;
            }

            // 타이핑 속도 조절
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

        // 타이핑이 완료된 후 자동 모드일 경우 자동으로 다음 대사로 진행
        if (isTextAuto)
        {
            yield return new WaitForSeconds(1.5f); // 자동 진행 시 대사 간 간격 조절
            isNextDialogueRequested = true;
        }
    }

    // 타이핑 효과가 진행 중인지 확인하는 메서드
    public bool IsTyping()
    {
        return isTyping;
    }

    public void SetCompleteDialogue()
    {
        isTypeComplete = true;
    }

    public void OnAutoTextClicked()
    {
        isTextAuto = !isTextAuto;
        if (isTextAuto)
        {
            dialogueAuto.GetComponent<Image>().color = new Color(1, 1, 1); // 활성화 시 흰색
            if (!isTyping && !isNextDialogueRequested) // 이미 타이핑이 완료된 상태라면 다음 대사로 넘어가기
            {
                isNextDialogueRequested = true;
            }
        }
        else
        {
            dialogueAuto.GetComponent<Image>().color = new Color(0.627f, 0.627f, 0.627f); // 비활성화 시 회색
            isSkipRequested = false;
            isNextDialogueRequested = false;
        }
    }

    public bool IsAutoText()
    {
        return isTextAuto;
    }
}

