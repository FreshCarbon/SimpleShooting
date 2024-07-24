using System.Collections;
using UnityEngine;
using UnityEngine.UI; // UI ���� �ڵ�

// �÷��̾� ĳ������ ����ü�μ��� ������ ���
public class PlayerHealth : LivingEntity
{
    //public Slider healthSlider; // ü���� ǥ���� UI �����̴�
    public Text playerHealth;

    public AudioClip deathClip; // ��� �Ҹ�
    public AudioClip hitClip; // �ǰ� �Ҹ�

    private AudioSource playerAudioPlayer; // �÷��̾� �Ҹ� �����
    private Animator playerAnimator; // �÷��̾��� �ִϸ�����

    private PlayerMovement playerMovement; // �÷��̾� ������ ������Ʈ
    private PlayerShooter playerShooter; // �÷��̾� ���� ������Ʈ

    private void Awake()
    {
        // ����� ������Ʈ�� ��������
        playerAnimator = GetComponent<Animator>();
        playerAudioPlayer = GetComponent<AudioSource>();

        playerMovement = GetComponent<PlayerMovement>();
        playerShooter = GetComponent<PlayerShooter>();

        // ����� �α� �߰�
        Debug.Assert(playerHealth != null, "playerHealth�� �Ҵ���� �ʾҽ��ϴ�.");
        Debug.Assert(playerAudioPlayer != null, "AudioSource�� �Ҵ���� �ʾҽ��ϴ�.");
        Debug.Assert(playerAnimator != null, "Animator�� �Ҵ���� �ʾҽ��ϴ�.");
        Debug.Assert(playerMovement != null, "PlayerMovement�� �Ҵ���� �ʾҽ��ϴ�.");
        Debug.Assert(playerShooter != null, "PlayerShooter�� �Ҵ���� �ʾҽ��ϴ�.");
    }

    protected override void OnEnable()
    {
        // LivingEntity�� OnEnable() ���� (���� �ʱ�ȭ)
        base.OnEnable();

        playerHealth.text = "HP: " + health;

        //�÷��̾� ������ �޴� ������Ʈ Ȱ��ȭ
        playerMovement.enabled = true;
        playerShooter.enabled = true;

        StartCoroutine(RestoreHealthOverTime());

    }
    // ü�� ȸ�� �ڷ�ƾ
    private IEnumerator RestoreHealthOverTime()
    {
        while (!dead)
        {
            RestoreHealth(3);
            //playerHealth.text = "HP: " + health; // UI ������Ʈ
            yield return new WaitForSeconds(2f);
        }
    }

    // ü�� ȸ��
    public override void RestoreHealth(float newHealth)
    {
        // LivingEntity�� RestoreHealth() ���� (ü�� ����)
        base.RestoreHealth(newHealth);

        //ü�� 100�̸� ����
        if(health > 100f)
        {
            health = 100f;
        }

        playerHealth.text = "HP: " + health;
    }

    // ������ ó��
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (!dead)
        {
            
            //������� ���� ��� ȿ���� �߻�
            playerAudioPlayer.PlayOneShot(hitClip);

            // LivingEntity�� OnDamage() ����(������ ����)    
            base.OnDamage(damage, hitPoint, hitDirection);
            playerHealth.text = "HP:" + health;

            
        }
   

    }

    // ��� ó��
    public override void Die()
    {
        // LivingEntity�� Die() ����(��� ����)
        base.Die();
    
        //����� ���
        playerAudioPlayer.PlayOneShot(deathClip);
        //�ִϸ������� Die Ʈ���Ÿ� �ߵ����� ��� �ִϸ��̼� ���
        playerAnimator.SetTrigger("Die");
        //�÷��̾� ������ �޴� ������Ʈ ��Ȱ��ȭ
        playerMovement.enabled = false;
        playerShooter.enabled = false;
    }

    
}