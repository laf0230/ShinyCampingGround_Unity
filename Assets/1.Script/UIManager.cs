using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum alertType
{
    main,
    sub
}

public class UIManager : MonoBehaviour
{
    public GameObject[] uis;

    public GameObject mainAlert;
    public GameObject subAlert;

    public TextMeshProUGUI uiCoin;
    public TextMeshProUGUI uiCoin2;

    public GameObject dialogue_box;
    public TextMeshProUGUI dialogue_name;
    public TextMeshProUGUI dialogue_text;
    public Button dialogueSkip;
    public Button dialogueInterectionRect;

    public float alertTime = 0;
    public WaitForSeconds alertDuration = new WaitForSeconds(3);

    public int coin = 0;
    public int score = 0;

    public float coinAlermTime = 1.8f;
    public GameObject coin_box;
    public TextMeshProUGUI coinCount;
    public WaitForSeconds coinAlermDuration;

    public bool isSkipRequested = false;
    private bool isNextDialogueRequested = false;


    private static UIManager instance;

    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIManager>();
                if (instance == null)
                {
                    GameObject singleton = new GameObject(typeof(UIManager).ToString());
                    instance = singleton.AddComponent<UIManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 자신을 파괴
        }
    }

    private void Start()
    {
        ChangeUI("Title");
        coinAlermDuration = new WaitForSeconds(coinAlermTime);
    }

    public void ChangeUI(string name)
    {
        if(name == "Title")
        {
            GameObject title = Array.Find(uis,x => x.name == name);
            title.SetActive(true);
            SoundManager.Instance.PlayMusic("Title");
        } else if(name == "Main")
        {
            GameObject[] _uis = Array.FindAll(this.uis,x => x.name != "Title");
            GameObject title = Array.Find(this.uis, x => x.name == "Title");
            SoundManager.Instance.PlayMusic("Day");
            title.SetActive(false);

            foreach (var u in _uis)
            {
                u.SetActive(true);
            }
        }
    }

    public void AddCoin(int _coin)
    {
        this.coin = this.coin + _coin;
        uiCoin.text = this.coin.ToString();

        // coin UI
        if(_coin > 0)
            coinCount.text = "+"+_coin.ToString();
        else
            coinCount.text = _coin.ToString();

        StartCoroutine(FlickCoinUI());
    }

    IEnumerator FlickCoinUI()
    {
        coin_box.SetActive(true);
        coin_box.GetComponent<Animator>().Play("ToUp", 0, 0);
        yield return coinAlermDuration;
        coin_box.SetActive(false);
    }

    public void AddScore(int score)
    {
        this.score = this.score + score;
        if(score < 0)
            uiCoin2.text = "+"+this.score.ToString();
        else
            uiCoin2.text = "-"+this.score.ToString();
    }

    public void Alert(string msg, alertType alertType)
    {
        switch (alertType)
        {
            case alertType.main:
                mainAlert.GetComponentInChildren<TextMeshProUGUI>().text = msg;
                StartCoroutine(IEDoAlert(mainAlert));
                break;

            case alertType.sub:
                subAlert.GetComponentInChildren<TextMeshProUGUI>().text = msg;
                StartCoroutine(IEDoAlert(subAlert));
                break;
        }
    }
    public void ToggleAlert(alertType alertType,string msg, bool isActive)
    {
        switch(alertType)
        {
            case alertType.main:
                mainAlert.GetComponentInChildren<TextMeshProUGUI>().text = msg;
                mainAlert.SetActive(isActive);
            break;
            case alertType.sub:
                subAlert.GetComponentInChildren<TextMeshProUGUI>().text = msg;
                subAlert.SetActive(isActive);
                break;     
        }
    }

    public void ResetGame()
    {
        SceneManager.LoadScene("Main");
    }

    public IEnumerator IEDoAlert(GameObject alert)
    {
        alert.SetActive(true);
        yield return alertDuration;
        alert.SetActive(false);
    }

    // 중앙 대사창
    public void ActiveDialogue(string name, string text)
    {
        dialogueSkip.onClick.AddListener(OnSkipBtnClicked);
        dialogueInterectionRect.onClick.AddListener(OnNextButtonClicked);

        dialogue_box.SetActive(true);
        dialogue_name.text = name;
        dialogue_text.text = text;
    }

    public void DisableDialogue()
    {
        dialogueSkip.onClick.RemoveAllListeners(); 
        dialogueInterectionRect.onClick.RemoveListener(OnNextButtonClicked);

        isSkipRequested = false;
        isNextDialogueRequested = false;
        
        dialogue_box.SetActive(false);
    }

    public void SetDialogue(string name, string text)
    {
        dialogue_name.text = name;
        dialogue_text.text = text;
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
        isNextDialogueRequested = true;
    }

    public bool IsNextDialogueRequested()
    {
        return isNextDialogueRequested;
    } 
}
