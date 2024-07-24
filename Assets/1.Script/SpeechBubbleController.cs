using System.Collections;
using TMPro;
using UnityEngine;

public class SpeechBubbleController : MonoBehaviour
{
    public GameObject speechBubble;
    public GameObject possitiveUpUI;
    public TextMeshProUGUI dialogue_name;
    public TextMeshProUGUI dialogue_text;
    public WaitForSeconds speechBubbleDuration = new WaitForSeconds(4);

    void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
    }

    public void FlickBubble()
    {
        StartCoroutine(Flick());
    }

    IEnumerator Flick()
    {
        ActiveBubble();
        yield return speechBubbleDuration;
        DisableBubble();
    }

    public void ActiveBubble()
    {
        speechBubble.SetActive(true);
    }

    public void DisableBubble()
    {
        speechBubble.SetActive(false);
    }

    public void SetName(string name)
    {
        dialogue_name.text = name;
    }

    public void SetText(string text)
    {
        dialogue_text.text = text;
    }

    public void PossitiveUIActive(bool active)
    {
        possitiveUpUI.SetActive(active);
    }

    public void FlickPossitiveUP()
    {
        StartCoroutine(IEPossitiveUpUIFlick());
    }

    IEnumerator IEPossitiveUpUIFlick()
    {
        PossitiveUIActive(true);
        yield return new WaitForSeconds(3f);
        PossitiveUIActive(false);
    }
}

