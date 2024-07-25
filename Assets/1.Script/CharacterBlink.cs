using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Face
{
    public string faceType;
    public Texture2D face_Upper;
    public Texture2D face_Lower;
}

public class CharacterBlink : MonoBehaviour
{
    public Material eye;
    public Material mouth;

    [Header("메터리얼")]
    public List<Face> faces = new List<Face>();
    private Face defaultFace;
    private Face blinkFace;
    private Face talkFace;

    private Coroutine blinkCoroutine;
    private Coroutine talkFaceCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Face face in faces)
        {
            if (face.faceType == "Default")
            {
                defaultFace = face;
            }
            else if (face.faceType == "Blink")
            {
                blinkFace = face;
            }
            else if (face.faceType == "Talk")
            {
                talkFace = face;
            }
        }

        ActiveBlink(true); // 초기값으로 눈 깜빡임을 활성화
        ActiveTalk(true); // 초기값으로 입 움직임을 비활성화
    }

    public void ActiveBlink(bool isActive)
    {
        if (isActive)
        {
            if (blinkCoroutine == null)
            {
                blinkCoroutine = StartCoroutine(BlinkAnimation());
            }
        }
        else
        {
            if (blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
                blinkCoroutine = null;
                // Blink를 중지하고 기본 얼굴로 설정
                eye.mainTexture = defaultFace.face_Upper;
            }
        }
    }

    public void ActiveTalk(bool isActive)
    {
        if(faces == null)
        {
            Debug.Log("Error: Face Class is null");
            return;
        }

        try
        {
            if (isActive)
            {
                if (talkFaceCoroutine == null)
                {
                    talkFaceCoroutine = StartCoroutine(TalkAnimation());
                }
            }
            else
            {
                if (talkFaceCoroutine != null)
                {
                    StopCoroutine(talkFaceCoroutine);
                    talkFaceCoroutine = null;
                    // Talk를 중지하고 기본 얼굴로 설정
                    mouth.mainTexture = defaultFace.face_Lower;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Default Face is null");
        }
       
    }

    private IEnumerator BlinkAnimation()
    {
        while (true)
        {
            // Normal state
            eye.mainTexture = defaultFace.face_Upper;
            yield return new WaitForSeconds(2f);

            // Blink state
            eye.mainTexture = blinkFace.face_Upper;
            yield return new WaitForSeconds(0.1f);

            // Back to normal
            eye.mainTexture = defaultFace.face_Upper;
        }
    }

    private IEnumerator TalkAnimation()
    {
        while (true)
        {
            // Normal state
            mouth.mainTexture = defaultFace.face_Lower;
            yield return new WaitForSeconds(1f);

            // Talk state
            mouth.mainTexture = talkFace.face_Lower;
            yield return new WaitForSeconds(0.5f);

            // Back to normal
            mouth.mainTexture = defaultFace.face_Lower;
        }
    }

    public void ActiveFace(string faceName, bool active)
    {
        switch (faceName)
        {
            case "Talk":
                mouth.mainTexture = active ? talkFace.face_Lower : defaultFace.face_Lower;
                break;

            case "Blink":
                eye.mainTexture = active ? blinkFace.face_Upper : defaultFace.face_Upper;
                break;

            default:
                Debug.LogWarning($"Unexpected faceName: {faceName}");
                break;
        }
    }
}

