using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TitleUI : UI, IPointerDownHandler
{
    [Header("Title")]
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject titleMenu;
    [SerializeField] private GameObject titleTextImage;

    [Space(10)]
    [Header("DataLoadUI")]
    [SerializeField] private GameObject dataLoadUI;
    [SerializeField] private TextMeshProUGUI dataLoadTitleText;
    public bool isLoad { get; private set; } = false;

    [Space(10)]
    [Header("ConfilmUI")]
    [SerializeField] private GameObject confilmUI;
    [SerializeField] private TextMeshProUGUI confilmTitleUIText;

    [Space(10)]
    [Header("# Variables")]
    [TextArea(5, 5)]
    [SerializeField] private string onDataNewGameText;
    [SerializeField] private string onDataLoadGameText;
    [SerializeField] private string onConfilmNewGameText;

    private void Start()
    {
        titlePanel.SetActive(true);

        titleMenu.SetActive(false);
        dataLoadUI.gameObject.SetActive(false);
        confilmUI.SetActive(false);
    }

    #region Title_Panel

    public void OnEnterButtonClick()
    {
        gameObject.SetActive(true);
        titleMenu.SetActive(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        titleMenu.SetActive(true);
        titleTextImage.SetActive(false);
    }

    #endregion

    #region Title_Menu

    public void OnNewGameClick()
    {
        dataLoadTitleText.text = onDataNewGameText;
        isLoad = false;
        confilmTitleUIText.text = onConfilmNewGameText;
        dataLoadUI.SetActive(true);
    }

    public void OnLoadGameButtonClick()
    {
        dataLoadTitleText.text = onDataLoadGameText;
        isLoad = true;
        dataLoadUI.SetActive(true);
    }

    public void OnExitGameClick()
    {
        Application.Quit();
    }

    #endregion

    #region LoadGame UI

    public void OnDataLoadUISaveFileSelected()
    {
        if (isLoad)
        {
            // OnLoad
            gameObject.SetActive(false);
        }
        else
        {
            // OnNew
            confilmUI.SetActive(true);
        }
    }

    public void OnDataLoadUIYesButtonClick()
    {
        if (isLoad)
        {
            // Load Game
            dataLoadUI.SetActive(false);
        }
        else
        {
            // New Game
            confilmTitleUIText.text = onConfilmNewGameText;
            confilmUI.SetActive(true);
        }
    }

    public void OnDataLoadUINoButtonClick()
    {
        dataLoadUI.SetActive(false);
    }

    #endregion

    #region Confilm UI

    public void OnConfilmUIYesButtonClick()
    {
        if (isLoad)
        {
            // Load Game
            
        }
        else
        {
            // New Game
            gameObject.SetActive(false);
        }
        confilmUI.SetActive(false);
    }

    public void OnConfilmUINoButtonClick()
    {
        if (isLoad)
        {
        }
        else
        {

        }
        confilmUI.SetActive(false);
    }

    #endregion
}
