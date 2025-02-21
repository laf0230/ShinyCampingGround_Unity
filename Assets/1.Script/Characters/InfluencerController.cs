using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfluencerController : NPCController
{
    private void Awake()
    {
        gameObject.name = "Influencer";
    }

    protected override IEnumerator Enter()
    {
        Debug.Log("Enter");
        goals = GameManager.Instance.wayPointManager.GetWayPoint(name);
        SetFirstMet(GameManager.Instance.characterManager.IsCharacterVisitFirst(this));

        yield return new WaitForSeconds(1f);

        if (goals != null)
        {

            Debug.Log(goals.ToString());

            yield return Talk();

            StartActionRoutine();
        }
    }

    protected override IEnumerator Tour()
    {
        int goalIndex = 0;

        while (true)
        {
            if (goals[goalIndex].GetComponent<CampSite>())
            {
                yield return Talk();
                StartActionRoutine();
                break;
            }
            else
            {
                yield return MoveTo(goals[goalIndex]);
                goalIndex++;
            }
            yield return new WaitForSeconds(2f);
        }
    }

    protected override IEnumerator Camping()
    {
        if (GameManager.Instance.campingManager.IsExistUseableCampSIte())
        {
            var camp = GameManager.Instance.campingManager.GetUseableCampSite();

            yield return MoveTo(camp.gameObject);
            yield return Build(campKit);
            yield return Talk();
            yield return RandomAction();
            yield return Pack(campKit);

            var trash = Instantiate(trashPrefab, transform.position, Quaternion.identity);
            trash.m_placedSite = camp;
        }

        StartActionRoutine();
    }

    protected override IEnumerator Exit()
    {
        yield return MoveTo(GameManager.Instance.campingManager.exit.gameObject);
        yield return Talk();
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);

        yield return null;
    }
}
