using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Face
{
    [Header("Face Type")]
    public string faceType;

    [Header("Upper")]
    public Texture2D face_Upper;
    public Texture2D face_Upper_Mask;
    public Texture2D face_Upper_Normal;

    [Header("Lower")]
    public Texture2D face_Lower;
    public Texture2D face_Lower_Normal;
    public Texture2D face_Lower_Mask;
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

    private string MAIN_TEXTURE = "_MainTex";
    private string MASK_MAP = "_MaskMap";
    private string NORMAL_MAP = "_BumpMap";

    private string MASK_MAP_KEYWORD = "_MASKMAP";
    private string NORMAL_MAP_KEYWORD = "_NORMALMAP";

    // Start is called before the first frame update

    void Awake()
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

        // Ensure defaultFace is set
        if (defaultFace != null)
        {
            SetFaceTextures(eye, defaultFace.face_Upper, defaultFace.face_Upper_Normal, defaultFace.face_Upper_Mask);
            SetFaceTextures(mouth, defaultFace.face_Lower, defaultFace.face_Lower_Normal, defaultFace.face_Lower_Mask);
        }
        else
        {
            Debug.LogError("Default face is not set.");
        }
    }

    private void OnEnable()
    {
        ActiveBlink(false);
        ActiveTalk(false);

        ActiveBlink(true);
        ActiveTalk(false);
    }

    public void ActiveBlink(bool isActive)
    {
        if (isActive)
        {
            if (blinkFace == null)
            {
                Debug.LogError("Blink face is not set.");
                return;
            }

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
                SetFaceTextures(eye, defaultFace.face_Upper, defaultFace.face_Upper_Normal, defaultFace.face_Upper_Mask);
            }
        }
    }

    public void ActiveTalk(bool isActive)
    {
        if (faces == null)
        {
            Debug.LogError("Error: Face Class is null");
            return;
        }

        try
        {
            if (isActive)
            {
                if (talkFace == null)
                {
                    Debug.LogError("Talk face is not set.");
                    return;
                }

                if (talkFaceCoroutine == null)
                {
                    Debug.Log("Start");
                    talkFaceCoroutine = StartCoroutine(TalkAnimation());
                }
            }
            else
            {
                if (talkFaceCoroutine != null)
                {
                    Debug.Log("End");
                    StopCoroutine(talkFaceCoroutine);
                    talkFaceCoroutine = null;
                    // Talk를 중지하고 기본 얼굴로 설정
                    SetFaceTextures(mouth, defaultFace.face_Lower, defaultFace.face_Lower_Normal, defaultFace.face_Lower_Mask);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Exception: " + e.Message);
        }
    }

    private IEnumerator BlinkAnimation()
    {
        Debug.Log("Blink");
        while (true)
        {
            // Normal state
            SetFaceTextures(eye, defaultFace.face_Upper, defaultFace.face_Upper_Normal, defaultFace.face_Upper_Mask);
            yield return new WaitForSeconds(2f);

            // Blink state
            SetFaceTextures(eye, blinkFace.face_Upper, blinkFace.face_Upper_Normal, blinkFace.face_Upper_Mask);
            yield return new WaitForSeconds(0.1f);

            // Back to normal
            SetFaceTextures(eye, defaultFace.face_Upper, defaultFace.face_Upper_Normal, defaultFace.face_Upper_Mask);
        }
    }

    private IEnumerator TalkAnimation()
    {
        while (true)
        {
            // Normal state
            SetFaceTextures(mouth, defaultFace.face_Lower, defaultFace.face_Lower_Normal, defaultFace.face_Lower_Mask);
            yield return new WaitForSeconds(0.5f);

            // Talk state
            SetFaceTextures(mouth, talkFace.face_Lower, talkFace.face_Lower_Normal, talkFace.face_Lower_Mask);
            yield return new WaitForSeconds(0.5f);

            // Back to normal
            SetFaceTextures(mouth, defaultFace.face_Lower, defaultFace.face_Lower_Normal, defaultFace.face_Lower_Mask);
        }
    }

    private void SetFaceTextures(Material material, Texture2D mainTexture, Texture2D normalMap, Texture2D maskMap)
    {
        material.mainTexture = mainTexture;

        if (maskMap != null)
        {
            material.SetTexture(MASK_MAP, maskMap);
            material.EnableKeyword(MASK_MAP_KEYWORD);
        }

        if (normalMap != null)
        {
            material.SetTexture(NORMAL_MAP, normalMap);
            material.EnableKeyword(NORMAL_MAP_KEYWORD);
        }
    }

    public void ActiveFace(string faceName, bool active)
    {
        switch (faceName)
        {
            case "Talk":
                if (active)
                {
                    SetFaceTextures(mouth, talkFace.face_Lower, talkFace.face_Lower_Normal, talkFace.face_Lower_Mask);
                }
                else
                {
                    SetFaceTextures(mouth, defaultFace.face_Lower, defaultFace.face_Lower_Normal, defaultFace.face_Lower_Mask);
                }
                break;

            case "Blink":
                if (active)
                {
                    SetFaceTextures(eye, blinkFace.face_Upper, blinkFace.face_Upper_Normal, blinkFace.face_Upper_Mask);
                }
                else
                {
                    SetFaceTextures(eye, defaultFace.face_Upper, defaultFace.face_Upper_Normal, defaultFace.face_Upper_Mask);
                }
                break;

            default:
                Debug.LogWarning($"Unexpected faceName: {faceName}");
                break;
        }
    }
}

