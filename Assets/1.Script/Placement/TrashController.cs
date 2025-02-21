using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashController : MonoBehaviour
{
    public CampSite m_placedSite { get; set; }

    public void Clean()
    {
        GameManager.Instance.uIManager.AddCoin(40);
        GameManager.Instance.campingManager.CampSiteUseEnd(m_placedSite);
       gameObject.SetActive(false);
    }
}
