using UnityEngine;

// �־��� Gun ������Ʈ�� ��ų� ������
// �˸��� �ִϸ��̼��� ����ϰ� IK�� ����� ĳ���� ����� �ѿ� ��ġ�ϵ��� ����
public class PlayerShooter : MonoBehaviour
{
    public Gun gun; // ����� ��    

    private PlayerInput playerInput; // �÷��̾��� �Է�

    private void Start()
    {
        // ����� ������Ʈ���� ��������
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        // ���Ͱ� Ȱ��ȭ�� �� �ѵ� �Բ� Ȱ��ȭ
        gun.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        // ���Ͱ� ��Ȱ��ȭ�� �� �ѵ� �Բ� ��Ȱ��ȭ
        gun.gameObject.SetActive(false);
    }

    private void Update()
    {
        // �Է��� �����ϰ� �� �߻��ϰų� ������
        if (playerInput.fire)
        {
            //�߻� �Է� ���� �� �� �߻�
            gun.Fire();
        }
        else if (playerInput.reload)
        {
            if (gun.Reload())
            {

            }
        }
        UpdateUI();
    }

    // ź�� UI ����
    private void UpdateUI()
    {
        if (gun != null && UIManager.instance != null)
        {
            // UI �Ŵ����� ź�� �ؽ�Ʈ�� źâ�� ź��� ���� ��ü ź���� ǥ��
            UIManager.instance.UpdateAmmoText(gun.magAmmo);
        }
    }

}