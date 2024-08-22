using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    public StateMachine stateMachine = new StateMachine();

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
    [SerializeField] public List<TalkData> talkData { get; set; }
    [SerializeField] TalkData currentTalkData;
    [SerializeField] public int currentTalkID = 0;

    public CharacterBlink characterFace { get; set; }
    private AudioSource audioSource;
    private List<TalkController> talks = new List<TalkController>();
    private GeneralTalk generalTalk;
    private SpecialTalk specialTalk;

    public bool isRandomAction = false;
    public bool isMetFirst = true;
    public bool isDestination;

    private void Awake()
    {
        generalTalk = new GeneralTalk(this);
        specialTalk = new SpecialTalk(this);

        talks.Add(specialTalk);
        talks.Add(generalTalk);
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
        rb = GetComponent<Rigidbody>();
        animator = Character.GetComponent<Animator>();
        cam = GetComponentInChildren<CinemachineVirtualCamera>();

        // npc id가 동일한 대사 불러오기
        talkData = GameManager.Instance.dataManager.talkData
            .Cast<TalkData>()
            .Where(item => item.npcID == id)
            .ToList();

        // npc 대사 중 첫번째 대사 불러오기
        currentTalkID = talkData[0].id;

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

    protected IEnumerator Talk()
    {
        TalkController talk;
        Debug.Log(gameObject.name + " : state Enter: Talk");

        // 다음 텍스트가 없을 경우 종료
        while (true)
        {
            if (characterFace != null)
                characterFace.ActiveTalk(true);

            currentTalkData = talkData.FirstOrDefault((x) => x.id == currentTalkID);

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
                break; // while break
            }

            if (talk == talks[0])
                GameManager.Instance.uIManager.dialogueManager.DisableDialogue();

            Debug.Log("Dialogue end");
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
