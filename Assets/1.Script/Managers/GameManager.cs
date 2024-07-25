using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
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
        if(Input.GetKeyDown(KeyCode.F))
        {
            Time.timeScale = 3.0f;
        }
        if (Input.GetKeyDown(KeyCode.F))
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
        StartCoroutine(SpawnNPC(npcs[0], spawnPoint: entrance.gameObject.transform.position));
        yield return null;
        yield return new WaitUntil(() =>  entrance.IsOutCharacter());
        yield return new WaitForSeconds(17f);
        StartCoroutine(SpawnNPC(npcs[1], spawnPoint: entrance.gameObject.transform.position));
        yield return null;
        yield return new WaitUntil(() => entrance.IsOutCharacter());
        yield return new WaitForSeconds(26f);
        StartCoroutine(SpawnNPC(npcs[2], spawnPoint: negativeEntrance.transform.position));
    }

    public IEnumerator SpawnNPC(GameObject npc, Vector3 spawnPoint)
    {
        if (npc.GetComponent<NegativeCharacterController>() != null)
        {
            SoundManager.Instance.PlaySFXMusic("NegativeEnter");
            uIManager.Alert("도둑이 물건을 훔치고 있어요!<br>도둑을 찾아 제압하세요!", alertType.sub);
        }
        else
        {
            SoundManager.Instance.PlaySFXMusic("PositiveEnter");
            uIManager.Alert("새로운 손님이 왔어요!", alertType.main);
            uIManager.AddCoin(100);
        }

        Quaternion rotation = entrance.transform.rotation * Quaternion.Euler(0, -90, 0);

        GameObject character = Instantiate(npc, spawnPoint, rotation);
        CharacterController controller = character.GetComponent<CharacterController>();
        NegativeCharacterController negativeController = character.GetComponent<NegativeCharacterController>();

        yield return spawnDelay;

        if(controller != null)
        {
            controller.goals = wayPointManager.GetWayPoint(npc.name);
            // Debug.Log(controller.goals.ToString());
            controller.StartAction();
        }
        else if(negativeController != null)
        {
            Debug.Log(negativeController.gameObject.name);
        }
    }
}
