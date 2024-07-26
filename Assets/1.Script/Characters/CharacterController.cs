using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum SpeechType
{
    global,
    personal
}

public class CharacterController : MonoBehaviour
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
    [SerializeField] public int currentSiturationIndex = 0; // ��Ȳ: ��ȭ���� ����
    [SerializeField] public int currentDialogueIndex = 0; // ���� ��ȭ �δ콺: ���� ��ȭ�� ��ġ

    CharacterBlink characterFace;
    AudioSource audioSource;
    public bool isRandomAction = false;
    public bool isMetFirst = true;

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

    protected IEnumerator Talk(SpeechType _speechType)
    {
        Debug.Log(gameObject.name + " : state Enter: Talk");
        // situration setting
        SODialogue currentSituration = dialogues[currentSiturationIndex];

        Debug.Log(currentSituration.dialogues.Count);
        Debug.Log(currentDialogueIndex);

        while (currentDialogueIndex < currentSituration.dialogues.Count)
        {
            if (characterFace != null)
                characterFace.ActiveTalk(true);

            switch (_speechType)
            {
                case SpeechType.global:
                    cam.Priority = 11;
                    GameManager.Instance.uIManager.dialogueManager.ActiveDialogue(characterName, currentSituration.dialogues[currentDialogueIndex]);

                    Debug.Log(currentSituration.dialogues[currentDialogueIndex]);
                    // �ؽ�Ʈ�� �ڵ����� �귯���� ������ �� �ڵ�
                    if (!GameManager.Instance.uIManager.dialogueManager.IsAutoText())
                    {
                        // �ؽ�Ʈ�� �ۼ����� ���� && (���� ��ư�� ������ �� || ��ŵ ��ư�� ������ ��)
                        yield return null;
                        yield return new WaitUntil(() => GameManager.Instance.uIManager.dialogueManager.IsNextDialogueRequested() || GameManager.Instance.uIManager.dialogueManager.IsSkipRequested());
                    }
                    else
                    {
                        yield return new WaitUntil(() => !GameManager.Instance.uIManager.dialogueManager.IsTyping());
                        yield return new WaitForSeconds(1.5f);
                    }
                    break; // case break

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

            currentDialogueIndex++; // ���� ���� ����


            if (GameManager.Instance.uIManager.dialogueManager.IsSkipRequested())
            {
                Debug.Log("Skip requeset " + GameManager.Instance.uIManager.dialogueManager.IsSkipRequested());
                break; // while break
            }

            GameManager.Instance.uIManager.dialogueManager.DisableDialogue();
        }

        GameManager.Instance.uIManager.dialogueManager.DisableDialogue();
        cam.Priority = 9; // ī�޶� ���� ����
        currentSiturationIndex++; // ���� ��������� ����
        currentDialogueIndex = 0; // ��� ���� �ʱ�ȭ
    }


    protected virtual IEnumerator RandomAction()
    {
        isRandomAction = true;
        KitController kitController = campKit.GetComponent<KitController>();

        float startTime = Time.time;

        while (Time.time - startTime < totalRandomAnimDuration)
        {
            Transform furniture = kitController.kit[Random.Range(0, kitController.kit.Length)];
            yield return MoveTo(furniture.gameObject);
            string furnitureTag = furniture.tag;
            Debug.Log(furnitureTag);
            rb.isKinematic = true;
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
                    yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length + 1f);
                    break;
                default:
                    yield return randomAnimSec;
                    break;
            }
            rb.isKinematic = false;
            if (tentTool != null)
                tentTool.SetActive(false);

            animator.SetBool("IsSit", false);
            animator.SetBool("IsLie", false);
            animator.SetBool("IsTent", false);
        }
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

