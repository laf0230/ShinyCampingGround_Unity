using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NegativeCharacterTrigger : MonoBehaviour
{
    public bool isInPlayer = false;
    
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
            isInPlayer = true;
    }
}
