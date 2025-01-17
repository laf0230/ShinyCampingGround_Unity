using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetOwnerController : NPCController
{
    private void Awake()
    {
        gameObject.name = "PetOwner";
    }

    public override IEnumerator ActionSequence()
    {
        yield return Enter();


        if (GameManager.Instance.isMetPetOwner == true)
        {
            GameManager.Instance.isMetPetOwner = false;
        }

        isMetFirst = !GameManager.Instance.isMetPetOwner;

        yield return Talk();

         yield return MoveTo(goals[0]);
        yield return Talk();
        yield return new WaitForSeconds(2f);
        yield return Build(kit: campKit);
        yield return RandomAction();
        yield return Pack(kit: campKit);
        Instantiate(trash, position:transform.position, Quaternion.identity);
        yield return MoveTo(goals[1]);
        yield return MoveTo(goals[2]);
        yield return Talk();
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }
}
