using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public bool isInterectionObj = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC") || other.CompareTag("Trash"))
        {
            if (other.CompareTag("NPC") && other.GetComponent<CharacterController>().isRandomAction)
            {
                other.GetComponent<CharacterController>().ActiveRandomDialogue(true);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Trash"))
            isInterectionObj = true;
        else if (other.CompareTag("NPC") && other.GetComponent<NegativeCharacterController>())
            isInterectionObj = !other.GetComponent<NegativeCharacterController>().isKnockdown;
        else if (other.CompareTag("NPC") && other.GetComponent<CharacterController>())
        {
            isInterectionObj = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("NPC") || other.CompareTag("Trash"))
        {
            if(other.CompareTag("NPC"))
                other.GetComponent<CharacterController>().ActiveRandomDialogue(false);
        }
    }

    public void SetIsInterectionObj(bool setBool)
    {
        isInterectionObj = setBool; 
    }
}
