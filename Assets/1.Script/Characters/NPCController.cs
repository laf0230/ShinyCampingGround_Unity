using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    protected Rigidbody rb;
    protected NavMeshAgent navMeshAgent;
    protected bool isMoveable = true;

    public enum ToolType
    {
        hammer
    }

    [SerializeField] public string characterName;
    [SerializeField] public int id;
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] public List<GameObject> goals { get; set; } = new List<GameObject>();
    [SerializeField] public CinemachineVirtualCamera cam;
    [SerializeField] public NPCInteraction interactionArea;
    [SerializeField] public Queue<System.Action> characterActionsQueue = new Queue<System.Action>();

    public CharacterBlink characterFace { get; set; }
    private AudioSource audioSource;

    #region Model Variables

    [SerializeField] protected GameObject Character;
    [SerializeField] protected GameObject hammer;
    [SerializeField] protected GameObject campKit;
    [SerializeField] protected GameObject tentTool;
    public GameObject trash;

    #endregion

    #region Animation variables

    public Animator animator { get; set; }
    [SerializeField] public float randomAnimDuration = 10f;
    [SerializeField] public float totalRandomAnimDuration = 120f;

    [SerializeField] public WaitForSeconds randomAnimSec;
    [SerializeField] public WaitForSeconds totalRandomAnimSec;

    #endregion

    #region Talk

    [SerializeField] public List<DialogueSO> dialogues;
    [SerializeField] public List<TalkData> talkData { get; set; }
    [SerializeField] public DialogueSO randomSpeech;
    private int currentTalkID { get; set; } = 0;
    
    public SpeechBubbleController speechBubbleController { get; private set; }
    
    private List<TalkController> talks = new List<TalkController>();
    private GeneralTalk generalTalk;
    private SpecialTalk specialTalk;

    #endregion

    public bool isRandomAction = false;
    public bool isMetFirst = true;
    public bool isDestination;

    protected virtual void Start()
    {
        characterFace = GetComponent<CharacterBlink>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        randomAnimSec = new WaitForSeconds(randomAnimDuration);
        totalRandomAnimSec = new WaitForSeconds(totalRandomAnimDuration);
        cam.Priority = 9;
        
        rb = GetComponent<Rigidbody>();
        animator = Character.GetComponent<Animator>();
        cam = GetComponentInChildren<CinemachineVirtualCamera>();


        speechBubbleController = GetComponentInChildren<SpeechBubbleController>();

        generalTalk = new GeneralTalk(this);
        specialTalk = new SpecialTalk(this);

        talks.Add(specialTalk);
        talks.Add(generalTalk);
        
        // NPC ID가 같은 ID를 가진 대사 목록 할당
        talkData = GameManager.Instance.dataManager.talkData
            .Cast<TalkData>()
            .Where(item => item.npcID == id)
            .ToList();

        // npc 대사 중 첫번째 대사 불러오기
        currentTalkID = talkData[0].id;

    }

    public void OnEnable()
    {
        if (characterFace != null)
            characterFace.ActiveBlink(true);
    }

    private void Update()
    {
        // stateMachine.currentState.Execute();
        
        if (GameManager.Instance.uIManager.dialogueManager.IsNextDialogueRequested() && GameManager.Instance.uIManager.dialogueManager.IsTyping())
        {
            GameManager.Instance.uIManager.dialogueManager.SetCompleteDialogue();
        }
    }

    public void StartAction()
    {
        StartCoroutine(ActionSequence());
    }

    public virtual IEnumerator ActionSequence()
    {
        // 캐릭터 수 제한
        yield return null;
        var characters = FindObjectsByType<NPCController>(sortMode: FindObjectsSortMode.None);
        var characterSameTypeCount = characters.Count(item => item.characterName == this.characterName);
/*
        if(characterSameTypeCount > 2)
        {
            yield return MoveTo(goals[goals.Count - 1]);
            yield return Talk(dialogues.Count - 1);
            gameObject.SetActive(false);

            yield break;
        }
*/
    }

    #region Phase

    protected virtual IEnumerator Enter()
    {
        yield return Talk();

        yield return new WaitForFixedUpdate();
    }

    public void SetCharacterRoutine()
    {
        Debug.Log("Routine Setted");
        characterActionsQueue.Clear();

        characterActionsQueue.Enqueue(DoEnter);
        characterActionsQueue.Enqueue(DOTour);
        characterActionsQueue.Enqueue(DoCamping);
        characterActionsQueue.Enqueue(DoExit);

        StartActionRoutine();
    }

    public void StartActionRoutine()
    {
        if(characterActionsQueue.Count > 0)
        {
            var nextAction = characterActionsQueue.Dequeue();
            nextAction.Invoke();
        } else
        {
            Debug.Log("All actions complete");
        }
    }

    public void DOTour() => StartCoroutine(Tour());

    public void DoCamping() => StartCoroutine (Camping());

    public void DoExit() => StartCoroutine(Exit());

    public void DoEnter() => StartCoroutine(Enter2()); 

    private IEnumerator Enter2()
    {
        Debug.Log("Enter");
        goals = GameManager.Instance.wayPointManager.GetWayPoint(name);
        SetFirstMet(GameManager.Instance.characterManager.IsCharacterVisitFirst(this));

        yield return new WaitForSeconds(1f);

        if (goals != null)
        {

            Debug.Log(goals.ToString());

            yield return Talk();

            StartActionRoutine();
        }
    }

    private IEnumerator Tour()
    {

        int goalIndex = 0;

        while (true)
        {
            if (goals[goalIndex].GetComponent<CampSite>())
            {
                StartActionRoutine();
                break;
            }
            else
            {
                yield return Talk();
                yield return MoveTo(goals[goalIndex]);
                goalIndex++;
            }
            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator Camping()
    {
        var camp = GameManager.Instance.campingManager.GetUseableCampSite();
        if (camp != null)
        {
            yield return MoveTo(camp.gameObject);
            yield return Talk();
            yield return Build(campKit);
            yield return RandomAction();
            yield return Pack(campKit);
            Instantiate(trash, transform.position, Quaternion.identity);
        }

        StartActionRoutine();
    }

    private IEnumerator Exit()
    {
        yield return MoveTo(GameManager.Instance.campingManager.exit.gameObject);
        yield return Talk();
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);

        yield return null;
    }

    #endregion


    #region Action
    
    protected virtual IEnumerator MoveTo(GameObject wayPoint)
    {
        yield return null;
        animator.SetBool("IsMove", true);
        while (true)
        {
            yield return null;

            if (Vector3.Distance(wayPoint.transform.position, transform.position) < 1f)
            {
                Debug.Log("Goal to target");
                animator.SetBool("IsMove", false);
                break;
            }

            transform.LookAt(wayPoint.transform.position);

            Vector3 direction = (wayPoint.transform.position - transform.position).normalized;

            if (navMeshAgent.enabled)
            {
                navMeshAgent.SetDestination(wayPoint.transform.position);
            }
        }
    }

    protected IEnumerator Build(GameObject kit)
    {
        Debug.Log("DoBuild");
        campKit = Instantiate(kit, transform.position, Quaternion.identity);
        KitController kitController = campKit.GetComponent<KitController>();
        hammer.SetActive(true);
        animator.SetBool("IsBuild", true);
        audioSource.Play();

        yield return kitController.IEBuildKit();

        animator.SetBool("IsBuild", false);

        yield return new WaitForFixedUpdate();
        hammer.SetActive(false);
        audioSource.Stop();
    }

    protected IEnumerator Pack(GameObject kit)
    {
        Debug.Log("DoPack");
        KitController kitController = campKit.GetComponent<KitController>();
        hammer.SetActive(true);
        audioSource.Play();
        animator.SetBool("IsBuild", true);

        yield return kitController.IEPackKit();

        animator.SetBool("IsBuild", false);

        yield return new WaitForFixedUpdate();
        hammer.SetActive(false);
        audioSource.Stop();
    }

    protected IEnumerator Talk()
    {
        TalkController talk;
        Debug.Log(gameObject.name + " : state Enter: Talk");

        // 다음 텍스트가 없을 경우 종료
        while (true)
        {
            if (characterFace != null)
                characterFace.ActiveTalk(true);

            var currentTalkData = talkData.FirstOrDefault((x) => x.id == currentTalkID);

            // talk의 타입 정하기
            talk = talks[currentTalkData.scriptType];
            talk.SetTalkData(currentTalkData);

            // 첫 만남이 아닐 때
            if (isMetFirst)
            {
                talk = talks[1];
            }
            else
            {
                talk = talks[currentTalkData.scriptType];
            }

            if (talk.Name != null && talk.Text != null)
            {
                yield return StartCoroutine(talk.Talk());
            }


            if (characterFace != null)
                characterFace.ActiveTalk(false);

            if (GameManager.Instance.uIManager.dialogueManager.IsSkipRequested())
            {
                Debug.Log("Skip requeset " + GameManager.Instance.uIManager.dialogueManager.IsSkipRequested());
                UIManager.instance.dialogueManager.DisableDialogue();
                break; // while break
            }

            if (talk == talks[0])
                GameManager.Instance.uIManager.dialogueManager.DisableDialogue();

            if (currentTalkData.nextScriptID == 0)
            {
                // 다음 대사가 없으면 다음 id로 넘어가는 코드
                currentTalkID++;
                break;
            }

            // 다음 대사를 현재 대사에 대입하는 코드
            currentTalkID = currentTalkData.nextScriptID;
        }


        cam.Priority = 9; // 카메라 순서 변경
    }


    #endregion

    /*
    protected IEnumerator Talk(int dialogueIndex)
    {
        TalkController talk;
        Debug.Log(gameObject.name + " : state Enter: Talk");

        // 다음 텍스트가 없을 경우 종료
        while (true)
        {
            if (characterFace != null)
                characterFace.ActiveTalk(true);

            var currentTalkData = talkData[dialogueIndex];

            // talk의 타입 정하기
            talk = talks[currentTalkData.scriptType];
            talk.SetTalkData(currentTalkData);

            // 첫 만남이 아닐 때
            if (!isMetFirst)
            {
                talk = talks[1];
            }
            else
            {
                talk = talks[currentTalkData.scriptType];
            }

            if (talk.Name != null && talk.Text != null)
            {
                yield return StartCoroutine(talk.Talk());
            }


            if (characterFace != null)
                characterFace.ActiveTalk(false);

            if (GameManager.Instance.uIManager.dialogueManager.IsSkipRequested())
            {
                Debug.Log("Skip requeset " + GameManager.Instance.uIManager.dialogueManager.IsSkipRequested());
                UIManager.instance.dialogueManager.DisableDialogue();
                break; // while break
            }

            if (talk == talks[0])
                GameManager.Instance.uIManager.dialogueManager.DisableDialogue();

            if (currentTalkData.nextScriptID == 0)
            {
                // 다음 대사가 없으면 다음 id로 넘어가는 코드
                currentTalkID++;
                break;
            }

            // 다음 대사를 현재 대사에 대입하는 코드
            currentTalkID = currentTalkData.nextScriptID;
        }


        cam.Priority = 9; // 카메라 순서 변경
    }

    */

    protected virtual IEnumerator RandomAction()
    {
        isRandomAction = true;
        KitController kitController = campKit.GetComponent<KitController>();

        float startTime = Time.time;
        string prevFurnitureTag = null;
        float animationCliplength = 0f;

        while (Time.time - startTime < totalRandomAnimDuration)
        {
            // 중복되는 애니메이션 처리
            Transform furniture = kitController.kit[Random.Range(0, kitController.kit.Length)];
            string furnitureTag = furniture.tag;
            Debug.Log(furnitureTag);

            rb.isKinematic = true;

            if (prevFurnitureTag == furnitureTag)
                Debug.Log("같은 애니메이션이 싫행되었습니다.    : " + characterName);

            // Only reset previous animations if the furniture tag changes
            if (prevFurnitureTag != null)
            {
                animator.SetBool("IsSit", false);
                animator.SetBool("IsLie", false);
                animator.SetBool("IsTent", false);
            }
            yield return MoveTo(furniture.gameObject);

            // Set the new animation
            switch (furnitureTag)
            {
                case "Sit":
                    transform.position = furniture.transform.position;
                    transform.rotation = furniture.transform.rotation;
                    animator.SetBool("IsSit", true);
                    yield return randomAnimSec;
                    break;
                case "Lie":
                    transform.position = furniture.transform.position;
                    transform.rotation = furniture.transform.rotation;
                    animator.SetBool("IsLie", true);
                    yield return randomAnimSec;
                    break;
                case "Tent":
                    transform.position = furniture.transform.position;
                    transform.rotation = furniture.transform.rotation;
                    if (tentTool != null)
                        tentTool.SetActive(true);
                    animator.SetBool("IsTent", true);

                    AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
                    if (clipInfo.Length > 0)
                    {
                        animationCliplength = clipInfo[0].clip.length;
                    }
                    yield return new WaitForSeconds(animationCliplength + 0.5f);
                    break;
                default:
                    yield return randomAnimSec;
                    break;
            }

            // Update the previous furniture tag
            prevFurnitureTag = furnitureTag;

            rb.isKinematic = false;
            if (tentTool != null)
                tentTool.SetActive(false);
        }


        animator.SetBool("IsSit", false);
        animator.SetBool("IsLie", false);
        animator.SetBool("IsTent", false);
        isRandomAction = false;
    }

    public void ActiveRandomDialogue(bool active)
    {
        if (active)
        {
            int randomIndex = Random.Range(0, randomSpeech.dialogues.Count);

            if (characterFace != null)
                characterFace.ActiveTalk(true);

            speechBubbleController.SetName(characterName);
            speechBubbleController.SetText(randomSpeech.dialogues[randomIndex]);
            speechBubbleController.ActiveBubble();
        }
        else
        {
            if (characterFace != null)
                characterFace.ActiveTalk(false);

            speechBubbleController.DisableBubble();
        }
    }

    public void FlickDialogue()
    {
        int randomIndex = Random.Range(0, randomSpeech.dialogues.Count);

        speechBubbleController.SetName(characterName);
        speechBubbleController.FlickBubble();
        speechBubbleController.SetText(randomSpeech.dialogues[randomIndex]);
    }

    public void SetFirstMet(bool isFirstMet)
    {
        this.isMetFirst = isFirstMet;
    }
    
    public void SetActiveTool(ToolType tool, bool activeTool)
    {
        switch (tool)
        {
            case ToolType.hammer:
                hammer.SetActive(activeTool);
                break;
        }
    }

    public void SetAnimation(AnimType type, bool active)
    {
        switch (type)
        {
            case AnimType.Idle:
                break;
            case AnimType.Walk:
                break;
            case AnimType.Work:
                break;
            case AnimType.Build:
                animator.SetBool("IsBuild", true);
                break;
            case AnimType.Sit:
                animator.SetBool("IsSit", true);
                break;
            case AnimType.Lie:
                animator.SetBool("IsLie", true);
                break;
            case AnimType.Help:
                break;
            case AnimType.Joy:
                break;
            case AnimType.Think:
                break;
            case AnimType.Wave:
                break;
            case AnimType.Unique:
                animator.SetBool("IsTent", true);
                break;
        }
    }
}
