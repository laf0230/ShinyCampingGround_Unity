using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataLoadUI : UI
{
    public GameObject confilmUI;
    public GameObject detectedUI;

    public void OpenConfilmUI()
    {
        if (confilmUI == null)
            return;
        confilmUI.SetActive(true);
    }

    public void CloseConfilmUI()
    {
        if (confilmUI == null)
            return;
        confilmUI.SetActive(false);
    }

    public void OpenDetectedUI()
    {
        if (detectedUI == null)
            return;
        detectedUI.SetActive(true);
    }

    public void CloseDetectedUI()
    {
        if (detectedUI == null)
            return;
        detectedUI.SetActive(false);
    }
}
