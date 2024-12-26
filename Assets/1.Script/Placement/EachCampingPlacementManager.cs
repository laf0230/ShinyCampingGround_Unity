using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EachCampingPlacementManager : MonoBehaviour, IBuilder
{
    [SerializeField] private List<CampingPlacement> tools;
    public bool isBuilding { get; private set; } = false;
    public bool isBuildFinish { get; private set; } = false;
    public float buildTurm = 0f;
    private WaitForSeconds waitForBuildTurm;

    private void Start()
    {
        waitForBuildTurm = new WaitForSeconds(buildTurm);
    }

    public void OnEnable()
    {
        tools = gameObject.GetComponentsInChildren<CampingPlacement>().ToList();
        // GameObject chair = tools.FirstOrDefault(c => c.type == CampingPlacementType.Chair).gameObject;
    }

    public void Build()
    {
        StartCoroutine(IEBuildTools(true));
    }

    public void Pack()
    {
        StartCoroutine(IEBuildTools(false));
    }

    private IEnumerator IEBuildTools(bool isBuild)
    {
        isBuilding = true;
        isBuildFinish = false;

        foreach (var t in tools)
        {
            yield return waitForBuildTurm;
            t.gameObject.SetActive(isBuild);
        }

        isBuilding = false;
        isBuildFinish = true;
    }

    public CampingPlacement GetRandomPlacement()
    {
        return tools[Random.Range(0, tools.Count - 1)];
    }
}
