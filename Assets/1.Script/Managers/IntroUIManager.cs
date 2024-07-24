using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroUIManager : MonoBehaviour, IPointerClickHandler
{
    public GameObject loadingUI;

    private void Start()
    {
        SoundManager.Instance.PlayMusic("Title");
    }

    IEnumerator GoToMainScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("Main");

        yield return operation;
        SceneManager.LoadScene("Main");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GoToMainScene();
    }
}
