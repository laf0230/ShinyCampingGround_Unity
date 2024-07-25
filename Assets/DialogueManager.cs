using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
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
        if(doTest)
            ActiveDialogue("Test", "테스트 텍스트입니다! 안녕하세요? \n 하하하");
    }

    public void ActiveDialogue(string name, string text)
    {
        dialogue_box.SetActive(true);
        SetDialogue(name, text);
    }

    public void DisableDialogue()
    {
        dialogueSkip.onClick.RemoveAllListeners(); 
        dialogueInterectionRect.onClick.RemoveListener(OnNextButtonClicked);
        dialogueAuto.onClick.RemoveAllListeners();

        isSkipRequested = false;
        isNextDialogueRequested = false;

        dialogue_box.SetActive(false);
    }

    private void SetDialogue(string name, string text)
    {
        dialogueSkip.onClick.AddListener(OnSkipBtnClicked);
        dialogueInterectionRect.onClick.AddListener(OnNextButtonClicked);
        dialogueAuto.onClick.AddListener(OnAutoTextClicked);

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
        isTypeComplete = false;
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
            yield return new WaitForSeconds(1.0f);
            OnNextButtonClicked();
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
        isTextAuto =! isTextAuto;
        if (isTextAuto)
        {
            dialogueAuto.GetComponent<Image>().color = new Color(160, 160, 160);
        } else
        {
            dialogueAuto.GetComponent<Image>().color = new Color(255, 255, 255);
        }
    }

    public bool IsAutoText()
    {
        return isTextAuto;
    }
}
