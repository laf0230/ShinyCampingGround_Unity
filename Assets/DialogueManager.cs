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

    public float typingSpeed = 0.05f; // Ÿ���� �ӵ� ���� ����

    public bool isSkipRequested = false;
    public bool doTest = false;
    private bool isNextDialogueRequested = false;
    private bool isTyping = false; // Ÿ���� ������ Ȯ���ϴ� ����
    private bool isTypeComplete = false;
    private bool isTextAuto = false;

    private void Start()
    {
        // Null üũ �߰�
        if (dialogue_text == null)
        {
            Debug.LogError("dialogue_text is not assigned. Please assign a TextMeshProUGUI component in the Unity Inspector.");
            return;
        }

        // ���� �ؽ�Ʈ
        if(doTest)
            ActiveDialogue("Test", "�׽�Ʈ �ؽ�Ʈ�Դϴ�! �ȳ��ϼ���? \n ������");
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

    // Ÿ���� ȿ���� �����ϴ� �޼���
    public void StartTyping(string text)
    {
        if (isTyping)
        {
            StopAllCoroutines(); // ������ Ÿ������ ���� ���̸� ����
        }

        StartCoroutine(TypeText(text));
    }

    // Ÿ���� ȿ���� �����ϴ� Coroutine
    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        isTypeComplete = false;
        dialogue_text.text = ""; // �ؽ�Ʈ�� �ʱ�ȭ

        int index = 0;
        while (index < text.Length)
        {
            if (isTypeComplete)
            {
                // �ؽ�Ʈ Ÿ���� ����Ʈ ���߿� ��ȣ�ۿ� �� ��ü �ؽ�Ʈ�� �ϼ��˴ϴ�.
                dialogue_text.text = text;
                isTypeComplete = false;
                break;
            }
            // ���� ���ڿ� ���� ���� ������ �˻��Ͽ� <br> �±׸� ã���ϴ�.
            if (text.Substring(index).StartsWith("<br>"))
            {
                // <br> �±׸� �߰��ϸ� �ٹٲ��� �߰�
                dialogue_text.text += '\n';
                index += 4; // <br> �±��� ���̸�ŭ �̵�
            }
            else
            {
                // �Ϲ� ���ڸ� ó��
                dialogue_text.text += text[index];
                index++;
            }

            // Ÿ���� �ӵ� ����
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

        // Ÿ������ �Ϸ�� �� �ڵ� ����� ��� �ڵ����� ���� ���� ����
        if (isTextAuto)
        {
            yield return new WaitForSeconds(1.0f);
            OnNextButtonClicked();
        }
    }


    // Ÿ���� ȿ���� ���� ������ Ȯ���ϴ� �޼���
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
