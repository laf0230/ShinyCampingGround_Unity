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
    public List<UI> UIS = new List<UI>();
    public GameObject[] uis;
    [Header("UIs")]
    public TitleUI titleUI;

    public GameObject mainAlert;
    public GameObject subAlert;

    public TextMeshProUGUI uiCoin;
    public TextMeshProUGUI uiCoin2;

    public DialogueUI dialogueManager;

    public GameObject Graphy;
    public Button graphySwitchBtrn;

    public Button changePlayerCharacterBtn;

    public float alertTime = 0;
    public WaitForSeconds alertDuration = new WaitForSeconds(3);

    public int coin = 0;
    public int score = 0;

    public float coinAlermTime = 1.8f;
    public GameObject coin_box;
    public TextMeshProUGUI coinCount;
    public WaitForSeconds coinAlermDuration;

    public static UIManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.LogError("2개 이상의 UIManager가 존재하고 있습니다.");
        }
    }

    private void Start()
    {
        for(int i = 0; i < UIS.Count; i++)
        {
            UIS[i].Close();
        }

        titleUI.gameObject.SetActive(true);

        coinAlermDuration = new WaitForSeconds(coinAlermTime);
        changePlayerCharacterBtn.onClick.AddListener(ChangeCharacter);
        graphySwitchBtrn.onClick.AddListener(SwitchDebugMode);
    }

    #region UI Controller
    public void ChangeUI(string name)
    {
        /*
        if(name == "Title")
        {
            GameObject title = Array.Find(uis,x => x.name == name);
            title.SetActive(true);
            SoundManager.Instance.PlayMusic("Title");
        }
        if(name == "Main")
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
        */
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(GameManager.Instance.sceneNameList[0]);
    }

    #endregion

    #region Coin

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

    #endregion

    #region Alert
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

    public IEnumerator IEDoAlert(GameObject alert)
    {
        alert.SetActive(true);
        yield return alertDuration;
        alert.SetActive(false);
    }
 
    #endregion

    public void ChangeCharacter()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var controller = player.GetComponent<PlayerController>();

        controller.SwitchCharacter();

    }

    public void SwitchDebugMode()
    {
        Graphy.SetActive(!Graphy.activeSelf);
    }

    public void SwitchTestScene()
    {
    }
}
