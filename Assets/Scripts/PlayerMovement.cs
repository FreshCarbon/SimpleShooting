using UnityEngine;

// 플레이어 캐릭터를 사용자 입력에 따라 움직이는 스크립트
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; //캐릭터 이동속도
    public float jumpSpeed = 5f;
    private bool isJumping;
    private bool isGrounded;
    public LayerMask groundLayer; // 착지 확인을 위한 레이어 마스크


    private PlayerInput playerInput; // 플레이어 캐릭터 컴포넌트
    private Rigidbody playerRigidbody; 
    private Animator playerAnimator;
 
    private Camera mainCamera;

    private void Start()
    {
        // 사용할 컴포넌트들의 참조를 가져오기
        playerInput = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
        mainCamera = Camera.main;
    }

    // FixedUpdate는 물리 갱신 주기에 맞춰 실행됨 이동 구현에 적절
    private void FixedUpdate()
    {
        
        Rotate();
        Move();
        Jump();


        // 입력값에 따라 애니메이터의 파라미터값 변경
        playerAnimator.SetFloat("MoveX", playerInput.moveHorizontal);
        playerAnimator.SetFloat("MoveY", playerInput.moveVertical);
        playerAnimator.SetBool("isJumping", playerInput.jump);
        playerAnimator.SetBool("isGrounded", isGrounded);
        
        CheckGroundStatus();

    }

    // 입력값에 따라 캐릭터 이동   
    private void Move()
    {
        // 입력값에 따라 이동할 방향 계산
        Vector3 moveDirection = new Vector3(playerInput.moveHorizontal, 0, playerInput.moveVertical).normalized;

        // 이동할 거리 계산
        Vector3 moveDistance = moveDirection * moveSpeed * Time.deltaTime;

        //리지드 바디를 이용해 게임 오브젝트 위치 변경
        playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);

    }
    private void Jump()
    {
        if(playerInput.jump && isGrounded)
        {
            isJumping = true;
            isGrounded = false;
            Debug.Log("isJumping: " + isJumping);
            Debug.Log("isGrounded: "+ isGrounded);

            //forceMode.Impulse - 무게적용,순간적인 힘, 정지출발 forece는 가속
            playerRigidbody.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse); 
            
        }
    }

    // 착지여부 확인
    private void CheckGroundStatus()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.4f, groundLayer))
        {
            isGrounded = true;
            isJumping = false;
        }
        else
        {
            isGrounded = false;
        }
    }


    // 입력값에 따라 캐릭터를 회전
    private void Rotate()
    {
        //마우스 위치를 가져오기
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            // 플레이어가 마우스를 바라보도록 회전
            Vector3 lookDirection = hit.point - transform.position;
            lookDirection.y = 0; // Y축 회전은 고려하지 않음

            Quaternion newRotation = Quaternion.LookRotation(lookDirection);
            playerRigidbody.MoveRotation(newRotation);
        }

    }
}