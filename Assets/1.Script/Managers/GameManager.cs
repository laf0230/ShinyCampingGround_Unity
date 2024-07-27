using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public UIManager uIManager;
    public WayPointManager wayPointManager;
    public EnteranceController entrance;
    public GameObject negativeEntrance;
    public GameObject ManagementOffice;
    public NavMeshSurface navmesh;
    public List<GameObject> npcs;

    public bool isMetInfluencer = false;
    public bool isMetPetOwner = false;

    private WaitForSeconds spawnDelay = new WaitForSeconds(3f);

    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    GameObject singleton = new GameObject(typeof(GameManager).ToString());
                    instance = singleton.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }

    // 싱글톤 인스턴스 초기화 시 검사
    private void Awake()
    {
        Application.targetFrameRate = 1000;

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (navmesh != null)
        {
            navmesh.BuildNavMesh();
        }

        if (uIManager != null)
        {
            uIManager.ToggleAlert(alertType.sub, "캠핑장 중앙에 있는 관리소로 이동하세요", true);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Time.timeScale = 3.0f;
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            Time.timeScale = 1.0f;
        }
    }

    public void GameStart()
    {
        uIManager.ToggleAlert(alertType.sub, "", false);
        StartCoroutine(GameSequence());
    }

    public IEnumerator GameSequence()
    {
        yield return StartCoroutine(SpawnNPC(npcs[0], entrance.gameObject.transform.position, 0));
        yield return new WaitUntil(() => entrance.IsOutCharacter());
        yield return new WaitForSeconds(17f);

        yield return StartCoroutine(SpawnNPC(npcs[1], entrance.gameObject.transform.position, 1));
        yield return new WaitUntil(() => entrance.IsOutCharacter());
        yield return new WaitForSeconds(26f);

        yield return StartCoroutine(SpawnNPC(npcs[2], negativeEntrance.transform.position, 2));
        yield return new WaitForSeconds(10f);

        yield return StartCoroutine(SpawnNPC(npcs[3], entrance.gameObject.transform.position, 3));
        yield return new WaitUntil(() => entrance.IsOutCharacter());
        yield return new WaitForSeconds(17f);

        yield return StartCoroutine(SpawnNPC(npcs[4], entrance.gameObject.transform.position, 4));
        yield return new WaitUntil(() => entrance.IsOutCharacter());
        yield return new WaitForSeconds(26f);

        yield return StartCoroutine(SpawnNPC(npcs[5], negativeEntrance.transform.position, 5));
    }

    public IEnumerator SpawnNPC(GameObject npc, Vector3 spawnPoint, int npcIndex)
    {
        GameObject character = Instantiate(npc, spawnPoint, entrance.transform.rotation * Quaternion.Euler(0, -90, 0));
        CharacterController controller = character.GetComponent<CharacterController>();
        NegativeCharacterController negativeController = character.GetComponent<NegativeCharacterController>();

        Debug.Log(npc.name == "Influencer" && isMetInfluencer == false);

        if (controller != null && negativeController == null)
        {
            if ((npc.name == "Influencer" && isMetInfluencer == false) || (npc.name == "PetOwner" && isMetPetOwner == false))
            {
                // 처음 등장한 손님
                SoundManager.Instance.PlaySFXMusic("PositiveEnter");
                uIManager.Alert("새로운 손님이 왔어요!", alertType.main);
                uIManager.AddCoin(100);
                if(character.name == "Influencer")
                {
                    isMetInfluencer = true;
                }else if(character.name == "PetOwner")
                {
                    isMetPetOwner = true;
                }
            }
            else
            {
                // 중복 등장 캐릭터
                SoundManager.Instance.PlaySFXMusic("PositiveEnter");
                uIManager.Alert("손님이 왔어요!", alertType.sub);
                uIManager.AddCoin(100);
            }
        }
        else if (negativeController != null)
        {
            // 도둑
            SoundManager.Instance.PlaySFXMusic("NegativeEnter");
            uIManager.Alert("도둑이 물건을 훔치고 있어요!<br>도둑을 찾아 제압하세요!", alertType.sub);
        }

        yield return spawnDelay;

        if (controller != null)
        {
            controller.goals = wayPointManager.GetWayPoint(npcIndex);
            Debug.Log(npcIndex);
            controller.StartAction();
        }
        else if (negativeController != null)
        {
            Debug.Log(negativeController.gameObject.name);
        }
    }
}

