using System;
using UnityEngine;

[CreateAssetMenu]
public class GameData : ScriptableObject
{
    public int Gold;
    public int Leaf;
    public Vector3 PlayerPosition;
    public Quaternion PlayerDirection;
    public float PlayerExprience;
    public int PlayerLevel;
    public NPCData NPCData;

    public GameData()
    {
        Gold = 0;
        Leaf = 0;
        PlayerPosition = Vector3.zero;
        PlayerDirection = Quaternion.identity;
        PlayerExprience = 0;
        PlayerLevel = 0;
        NPCData = new NPCData();
    }
}

public class NPCData
{
    public string Name;
    public int ID;
    public int visitCount = 0;
}
