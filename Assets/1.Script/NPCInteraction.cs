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
            if (other.CompareTag("NPC") && other.GetComponent<NPCController>().isRandomAction)
            {
                other.GetComponent<NPCController>().ActiveRandomDialogue(true);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Trash"))
            isInterectionObj = true;
        else if (other.CompareTag("NPC") && other.GetComponent<NegativeNPCController>())
            isInterectionObj = !other.GetComponent<NegativeNPCController>().isKnockdown;
        else if (other.CompareTag("NPC") && other.GetComponent<NPCController>())
        {
            isInterectionObj = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("NPC") || other.CompareTag("Trash"))
        {
            if(other.CompareTag("NPC"))
                other.GetComponent<NPCController>().ActiveRandomDialogue(false);
        }
    }

    public void SetIsInterectionObj(bool setBool)
    {
        isInterectionObj = setBool; 
    }
}
