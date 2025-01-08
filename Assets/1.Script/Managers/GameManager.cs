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
    public DataManager dataManager;
    public EnteranceController entrance;
    public GameObject negativeEntrance;
    public GameObject ManagementOffice;
    public NavMeshSurface navmesh;
    public List<GameObject> npcs;
    [Header("Scenes")]
    public List<string> sceneNameList;

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

        dataManager = GetComponentInChildren<DataManager>();
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
        #region Game Speed

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Alpha1)) { Time.timeScale *= 0.5f; }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Alpha2)) { Time.timeScale = 1f; }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Alpha3)) { Time.timeScale /= 0.5f; }

        #endregion
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

        Debug.Log("Spawn PetOwner");
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
        // 캐릭터 소환
        GameObject character = Instantiate(npc, spawnPoint, entrance.transform.rotation * Quaternion.Euler(0, -90, 0));
        // 캐릭터의 컨트롤러 반환
        NPCController controller = character.GetComponent<NPCController>();
        NegativeNPCController negativeController = character.GetComponent<NegativeNPCController>();

        Debug.Log(npc.name == "Influencer" && isMetInfluencer == false);

        if (controller != null && negativeController == null)
        {
            // 긍정적인 캐릭터 처리
            if ((npc.name == "Influencer" && isMetInfluencer == false) || (npc.name == "PetOwner" && isMetPetOwner == false))
            {
                SoundManager.Instance.PlaySFXMusic("PositiveEnter");

                // UI
                uIManager.Alert("새로운 손님이 왔어요!", alertType.main);

                // 코인 획득 코드
                uIManager.AddCoin(100);

                // 최초로 캐릭터가 등장할 때
                if (character.name == "Influencer")
                {
                    isMetInfluencer = true;
                }
                else if (character.name == "PetOwner")
                {
                    isMetPetOwner = true;
                }
            }
            else
            {
                // 최초로 등장한 캐릭터가 아닌 경우 알람만 띄움
                controller.isMetFirst = false;
                SoundManager.Instance.PlaySFXMusic("PositiveEnter");
                uIManager.Alert("손님이 왔어요!", alertType.sub);
                uIManager.AddCoin(100);
            }
        }
        else if (negativeController != null)
        {
            // 부정적인 캐릭터 처리(사운드, 알람)
            SoundManager.Instance.PlaySFXMusic("NegativeEnter");
            uIManager.ToggleAlert(alertType.sub, "도둑이 물건을 훔치고 있어요!<br>도둑을 찾아 제압하세요!", true);

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

    #region Scene Control

    public void SwitchScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }

    #endregion
}

