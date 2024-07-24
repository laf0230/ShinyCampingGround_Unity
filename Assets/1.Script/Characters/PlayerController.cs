using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector2 moveInput;
    public Camera freeLookCamera;
    public GameObject playerObject;
    public SODialogue dialogues;
    public Joystick joystick;
    public NPCInteraction nPCInteraction;

    private Transform characterTransform;
    private Rigidbody rb;
    private Animator animator;
    private bool isMoving;

    private void Start()
    {
        characterTransform = transform; // ĳ���� ������Ʈ�� Transform�� ����
        animator = playerObject.GetComponent<Animator>(); // Animator ������Ʈ ��������

        rb = GetComponent<Rigidbody>();
        freeLookCamera = Camera.main;
    }

    private void Update()
    {
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        // ĳ���� �̵� ó��
        float moveHorizontal = joystick.Horizontal;
        float moveVertical = joystick.Vertical;

        isMoving = moveHorizontal != 0 || moveVertical != 0;

        Vector3 cameraForward = freeLookCamera.transform.forward;
        Vector3 cameraRight = freeLookCamera.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection = cameraForward * moveVertical + cameraRight * moveHorizontal;

        if (moveDirection.magnitude > 0.1f)
        {
            // ĳ������ ȸ�� ó��
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            characterTransform.rotation = targetRotation;

            // ĳ������ �̵� ó��
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
        }

        if (Input.GetMouseButtonDown(0) && nPCInteraction.isInterectionObj)
        {
            ClassifyObject(Input.mousePosition);
        }

        if (Input.touchCount > 0 && nPCInteraction.isInterectionObj)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Vector3 touchPosition = touch.position;
                ClassifyObject(touchPosition);
            }
        }

        if(nPCInteraction.isInterectionObj)
        {
            // npc random dialogue
        }
    }

    public void ClassifyObject(Vector3 vectorToRay)
    {
        Ray pointPosition = Camera.main.ScreenPointToRay(vectorToRay);

        if (Physics.Raycast(pointPosition, out RaycastHit hit,Mathf.Infinity, layerMask: LayerMask.GetMask("Interection")))
        {
            Debug.Log("Cathched: " + hit.collider.name);
            if (hit.collider.TryGetComponent<NegativeCharacterController>(out NegativeCharacterController negativeCharacter))
            {
                // ������ NPC
                negativeCharacter.Catehed();
            }
            else if (hit.collider.TryGetComponent<TrashController>(out TrashController trash))
            {
                Debug.Log("Detected: trash");
                // ������
                trash.Clean();
            }
        }
    }

    private void UpdateAnimator()
    {
        // Animator�� �̵� ���� ����
        animator.SetBool("IsMove", isMoving);
    }
}

