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
        characterTransform = transform; // 캐릭터 오브젝트의 Transform을 저장
        animator = playerObject.GetComponent<Animator>(); // Animator 컴포넌트 가져오기

        rb = GetComponent<Rigidbody>();
        freeLookCamera = Camera.main;
    }

    private void Update()
    {
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        // 캐릭터 이동 처리
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
            // 캐릭터의 회전 처리
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            characterTransform.rotation = targetRotation;

            // 캐릭터의 이동 처리
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
                // 부정적 NPC
                negativeCharacter.Catehed();
            }
            else if (hit.collider.TryGetComponent<TrashController>(out TrashController trash))
            {
                Debug.Log("Detected: trash");
                // 쓰래기
                trash.Clean();
            }
        }
    }

    private void UpdateAnimator()
    {
        // Animator에 이동 상태 전달
        animator.SetBool("IsMove", isMoving);
    }
}

