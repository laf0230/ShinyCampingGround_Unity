using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfluencerController : NPCController
{
    public override IEnumerator ActionSequence()
    {
        yield return Enter();

        if (!GameManager.Instance.isMetInfluencer)
        {
            yield return Talk();
            GameManager.Instance.isMetInfluencer = true;
            isMetFirst = false;
        }
        else
        {
            yield return Talk();
        }
        
        yield return MoveTo(goals[0]);
        yield return MoveTo(goals[1]);
        yield return Talk();
        yield return new WaitForSeconds(2f);
        yield return MoveTo(goals[2]);
        yield return Build(kit: campKit);
        yield return Talk();
        yield return RandomAction();
        yield return Pack(kit: campKit);
        Instantiate(trash, position:transform.position, Quaternion.identity);
        yield return MoveTo(goals[3]);
        yield return Talk();
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }
}
