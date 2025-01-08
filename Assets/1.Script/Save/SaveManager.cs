using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataPersistenceManager : MonoBehaviour
{
    public List<GameData> gameDatas = new List<GameData>();

    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        if(instance != null)
        {
            Debug.Log("Already exist SaveManger");
        }
        instance = this;
        // gameDatas = FindObjectsOfTypeAll<MonoBehaviour>().OfType<GameData>();
    }

    public void NewGame()
    {

    }

    public void Save()
    {

    }

    public void Load()
    {
        
    }
}
