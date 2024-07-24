using UnityEngine;

// �÷��̾� ĳ���͸� �����ϱ� ���� ����� �Է��� ����
// ������ �Է°��� �ٸ� ������Ʈ���� ����� �� �ֵ��� ����
public class PlayerInput : MonoBehaviour
{
    //public string rotateAxisName = "Horizontal"; // �¿� ȸ���� ���� �Է��� �̸�

    public string moveAxisName = "Vertical"; // �������� ���� �Է��� �̸�
    public string moveHorizontalName = "Horizontal";
    public string jumpKeyName = "Jump";

    public string fireButtonName = "Fire1"; // �߻縦 ���� �Է� ��ư �̸�
    public string reloadButtonName = "Reload"; // �������� ���� �Է� ��ư �̸�

    public Gun gun; // ����� ��    

    // �� �Ҵ��� ���ο����� ����
    public float moveVertical { get; private set; } // ������ ������ �Է°�
    public float moveHorizontal { get; private set; }
    public bool jump { get; private set; }

    public float rotate { get; private set; } // ������ ȸ�� �Է°�
    public bool fire { get; private set; } // ������ �߻� �Է°�
    public bool reload { get; private set; } // ������ ������ �Է°�

    // �������� ����� �Է��� ����
    private void Update()
    {
        // ���ӿ��� ���¿����� ����� �Է��� �������� �ʴ´�  
        if (GameManager.instance != null && GameManager.instance.isGameover)
        {
            moveVertical = 0;
            moveHorizontal = 0;
            rotate = 0;
            fire = false;
            reload = false;
            jump = false;
            return;
        }

        // move�� ���� �Է� ����
        moveVertical = Input.GetAxis(moveAxisName);
        moveHorizontal = Input.GetAxis(moveHorizontalName);
        jump = Input.GetKeyDown("space");

        if(jump)
        {
            Debug.Log("JumpKey Pressed");
        }
        // fire �Է� ����
        fire = Input.GetButton(fireButtonName);

        // reload �Է� ����
        reload = Input.GetButtonDown(reloadButtonName);
    }
}