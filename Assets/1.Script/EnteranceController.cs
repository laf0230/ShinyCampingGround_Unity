using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnteranceController : MonoBehaviour
{
    public bool isInCharacter = false;
    
    public bool IsOutCharacter()
    {
        return !isInCharacter;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("NPC"))
        {
            isInCharacter = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("NPC"))
        {
            isInCharacter = false;
        }
    }
}
