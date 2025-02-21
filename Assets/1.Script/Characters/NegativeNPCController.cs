using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NegativeNPCController : NPCController
{
    public float knockdownTime = 2.05f;
    public WaitForSeconds knockdownDelay;
    public float stealTime = 3f;
    public WaitForSeconds stealDelay;
    public bool isKnockdown = true;
    public Transform ManagementOffice;

    private void Awake()
    {
        isVisitedFirst = false;
    }

    protected override void Start()
    {
        base.Start();

        knockdownDelay = new WaitForSeconds(knockdownTime);
        Debug.Log(currentTalkID);
        Debug.Log(string.Join(",", talks));
    }

/*
    public override IEnumerator ActionSequence()
    {
        // TODO: 반복되는 코드를 줄이기
        Debug.Log("좀도둑 활동 시작!");
        yield return Enter();

        yield return MoveTo(goals[0]);
        yield return Talk(); // 4
        yield return Steal();
        yield return new WaitForSeconds(2f);
        yield return stealDelay;

        yield return MoveTo(goals[1]);
        yield return Talk(); // 3
        yield return Steal();
        yield return new WaitForSeconds(2f);
        yield return stealDelay;

        yield return MoveTo(goals[2]);
        yield return Talk(); // 2
        yield return Steal(isManagementTent: true);
        yield return new WaitForSeconds(2f);
        yield return stealDelay;

        yield return MoveTo(goals[3]);
        yield return Talk(); // 1
        yield return new WaitForSeconds(2f);

        GameManager.Instance.uIManager.ToggleAlert(alertType.sub, "", false);
        gameObject.SetActive(false);
    }
*/
    public override void SetCharacterRoutine()
    {
        characterActionsQueue.Clear();
        characterActionsQueue.Enqueue(DoEnter);
        characterActionsQueue.Enqueue(DoTheifAction);
        characterActionsQueue.Enqueue(DoExit);

        StartActionRoutine();
    }

    public void DoTheifAction() => StartCoroutine(ITheifAction());

    protected IEnumerator ITheifAction()
    {
        //  매 행동마다 CampManager으로부터 활성화된 포인트를 받는다.
        List<CampSite> visitedWayPoint = new List<CampSite>();
        var campManager = GameManager.Instance.campingManager;

        // 사용하고 있는 사이트가 존재 여부
        if(!campManager.IsEmptyCampingGround())
        {
            while (true)
            {
                // 사용 중인 사이트가 있을 경우 매 이동 시마다 확인
                foreach (var camp in campManager.m_Sites)
                {
                    if(camp.isCampSiteUsed && !visitedWayPoint.Contains(camp))
                    {
                        visitedWayPoint.Add(camp);
                        yield return MoveTo(camp.gameObject);
                        yield return Talk();
                        yield return Steal();
                        yield return stealDelay;
                    }
                }

                yield return null;

                // 총 사이트 수 - 빈 사이트 수 == 사용중인 사이트 수
                // 사용중인 사이트수 == 방문한 사이트 수 비교
                if((campManager.m_Sites.Length - campManager.m_useableCampSites.Count-1) == visitedWayPoint.Count-1)
                {
                    // 사용 중인 사이트를 모두 방문한 경우
                    break;
                }


                if (visitedWayPoint.Count >= 4) // 임의의 방문 사이트 수 제한
                    break;
            }
        }

        yield return MoveTo(GameManager.Instance.ManagementOffice);
        yield return Talk();
        yield return Steal();
        yield return stealDelay;

        StartActionRoutine();
    }

    protected override IEnumerator Enter()
    {
        yield return Talk(); // 5
        yield return new WaitForSeconds(2f);

        StartActionRoutine();
    }

    protected override IEnumerator Tour()
    {
        int campGoalCount = 0;
        goals = GameManager.Instance.campingManager.m_Sites.Select(select => select.gameObject).ToList();

        while (true)
        {
            yield return null;
            MoveTo(goals[campGoalCount]);
                
            if(goals.Count == campGoalCount)
            {
                break;
            }
            campGoalCount++;
        }

        StartActionRoutine();
    }

    protected override IEnumerator Exit()
    {
        yield return MoveTo(GameManager.Instance.campingManager.enterence.gameObject);
        yield return Talk();
        yield return new WaitForSeconds(2f);

        GameManager.Instance.uIManager.ToggleAlert(alertType.sub, "", false);
        gameObject.SetActive(false);
    }

    public void Catehed()
    {
        StartCoroutine(Knockdown());    
    }

    protected override IEnumerator MoveTo(GameObject wayPoint)
    {
        Debug.Log(wayPoint);
        return base.MoveTo(wayPoint);
    }

    IEnumerator Knockdown()
    {
        isKnockdown = true;
        MovementActive(false);

        SoundManager.Instance.PlaySFXMusic("NegativeDamaged");
        GameManager.Instance.uIManager.ToggleAlert(alertType.sub, "", false);

        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");

        foreach (GameObject npc in npcs)
        {
            NPCController characterController = npc.GetComponent<NPCController>();
            if (characterController != null && characterController.gameObject.GetComponent<NegativeNPCController>() == null)
            {
                SpeechBubbleController speechBubbleController = characterController.speechBubbleController;
                if (speechBubbleController != null)
                {
                    speechBubbleController.FlickPossitiveUP();
                }
            }
        }

        animator.SetBool("IsKnockdown", true);
        yield return knockdownDelay;
        gameObject.SetActive(false);
        animator.SetBool("IsKnockdown", false);
    }

    IEnumerator Steal(bool isManagementTent = false)
    {
        animator.SetBool("IsSteal", true);
        yield return stealDelay;
        if (isManagementTent) {
            // 코인 전부 소실
            GameManager.Instance.uIManager.AddCoin(-GameManager.Instance.uIManager.coin);
        }
        animator.SetBool("IsSteal", false);
    }

    public void MovementActive(bool isActive)
    {
        if (!isActive)
        {
            navMeshAgent.enabled = false;
        }
    }
}
