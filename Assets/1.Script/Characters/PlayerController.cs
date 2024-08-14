using UnityEngine;
using Cinemachine;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(GroundChecker))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float gravity = 9.81f;
    public float slideSpeed = 3f;

    private Vector2 moveInput;
    private Camera freeLookCamera;
    public List<GameObject> characters;
    public GameObject playerObject;
    public SODialogue dialogues;
    public NPCInteraction nPCInteraction;
    public int currentCharacterIndex = 0;

    private CharacterController characterController;
    private Transform characterTransform;
    private Animator animator;
    private GroundChecker groundCheck;
    private InputManager inputs;
    private Vector3 velocity;
    private bool isMoving;
    private bool isSliding;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        groundCheck = GetComponent<GroundChecker>();
        inputs = GetComponent<InputManager>();
        isSliding = false;
        velocity = Vector3.zero;
    }

    private void Start()
    {
        characterTransform = transform; // 캐릭터 오브젝트의 Transform을 저장
        animator = playerObject.GetComponent<Animator>(); // Animator 컴포넌트 가져오기
        freeLookCamera = Camera.main; // 메인 카메라 참조
    }

    private void Update()
    {
        UpdateAnimator();

        if (Input.GetMouseButtonDown(0) && nPCInteraction.isInterectionObj)
        {
            ClassifyObject(Input.mousePosition);
        }

        if (Input.touchCount > 0 && nPCInteraction.isInterectionObj)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);

                if (touch.phase == TouchPhase.Began)
                {
                    Vector3 touchPosition = touch.position;
                    ClassifyObject(touchPosition);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        isMoving = inputs.inputvalue.magnitude > 0.001f;

        Vector3 cameraForward = freeLookCamera.transform.forward;
        Vector3 cameraRight = freeLookCamera.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection = cameraForward * inputs.inputvalue.y + cameraRight * inputs.inputvalue.x;

        if (moveDirection.magnitude > 0.1f)
        {
            // 캐릭터의 회전 처리
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            characterTransform.rotation = targetRotation;

            // 캐릭터의 이동 처리
            characterController.Move(moveDirection * moveSpeed * Time.fixedDeltaTime);
        }

        // 중력 처리
        if (!groundCheck.IsGrounded())
        {
            velocity.y -= gravity * Time.fixedDeltaTime; // 중력 가속도 적용
        }
        else
        {
            if (velocity.y < 0)
            {
                velocity.y = 0f; // 바닥에 닿으면 속도를 0으로
            }
        }

        characterController.Move(velocity * Time.fixedDeltaTime); // 중력에 의한 이동 적용

        SlidingCharacter();
    }

    public void SlidingCharacter()
    {
        if (!characterController.isGrounded) return;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, characterController.height / 2))
        {
            // 경사면의 기울기가 slopeLimit 보다 클경우 미끄러짐 처리
            if (Vector3.Angle(hit.normal, Vector3.up) > characterController.slopeLimit)
            {
                Vector3 slideDirection = Vector3.ProjectOnPlane(Physics.gravity, hit.normal).normalized;
                characterController.Move(slideDirection * Time.deltaTime * slideSpeed);
                isSliding = true;
                return;
            }
        }
        isSliding = false;
    }

    public void ClassifyObject(Vector3 vectorToRay)
    {
        nPCInteraction.SetIsInterectionObj(false);
        Ray pointPosition = Camera.main.ScreenPointToRay(vectorToRay);

        if (Physics.Raycast(pointPosition, out RaycastHit hit, 1000f, 1 << LayerMask.NameToLayer("Interection")))
        {
            if (hit.collider.CompareTag("NPC"))
            {
                hit.collider.GetComponent<NegativeNPCController>().Catehed();
            }

            if (hit.collider.CompareTag("Trash"))
            {
                hit.collider.GetComponent<TrashController>().Clean();
            }
        }
    }

    private void UpdateAnimator()
    {
        // Animator에 이동 상태 전달
        animator.SetBool("IsMove", isMoving);
    }

    public void SwitchCharacter()
    {
        Debug.Log("Current character index: " + currentCharacterIndex);
        Debug.Log("Max character index: " + (characters.Count - 1));

        if (currentCharacterIndex < characters.Count - 1)
        {
            currentCharacterIndex++;
        }
        else
        {
            currentCharacterIndex = 0;
        }

        var nextCharacter = characters[currentCharacterIndex];
        Debug.Log(nextCharacter.name);
        ChangeCharacterModel(nextCharacter);
    }

    public void ChangeCharacterModel(GameObject model)
    {
        playerObject.SetActive(false);
        playerObject = model;
        playerObject.SetActive(true);

        // Animation Change
        animator = model.GetComponent<Animator>();
    }
}

