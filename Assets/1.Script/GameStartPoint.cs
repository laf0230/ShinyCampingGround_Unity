using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.GameStart();
            gameObject.SetActive(false);
        }
    }
}
