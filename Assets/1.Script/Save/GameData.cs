using System;
using UnityEngine;

[CreateAssetMenu]
public class GameData : ScriptableObject
{
    public int Gold = 0;
    public int Leaf = 0;
    public Vector3 PlayerPosition = Vector3.zero;
    public Vector3 PlayerDirection = Vector3.zero;
    public float PlayerExprience = 0;
    public int PlayerLevel = 0;
    public NPCData NPCData = new NPCData();
}

public class NPCData
{
    public string Name;
    public int ID;
    public int visitCount = 0;
}
