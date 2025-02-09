using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    // TODO: Controlling Character Action Phase.
    // Enter CampingGround, Do Tour, Do Campiing, Exit CampingGround
    /* 캐릭터 첫 방문 코드
     1. 캐릭터가 첫 방문인지 확인한다.
     2. 첫 방문이라면 캐릭터에게 넌 첫 캐릭터라는 것을 알린다.
     3. 첫 방문이 아닌 것으로 설정값을 바꾼다. <- 매 캐릭터 소환 시마다 지정
    */
    [SerializeField] private List<NPCController> characterPool = new List<NPCController>();
    public List<string> firstVisitStatus = new List<string>();

    #region Pool

    public GameObject SpawnCharacter(NPCController character)
    {
        Debug.Log(character.name);
        var useableCharacter = characterPool.FirstOrDefault(c => c == character && !c.gameObject.activeSelf);
        if (useableCharacter != null)
        {
            useableCharacter.gameObject.SetActive(true);
            useableCharacter.transform.position = GameManager.Instance.campingManager.enterence.position;
            return useableCharacter.gameObject;
        }
        else
        {
            var instantiatedCharacter = Instantiate(character, GameManager.Instance.campingManager.enterence.position, GameManager.Instance.campingManager.enterence.transform.rotation * Quaternion.Euler(0, -90, 0));
            characterPool.Add(instantiatedCharacter);
            instantiatedCharacter.SetCharacterRoutine();
            return instantiatedCharacter.gameObject;
        }
    }

    #endregion

    public bool IsCharacterVisitFirst(NPCController character)
    {
        if (firstVisitStatus.Contains(character.characterName))
        {
            Debug.Log("첫 방문인 NPC입니다.");
            firstVisitStatus.Add(character.characterName);
            return true;
        }
        else
        {
            Debug.Log("첫 방문이 아닙니다.");
            return false;
        }
    }
}




public class Phase
{
    NPCController controller;
    NPCPhaseType phaseType;
}

public enum NPCPhaseType
{
    Enter,
    Tour,
    Camping,
    Exit,
}