using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
 *LivingEntity �⺻ ����ü ���, �ܺο��� ���� �ʱ� �ɷ�ġ �¾� ���
 *�ֱ������� ��ǥ ��ġ�� ã�� ��� ����
 *���� ������ ��źȿ�� ���
 *Ʈ���� �ݶ��̴��� ������ ��� ����
 *��� �� ���� �ߴ�
 *��� �� ���ȿ�� ���
 */
public class Zombie : LivingEntity
{
    public LayerMask whatIsTarget; //���� ��� ���̾�

    private LivingEntity targetEntity; //���� ��� - ����ü�� ������� �ϱ� ����
    private NavMeshAgent navMeshAgent; //��� ��� AI������Ʈ

    public ParticleSystem hitEffect; //�ǰ� ����Ʈ ��ƼŬ
    public AudioClip deathSound; //��� �� �����
    public AudioClip hitSound; //�ǰ� �� �����

    private Animator zombieAnimator; //���ϸ����� ������Ʈ
    private AudioSource zombieAudioPlayer; //����� �ҽ� ������Ʈ
    private Renderer zombieRenderer; //������ ������Ʈ

    public float damage; //���� ���ݷ�
    public float timeBetAttack = 0.5f; // ���� ����
    private float lastAttackTime; //������ ���� ����

    private bool hasTarget
    {
        get
        {
            //���� �� ����� �����ϰ� ����� ������� �ʾҴٸ� true
            if (targetEntity != null && !targetEntity.dead)
            {
                return true;
            }
            return false;
        }
    }

    private void Awake()
    {
        //�ʱ�ȭ
        //���� ������Ʈ�κ��� ����� ������Ʈ ��������
        navMeshAgent = GetComponent<NavMeshAgent>();
        zombieAnimator = GetComponent<Animator>();
        zombieAudioPlayer = GetComponent<AudioSource>();

        //������ ������Ʈ�� �ڽ� ���� ������Ʈ�� �����Ƿ� GetComponetInChildren ���
        zombieRenderer = GetComponentInChildren<Renderer>();

    }

    //���� AI�� �ʱ� ������ �����ϴ� �¾� �޼���
    public void Setup(ZombieDatas zombieDatas)
    {
        //ü�� ����
        startingHealth = zombieDatas.health;
        health = zombieDatas.health;

        //���ݷ� ����
        damage = zombieDatas.damage;

        //����޽� ������Ʈ�� �̵��ӵ� ����
        navMeshAgent.speed = zombieDatas.speed;
        //�������� ������� ���͸����� �÷��� ����,���� ���� ����
        zombieRenderer.material.color = zombieDatas.skinColor;
    }

    private void Start()
    {
        //���� ������Ʈ Ȱ��ȭ�� ���ÿ� AI�� ���� ��ƾ ����
        StartCoroutine(UpdatePath());
    }
    // Update is called once per frame
    void Update()
    {
        //���� ����� ���� ���ο� ���� �ٸ� �ִϸ��̼� ���
        zombieAnimator.SetBool("HasTarget", hasTarget);
    }

