using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterWayPoint
{
    public string name;
    public List<GameObject> waypoints;
}

public class WayPointManager : MonoBehaviour
{
    public List<CharacterWayPoint> characterWayPoint = new List<CharacterWayPoint>();

    public List<GameObject> GetWayPoint(string name)
    {
        foreach (CharacterWayPoint c in characterWayPoint)
        {
            if (c.name == name)
            {
                return c.waypoints;
            }
        }
            return null;
    }
}
