using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class UI : MonoBehaviour
{
    public void Open()
    {
        this.gameObject.SetActive(true);
    }
    
    public void Close()
    {
        this.gameObject.SetActive(false);
    }
}