    //�ֱ������� ������ ����� ��ġ�� ã�� ��� ����
    private IEnumerator UpdatePath()
    {
        Debug.Log($"hasTarget: {hasTarget}");
        Debug.Log($"targetEntity: {targetEntity}");

        //��� ������ ���ѷ���
        while (!dead)
        {
            //���� ����� �����Ѵٸ� ��θ� �����ϰ� AI �̵��� ����
            if (hasTarget)
            {
                //Debug.Log("������� ����");
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(targetEntity.transform.position);
            }
            else
            {
                //���� ����� ���ٸ� AI �̵� ����
                //Debug.Log("������� ����");   
                navMeshAgent.isStopped = true;
            }

            //20 ������ �������� ���� ������ ���� �׷��� �� ���� ��ġ�� ��� �ݶ��̴� ������
            //�� whatIsTarget ���̾ ���� �ݶ��̴��� ���������� ���͸�
            //overlapsphere(vector3, float ������radious ,(���ʼ�)����)
            Collider[] colliders = Physics.OverlapSphere(transform.position, 1000f, whatIsTarget);

            //��� �ݶ��̴��� ��ȸ�ϸ鼭 ��� �ִ� LivingEntitiy ã��
            for (int i = 0; i < colliders.Length; i++)
            {
                //�ݶ��̴��κ��� LIvingEntitiy ��������
                LivingEntity livingEntity = colliders[i].GetComponent<LivingEntity>();
                //Debug.Log($"Collider {i}: {livingEntity}");


                //LivingEntity ������Ʈ�� �����ϸ� �ش� LivingEntity�� ��� �ִٸ�
                if (livingEntity != null && !livingEntity.dead)
                {
                    //���� ����� �ش� LivingEntity�� �����ϰ� ���� 
                    targetEntity = livingEntity;
                    break;
                }
            }

            // 0.25�� �ֱ�� ó�� �ݺ�
            yield return new WaitForSeconds(0.25f);
        }
    }
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        //���� ������� ���� ��쿡�� �ǰ�ȿ�� ���
        if (!dead)
        {
            //���ݹ��� ������ �������� ��ƼŬ ȿ�� ���
            hitEffect.transform.position = hitPoint;
            hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
            hitEffect.Play();

            //�ǰ� ȿ���� ���
            zombieAudioPlayer.PlayOneShot(hitSound);
            
            //LivingEntity �� Ondamage()�� �����Ͽ� ����� ����
            base.OnDamage(damage, hitPoint, hitNormal);
        }


    }

    //���ó��
    public override void Die()
    {
        //LivingEntitiy�� Die() �޼��� ���� 
        base.Die();

        //���� ������Ʈ�� 2�� �̻��� �ݶ��̴��� ����ؼ� GetComponents ���
        //�ٸ� AI�� �������� �ʵ��� �ڽ��� ��� �ݶ��̴��� ��Ȱ��ȭ - ��ü�� ���� �ݶ��̴��� ���� ���ɼ� �߱�
        Collider[] zombieColliders = GetComponents<Collider>();
        for (int i = 0; i < zombieColliders.Length; i++)
        {
            zombieColliders[i].enabled = false;

        }

        //AI ������ �����ϰ� ����޽� ������Ʈ ��Ȱ��ȭ - ������, ���� ��Ȱ��ȭ
        navMeshAgent.isStopped = true;
        navMeshAgent.enabled = false; //�ٸ� �׺�޽� ������Ʈ ���� ���ϵ���

        //��� �ִϸ��̼� ���
        zombieAnimator.SetTrigger("Die");
        zombieAudioPlayer.PlayOneShot(deathSound);
    }

    //Ʈ���� ������ �ݶ��̴����� ��ġ�� ���� ��� ����� - ���� �ֱ� 0.02��
    //Ʈ���� �浹�� ���� ������Ʈ�� ���� ����̶�� ���� ����
    private void OnTriggerStay(Collider other)
    {
        //�ڽ��� ������� �ʾ����� �ֱ� ���� �������� timeBetAttack �̻� �ð��� �����ٸ� ���� ����
        if (!dead && Time.time >= lastAttackTime + timeBetAttack)
        {
            //������ Living Entity Ÿ�� ��������
            LivingEntity attackTarget = other.GetComponent<LivingEntity>();

            //������ LivingEntity�� ���� ���(���)�̶�� ���� ����
            if (attackTarget != null && attackTarget == targetEntity)
            {
                //���� �ð� ����
                lastAttackTime = Time.time;

                // ������ �ǰ� ��ġ�� �ǰ� ������ �ٻ����� ���
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 hitNomal = transform.position - other.transform.position;

                //���ݽ���
                attackTarget.OnDamage(damage, hitPoint, hitNomal);
            }
        }


    }



}
