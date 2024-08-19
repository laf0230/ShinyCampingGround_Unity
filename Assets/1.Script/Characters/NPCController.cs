using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public enum SpeechType
{
    global,
    personal
}

public class NPCController : MonoBehaviour
{
    public enum States
    {
        Idle,
        Move,
        Talk,
        Build,
        Pack,
        Exit
    }

    public List<States> states = new List<States>();

    protected Rigidbody rb;
    protected NavMeshAgent navMeshAgent;
    protected bool isMoveable = true;

    public GameObject tentTool;
    public Animator animator { get; set; }
    public GameObject trash;

    [SerializeField] public string characterName;
    [SerializeField] public int id;
    [SerializeField] public GameObject Character;
    [SerializeField] public GameObject hammer;
    [SerializeField] public GameObject campKit;
    // [SerializeField] public 
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] public GameObject goal;
    [SerializeField] public List<GameObject> goals = new List<GameObject>();
    [SerializeField] public CinemachineVirtualCamera cam;
    [SerializeField] public SpeechBubbleController SpeechBubbleController;
    [SerializeField] public List<SODialogue> dialogues;
    [SerializeField] public SODialogue randomSpeech;
    [SerializeField] public NPCInteraction interactionArea;

    [SerializeField] public float randomAnimDuration = 10f;
    [SerializeField] public float totalRandomAnimDuration = 120f;

    [SerializeField] public WaitForSeconds randomAnimSec;
    [SerializeField] public WaitForSeconds totalRandomAnimSec;
    [SerializeField] public int currentSiturationIndex = 0; // 상황: 대화들의 묶음
    [SerializeField] public int currentDialogueIndex = 0; // 현제 대화 인댁스: 현제 대화의 위치
    [SerializeField] List<TalkData> talkData;
    [SerializeField] TalkData currentTalkData;
    [SerializeField] public int dialogueIndex = 0;
    [SerializeField] public int currentDialogueID = 0;

    CharacterBlink characterFace;
    AudioSource audioSource;
    public bool isRandomAction = false;
    public bool isMetFirst = true;

    private void Awake()
    {
        
    }

    protected virtual void Start()
    {
        characterFace = GetComponent<CharacterBlink>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        randomAnimSec = new WaitForSeconds(randomAnimDuration);
        totalRandomAnimSec = new WaitForSeconds(totalRandomAnimDuration);
        cam.Priority = 9;
    }

    public void OnEnable()
    {
        isMetFirst = true;
        rb = GetComponent<Rigidbody>();
        animator = Character.GetComponent<Animator>();
        cam = GetComponentInChildren<CinemachineVirtualCamera>();

        // npc id가 동일한 대사 불러오기
        talkData = GameManager.Instance.dataManager.talkData
            .Cast<TalkData>()
            .Where(item => item.npcID == id)
            .ToList();

        // npc 대사 중 첫번째 대사 불러오기
        currentDialogueID = talkData[0].id;

        if (characterFace != null)
            characterFace.ActiveBlink(true);
    }

    private void Update()
    {
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
        yield return null;
    }

    protected virtual IEnumerator Enter()
    {

        yield return new WaitForFixedUpdate();
    }

    protected virtual IEnumerator MoveTo(GameObject wayPoint)
    {
        animator.SetBool("IsMove", true);
        while (true)
        {
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

            // rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);


            yield return new WaitForFixedUpdate();
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

    protected IEnumerator Talk(SpeechType speechType)
    {
        var _speechType = speechType;

        Debug.Log(gameObject.name + " : state Enter: Talk");
        // Before code
        SODialogue currentSituration = dialogues[currentSiturationIndex];

        // Afeter Code
        // next id가 0인 경우 대화 끝
        
       
        Debug.Log(currentSituration.dialogues.Count);
        
        // 다음 텍스트가 없을 경우 종료
        while (true)
        {
            if (characterFace != null)
                characterFace.ActiveTalk(true);

            switch (_speechType)
            {
                case SpeechType.global:
                    cam.Priority = 11;

                   currentTalkData = talkData.FirstOrDefault(x => x.id == currentDialogueID);
                   if(currentTalkData == null)
                   {
                       Debug.Log($"TalkData with ID {currentDialogueID}is not found");
                   }
            
                   Debug.Log(currentTalkData);
 

                    var name = currentTalkData.npcName;
                    var text = GameManager.Instance.dataManager.GetScriptData(currentTalkData.stringID).text;

                    if(name == null)
                    {
                        Debug.Log(currentTalkData.npcName + "Character name is null");
                    }

                    GameManager.Instance.uIManager.dialogueManager.ActiveDialogue(name, text);
                    // 텍스트가 완성되지 않았을 때 오토 해제 시 자동 넘어가기 취소
                    if (!GameManager.Instance.uIManager.dialogueManager.IsAutoText())
                    {
                        // 터치 혹은 스킵
                        yield return null;
                        yield return new WaitUntil(() => GameManager.Instance.uIManager.dialogueManager.IsNextDialogueRequested() || GameManager.Instance.uIManager.dialogueManager.IsSkipRequested());
                    }
                    else
                    {
                        // 오토 모드
                        yield return new WaitUntil(() => !GameManager.Instance.uIManager.dialogueManager.IsTyping());

                        // 오토 모드가 해제되었는지 확인
                        if (!GameManager.Instance.uIManager.dialogueManager.IsAutoText())
                        {
                            // 오토 모드가 해제되었을 때, 사용자가 다음 대사나 스킵을 요청할 때까지 대기
                            yield return new WaitUntil(() => GameManager.Instance.uIManager.dialogueManager.IsNextDialogueRequested() || GameManager.Instance.uIManager.dialogueManager.IsSkipRequested());
                        }
                        else
                        {
                            // 오토 모드가 유지되는 경우, 일정 시간 대기 후 다음 대사로 이동
                            yield return new WaitForSeconds(1.5f);
                        }
                    }
                    break;
                // case break

                case SpeechType.personal:
                    if (characterFace != null)
                        characterFace.ActiveTalk(true);

                    SpeechBubbleController.SetName(characterName);
                    SpeechBubbleController.SetText(currentSituration.dialogues[currentDialogueIndex]);
                    SpeechBubbleController.FlickBubble();
                    break; // case break
            }

            if (characterFace != null)
                characterFace.ActiveTalk(false);

            currentDialogueIndex++; // 다음 대사로 변경


            if (GameManager.Instance.uIManager.dialogueManager.IsSkipRequested())
            {
                Debug.Log("Skip requeset " + GameManager.Instance.uIManager.dialogueManager.IsSkipRequested());
                break; // while break
            }

            if (speechType == SpeechType.global)
                GameManager.Instance.uIManager.dialogueManager.DisableDialogue();

            Debug.Log("Dialogue end");
            if(currentTalkData.nextScriptID == 0)
            {
                // 다음 대사가 없으면 다음 id로 넘어가는 코드
                currentDialogueID++;
                break;
            }

            // 다음 대사를 현재 대사에 대입하는 코드
            currentDialogueID = currentTalkData.nextScriptID; 
        }

        if (speechType == SpeechType.global)
            GameManager.Instance.uIManager.dialogueManager.DisableDialogue();
        
        cam.Priority = 9; // 카메라 순서 변경
        currentSiturationIndex++; // 다음 대사집으로 변경
        currentDialogueIndex = 0; // 대사 순서
    }

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

            // Reset all animations to false if there is a change
            bool animationChanged = false;

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
                        animationChanged = true;
                        yield return randomAnimSec;
                        break;
                    case "Lie":
                        transform.position = furniture.transform.position;
                        transform.rotation = furniture.transform.rotation;
                        animator.SetBool("IsLie", true);
                        animationChanged = true;
                        yield return randomAnimSec;
                        break;
                    case "Tent":
                        transform.position = furniture.transform.position;
                        transform.rotation = furniture.transform.rotation;
                        if (tentTool != null)
                            tentTool.SetActive(true);
                        animator.SetBool("IsTent", true);
                        animationChanged = true;

                        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
                        if(clipInfo.Length > 0)
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

            SpeechBubbleController.SetName(characterName);
            SpeechBubbleController.SetText(randomSpeech.dialogues[randomIndex]);
            SpeechBubbleController.ActiveBubble();
        }
        else
        {
            if (characterFace != null)
                characterFace.ActiveTalk(false);

            SpeechBubbleController.DisableBubble();
        }
    }

    public void FlickDialogue()
    {
        int randomIndex = Random.Range(0, randomSpeech.dialogues.Count);

        SpeechBubbleController.SetName(characterName);
        SpeechBubbleController.FlickBubble();
        SpeechBubbleController.SetText(randomSpeech.dialogues[randomIndex]);
    }

    public void SetFirstMet(bool isFirstMet)
    {
        this.isMetFirst = isFirstMet;
    }
}

