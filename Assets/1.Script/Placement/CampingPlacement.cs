using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CampingPlacementType
{
    Tent,
    Chair,
    Mattress
}

public class CampingPlacement : Placement
{
    public CampingPlacementType type;
}
