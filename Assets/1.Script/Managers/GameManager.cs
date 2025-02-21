using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    public UIManager uIManager;
    public WayPointManager wayPointManager;
    public DataManager dataManager;
    public CampingManager campingManager;
    public CharacterManager characterManager;

    public EnteranceController entrance;
    public GameObject negativeEntrance;
    public GameObject ManagementOffice;
    public NavMeshSurface navmesh;
    public List<GameObject> npcs;
    [Header("Scenes")]
    public List<string> sceneNameList;

    // false == ó�� ����    // true == ó�������� �ƴ�
    public bool isMetInfluencer { get; set; } = true;
    public bool isMetPetOwner { get; set; } = true;

    private WaitForSeconds spawnDelay = new WaitForSeconds(3f);

    #region Debug

    public GameObject D_Character;

    #endregion


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
            uIManager.ToggleAlert(alertType.sub, "ķ���� �߾ӿ� �ִ� �����ҷ� �̵��ϼ���", true);
        }
    }

    private void Update()
    {
        #region Game Speed

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Alpha1)) { Time.timeScale *= 0.5f; }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Alpha2)) { Time.timeScale = 1f; }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Alpha3)) { Time.timeScale /= 0.5f; }

        #endregion

        if(Input.GetKeyDown(KeyCode.V) && D_Character != null)
        {
            StartCoroutine(SpawnNPC(D_Character, entrance.transform.position, 4));
        }
    }

    public void GameStart()
    {
        uIManager.ToggleAlert(alertType.sub, "", false);
        StartCoroutine(GameSequence());
    }

    public IEnumerator GameSequence()
    {
        // TODO: Need to check is character visited first TIme
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

        // yield return StartCoroutine(SpawnNPC(npcs[5], negativeEntrance.transform.position, 5));

        // Random Spawn
        while (true)
        {
            yield return null;
            var gameCharacters = GameObject.FindObjectsByType<NPCController>(sortMode: FindObjectsSortMode.InstanceID);

            if (gameCharacters.Length <= 5)
            {
                var randomturm = Random.Range(17f, 26f);
                yield return new WaitForSeconds(randomturm);
                var randomNum = Random.Range(1, npcs.Count);
                if (randomNum == 2 || randomNum == 5)
                {
                    yield return StartCoroutine(SpawnNPC(npcs[randomNum - 1], negativeEntrance.transform.position, randomNum - 1));
                }
                else
                {
                    yield return StartCoroutine(SpawnNPC(npcs[randomNum - 1], entrance.gameObject.transform.position, randomNum - 1));
                }
            }
            yield return new WaitUntil(() => entrance.IsOutCharacter());
        }

    }

    public IEnumerator SpawnNPC(GameObject npc, Vector3 spawnPoint, int npcIndex)
    {
        // ĳ���� ��ȯ
        // GameObject character = Instantiate(npc, spawnPoint, entrance.transform.rotation * Quaternion.Euler(0, -90, 0));
        GameObject character = characterManager.SpawnCharacter(npc.GetComponent<NPCController>());
        // ĳ������ ��Ʈ�ѷ� ��ȯ
        NPCController controller = character.GetComponent<NPCController>();
        NegativeNPCController negativeController = character.GetComponent<NegativeNPCController>();

        Debug.Log(npc.name == "Influencer" && isMetInfluencer == false);

        if (controller != null && negativeController == null)
        {
            // �������� ĳ���� ó��
            if ((npc.name == "Influencer" && isMetInfluencer == true) || (npc.name == "PetOwner" && isMetPetOwner == true))
            {
                SoundManager.Instance.PlaySFXMusic("PositiveEnter");

                // UI
                uIManager.Alert("���ο� �մ��� �Ծ��!", alertType.main);

                // ���� ȹ�� �ڵ�
                uIManager.AddCoin(100);

                // ���ʷ� ĳ���Ͱ� ������ ��
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
                // ���ʷ� ������ ĳ���Ͱ� �ƴ� ��� �˶��� ���
                controller.isVisitedFirst = false;
                SoundManager.Instance.PlaySFXMusic("PositiveEnter");
                uIManager.Alert("�մ��� �Ծ��!", alertType.sub);
                uIManager.AddCoin(100);
            }
        }
        else if (negativeController != null)
        {
            // �������� ĳ���� ó��(����, �˶�)
            SoundManager.Instance.PlaySFXMusic("NegativeEnter");
            uIManager.ToggleAlert(alertType.sub, "������ ������ ��ġ�� �־��!<br>������ ã�� �����ϼ���!", true);

        }

        yield return spawnDelay;
    }

    #region Scene Control

    public void SwitchScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }

    #endregion
}

