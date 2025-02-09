using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampSite : MonoBehaviour
{
    // Has some Places for building.
    public Transform[] furnitureLoacations;
    // some Site has diffrent Size
    public SiteSize size = new SiteSize(0,0);
    // Check for Aleady Used
    public bool isCampSiteUsed { get; set; }
    public List<CampingSupply> campingSupplies {  get; private set; }
}

public struct SiteSize
{
    public int width;
    public int height;

    public SiteSize(int width, int height)
    {
        this.width = width;
        this.height = height;
    }
}
