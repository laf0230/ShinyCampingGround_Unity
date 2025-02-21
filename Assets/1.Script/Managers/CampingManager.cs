using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CampingManager : MonoBehaviour
{
    public Transform enterence;
    public Transform exit;

    public CampSite[] m_Sites;
    public List<CampSite> m_useableCampSites = new List<CampSite>();

    private void Start()
    {
        m_Sites = FindObjectsByType(typeof(CampSite), FindObjectsSortMode.None) as CampSite[];
        for (int i = 0; i < m_Sites.Length; i++)
        {
            if(! m_Sites[i].isCampSiteUsed) // ��� ������ ���� ����Ʈ ó��
                m_useableCampSites.Add(m_Sites[i]);
            else // ������� ����Ʈ ó��
                m_useableCampSites.Remove(m_Sites[i]);
        }
    }

    public bool IsExistUseableCampSIte()
    {
        if(m_useableCampSites.Count == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool IsEmptyCampingGround()
    {
        if(m_Sites.Length == m_useableCampSites.Count-1)
            return true;
        else 
            return false;
    }

    public CampSite GetUseableCampSite()
    {
        Debug.Log("����Ʈ�� �����߽����٤�.");
        // ��� ������ ����Ʈ ��ȯ
        var selectedCampSite = m_useableCampSites[0]; 
        selectedCampSite.isCampSiteUsed = true;
        m_useableCampSites.RemoveAt(0);
        return selectedCampSite;
    }

    public void CampSiteUseEnd(CampSite site)
    {
        m_useableCampSites.Add(site);
        site.isCampSiteUsed = false;
    }
}
