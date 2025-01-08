using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dailogue", menuName = "Dialogues")]
public class DialogueSO : ScriptableObject
{
    [TextArea(0, 5)]
    public List<string> dialogues = new List<string>();
}
