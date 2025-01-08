using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NegativeNPCTrigger : MonoBehaviour
{
    public bool isInPlayer = false;
    
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
            isInPlayer = true;
    }
}
