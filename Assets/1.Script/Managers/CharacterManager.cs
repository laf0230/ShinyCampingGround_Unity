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
    /* ĳ���� ù �湮 �ڵ�
     1. ĳ���Ͱ� ù �湮���� Ȯ���Ѵ�.
     2. ù �湮�̶�� ĳ���Ϳ��� �� ù ĳ���Ͷ�� ���� �˸���.
     3. ù �湮�� �ƴ� ������ �������� �ٲ۴�. <- �� ĳ���� ��ȯ �ø��� ����
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
            Debug.Log("ù �湮�� NPC�Դϴ�.");
            firstVisitStatus.Add(character.characterName);
            return true;
        }
        else
        {
            Debug.Log("ù �湮�� �ƴմϴ�.");
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