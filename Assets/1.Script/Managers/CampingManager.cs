using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CampingManager : MonoBehaviour
{
    public Transform enterence;
    public Transform exit;

    public CampSite[] m_Sites;
    public List<CampSite> m_useableCampSites;

    private void Start()
    {
        m_Sites = FindObjectsByType(typeof(CampSite), FindObjectsSortMode.None) as CampSite[];
        for (int i = 0; i < m_Sites.Length; i++)
        {
            if(! m_Sites[i].isCampSiteUsed) // 사용 중이지 않은 사이트 처리
                m_useableCampSites.Add(m_Sites[i]);
            else // 사용중인 사이트 처리
                m_useableCampSites.Remove(m_Sites[i]);
        }
    }

    public CampSite GetUseableCampSite()
    {
        Debug.Log("사이트를 지급했습ㄴ다ㅣ.");
        // 사용 가능한 사이트 반환
        var selectedCampSite = m_useableCampSites[0]; 
        selectedCampSite.isCampSiteUsed = true;
        if (m_useableCampSites.Count >= 1)
        {
            m_useableCampSites.RemoveAt(0);
            return selectedCampSite;
        }
        else
        {
            return null;
        }
    }

    public void CampSiteUseEnd(CampSite site)
    {
        m_useableCampSites.Add(site);
        site.isCampSiteUsed = false;
    }
}
