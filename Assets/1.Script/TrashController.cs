using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashController : MonoBehaviour
{
    public void Clean()
    {
        GameManager.Instance.uIManager.AddCoin(40);
       gameObject.SetActive(false);
    }
}
