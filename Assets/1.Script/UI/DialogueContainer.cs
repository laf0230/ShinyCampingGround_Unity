using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueBox
{
    public int id;
    public int npcid;
    public string npcName;
    public int cameraDirectionType;
    public int scriptType;
    public int scriptImageid;
    public int scriptImageColorId;
    public int stringId;
    public int visualEffectId;
    public int soundEffectId;
    public int animId;
    public int skipType;
    public int skillId;
    public int nextScriptId;
}

public class DialogueContainer : MonoBehaviour
{
   public List<DialogueBox> boxes = new List<DialogueBox>();
    public ArrayList elementList = new ArrayList();
}
