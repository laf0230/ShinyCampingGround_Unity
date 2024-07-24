using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public bool isInterectionObj;
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("NPC") || other.CompareTag("Trash"))
        {
            if(other.CompareTag("NPC") && other.GetComponent<CharacterController>().isRandomAction)
                other.GetComponent<CharacterController>().ActiveRandomDialogue(true);

            isInterectionObj = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("NPC") || other.CompareTag("Trash"))
        {
            if(other.CompareTag("NPC"))
                other.GetComponent<CharacterController>().ActiveRandomDialogue(false);

            isInterectionObj = false;
        }
    }
}
