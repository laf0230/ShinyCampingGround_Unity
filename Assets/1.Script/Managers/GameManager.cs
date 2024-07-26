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

    public bool isMetInpluencer = false;
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

    // �̱��� �ν��Ͻ� �ʱ�ȭ �� �˻�
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
            uIManager.ToggleAlert(alertType.sub, "ķ���� �߾ӿ� �ִ� �����ҷ� �̵��ϼ���", true);
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
        yield return StartCoroutine(SpawnNPC(npcs[0], spawnPoint: entrance.gameObject.transform.position, npcIndex: 0));
        yield return new WaitUntil(() => entrance.IsOutCharacter());
        yield return new WaitForSeconds(17f);

        yield return StartCoroutine(SpawnNPC(npcs[1], spawnPoint: entrance.gameObject.transform.position, npcIndex: 1));
        yield return new WaitUntil(() => entrance.IsOutCharacter());
        yield return new WaitForSeconds(26f);

        yield return StartCoroutine(SpawnNPC(npcs[2], spawnPoint: negativeEntrance.transform.position, npcIndex: 2));
        yield return new WaitForSeconds(10f);

        yield return StartCoroutine(SpawnNPC(npcs[3], spawnPoint: entrance.gameObject.transform.position, npcIndex: 3));
        yield return new WaitUntil(() => entrance.IsOutCharacter());
        yield return new WaitForSeconds(17f);

        yield return StartCoroutine(SpawnNPC(npcs[4], spawnPoint: entrance.gameObject.transform.position, npcIndex: 4));
        yield return new WaitUntil(() => entrance.IsOutCharacter());
        yield return new WaitForSeconds(26f);

        yield return StartCoroutine(SpawnNPC(npcs[5], spawnPoint: negativeEntrance.transform.position, npcIndex: 5));
    }

    public IEnumerator SpawnNPC(GameObject npc, Vector3 spawnPoint, int npcIndex)
    {
        if (npc.GetComponent<NegativeCharacterController>() != null)
        {
            // ����
            SoundManager.Instance.PlaySFXMusic("NegativeEnter");
            uIManager.Alert("������ ������ ��ġ�� �־��!<br>������ ã�� �����ϼ���!", alertType.sub);
        }
        else if(!npc.GetComponent<CharacterController>().isMetFirst)
        {
            // �ߺ� ���� ĳ����
            SoundManager.Instance.PlaySFXMusic("PositiveEnter");
            uIManager.Alert("�մ��� �Ծ��!", alertType.sub);
            uIManager.AddCoin(100);
        }
        else
        {
            // ó�� �� �մ�
            SoundManager.Instance.PlaySFXMusic("PositiveEnter");
            uIManager.Alert("���ο� �մ��� �Ծ��!", alertType.main);
            uIManager.AddCoin(100);
        }

        Quaternion rotation = entrance.transform.rotation * Quaternion.Euler(0, -90, 0);

        GameObject character = Instantiate(npc, spawnPoint, rotation);
        CharacterController controller = character.GetComponent<CharacterController>();
        NegativeCharacterController negativeController = character.GetComponent<NegativeCharacterController>();

        if (controller != null)
        {
            controller.SetFirstMet(false);
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
