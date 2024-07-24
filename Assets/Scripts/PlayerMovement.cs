using UnityEngine;

// �÷��̾� ĳ���͸� ����� �Է¿� ���� �����̴� ��ũ��Ʈ
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; //ĳ���� �̵��ӵ�
    public float jumpSpeed = 5f;
    private bool isJumping;
    private bool isGrounded;
    public LayerMask groundLayer; // ���� Ȯ���� ���� ���̾� ����ũ


    private PlayerInput playerInput; // �÷��̾� ĳ���� ������Ʈ
    private Rigidbody playerRigidbody; 
    private Animator playerAnimator;
 
    private Camera mainCamera;

    private void Start()
    {
        // ����� ������Ʈ���� ������ ��������
        playerInput = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
        mainCamera = Camera.main;
    }

    // FixedUpdate�� ���� ���� �ֱ⿡ ���� ����� �̵� ������ ����
    private void FixedUpdate()
    {
        
        Rotate();
        Move();
        Jump();


        // �Է°��� ���� �ִϸ������� �Ķ���Ͱ� ����
        playerAnimator.SetFloat("MoveX", playerInput.moveHorizontal);
        playerAnimator.SetFloat("MoveY", playerInput.moveVertical);
        playerAnimator.SetBool("isJumping", playerInput.jump);
        playerAnimator.SetBool("isGrounded", isGrounded);
        
        CheckGroundStatus();

    }

    // �Է°��� ���� ĳ���� �̵�   
    private void Move()
    {
        // �Է°��� ���� �̵��� ���� ���
        Vector3 moveDirection = new Vector3(playerInput.moveHorizontal, 0, playerInput.moveVertical).normalized;

        // �̵��� �Ÿ� ���
        Vector3 moveDistance = moveDirection * moveSpeed * Time.deltaTime;

        //������ �ٵ� �̿��� ���� ������Ʈ ��ġ ����
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

            //forceMode.Impulse - ��������,�������� ��, ������� forece�� ����
            playerRigidbody.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse); 
            
        }
    }

    // �������� Ȯ��
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


    // �Է°��� ���� ĳ���͸� ȸ��
    private void Rotate()
    {
        //���콺 ��ġ�� ��������
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            // �÷��̾ ���콺�� �ٶ󺸵��� ȸ��
            Vector3 lookDirection = hit.point - transform.position;
            lookDirection.y = 0; // Y�� ȸ���� ������� ����

            Quaternion newRotation = Quaternion.LookRotation(lookDirection);
            playerRigidbody.MoveRotation(newRotation);
        }

    }
}