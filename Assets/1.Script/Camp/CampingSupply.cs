using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampingSupply : MonoBehaviour
{
    public enum SupplyType
    {
        None,
        Chair,
        Mat,
        Tent,
    }

    [SerializeField] private SupplyType supplyType;

    public void Use()
    {
        // TODO: when character use this supply
    }

    public void Exit()
    {
        // TODO: character exit this supply
    }
}
