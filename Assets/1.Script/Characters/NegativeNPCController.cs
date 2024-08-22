using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NegativeNPCController : NPCController
{
    public new bool isMetFirst = false;
    public float knockdownTime = 2.05f;
    public WaitForSeconds knockdownDelay;
    public float stealTime = 3f;
    public WaitForSeconds stealDelay;
    public bool isKnockdown = true;
    public Transform ManagementOffice;


    protected override void Start()
    {
        base.Start();

        knockdownDelay = new WaitForSeconds(knockdownTime);
    }

    public override IEnumerator ActionSequence()
    {
        Debug.Log("좀도둑 활동 시작!");
        yield return Enter();

        yield return Talk(); // 5
        yield return new WaitForSeconds(2f);

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

    public void Catehed()
    {
        StartCoroutine(Knockdown());    
    }

    protected override IEnumerator MoveTo(GameObject wayPoint)
    {
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
                SpeechBubbleController speechBubbleController = characterController.SpeechBubbleController;
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
