using UnityEngine;
using Cinemachine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector2 moveInput;
    private Camera freeLookCamera;
    public List<GameObject> characters;
    public GameObject playerObject;
    public SODialogue dialogues;
    public Joystick joystick;
    public NPCInteraction nPCInteraction;
    public int currentCharacterIndex = 0;

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

        if (Input.GetMouseButtonDown(0) && nPCInteraction.isInterectionObj)
        {
            Debug.Log("a");
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
    }

    public void ClassifyObject(Vector3 vectorToRay)
    {
        nPCInteraction.SetIsInterectionObj(false);
        Ray pointPosition = Camera.main.ScreenPointToRay(vectorToRay);

        if (Physics.Raycast(pointPosition, out RaycastHit hit, 1000f, 1 << LayerMask.NameToLayer("Interection")))
        {
            if(hit.collider.CompareTag("NPC"))
            {
                hit.collider.GetComponent<NegativeNPCController>().Catehed();
            }

            if(hit.collider.CompareTag("Trash"))
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
        var current_Model = playerObject; 
        playerObject.SetActive(false);
        playerObject = model;
        playerObject.SetActive(true);
        // Animation Change
        animator = model.GetComponent<Animator>();

        
    }
}

