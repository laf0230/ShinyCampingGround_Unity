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

    public DialogueManager dialogueManager;

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
        changePlayerCharacterBtn.onClick.AddListener(ChangeCharacter);
        graphySwitchBtrn.onClick.AddListener(SwitchDebugMode);
    }

    #region UI Controller
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

    public void ResetGame()
    {
        SceneManager.LoadScene("Main");
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
