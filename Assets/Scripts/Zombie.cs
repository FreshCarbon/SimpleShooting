using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
 *LivingEntity 기본 생명체 기능, 외부에서 좀비 초기 능력치 셋업 기능
 *주기적으로 목표 위치를 찾아 경로 갱신
 *공격 받으면 피탄효과 재생
 *트리거 콜라이더로 감지된 대상 공격
 *사망 시 추적 중단
 *사망 시 사망효과 재생
 */
public class Zombie : LivingEntity
{
    public LayerMask whatIsTarget; //추적 대상 레이어

    private LivingEntity targetEntity; //추적 대상 - 생명체만 대상으로 하기 위함
    private NavMeshAgent navMeshAgent; //경로 계산 AI에이전트

    public ParticleSystem hitEffect; //피격 이펙트 파티클
    public AudioClip deathSound; //사망 시 오디오
    public AudioClip hitSound; //피격 시 오디오

    private Animator zombieAnimator; //에니메이터 컴포넌트
    private AudioSource zombieAudioPlayer; //오디오 소스 컴포넌트
    private Renderer zombieRenderer; //렌더러 컴포넌트

    public float damage; //좀비 공격력
    public float timeBetAttack = 0.5f; // 공격 간격
    private float lastAttackTime; //마지막 공격 시점

    private bool hasTarget
    {
        get
        {
            //추적 할 대상이 존재하고 대상이 사망하지 않았다면 true
            if (targetEntity != null && !targetEntity.dead)
            {
                return true;
            }
            return false;
        }
    }

    private void Awake()
    {
        //초기화
        //게임 오브젝트로부터 사용할 컴포넌트 가져오기
        navMeshAgent = GetComponent<NavMeshAgent>();
        zombieAnimator = GetComponent<Animator>();
        zombieAudioPlayer = GetComponent<AudioSource>();

        //렌더러 컴포넌트는 자식 게임 오브젝트에 있으므로 GetComponetInChildren 사용
        zombieRenderer = GetComponentInChildren<Renderer>();

    }

    //좀비 AI의 초기 스펙을 결정하는 셋업 메서드
    public void Setup(ZombieDatas zombieDatas)
    {
        //체력 설정
        startingHealth = zombieDatas.health;
        health = zombieDatas.health;

        //공격력 설정
        damage = zombieDatas.damage;

        //내비메시 에이전트의 이동속도 설정
        navMeshAgent.speed = zombieDatas.speed;
        //랜더러가 사용중인 머터리얼의 컬러를 변경,외형 색이 변함
        zombieRenderer.material.color = zombieDatas.skinColor;
    }

    private void Start()
    {
        //게임 오브젝트 활성화와 동시에 AI의 추적 루틴 시작
        StartCoroutine(UpdatePath());
    }
    // Update is called once per frame
    void Update()
    {
        //추적 대상의 존재 여부에 따라 다른 애니메이션 재생
        zombieAnimator.SetBool("HasTarget", hasTarget);
    }

    //주기적으로 추적할 대상의 위치를 찾아 경로 갱신
    private IEnumerator UpdatePath()
    {
        Debug.Log($"hasTarget: {hasTarget}");
        Debug.Log($"targetEntity: {targetEntity}");

        //살아 있으면 무한루프
        while (!dead)
        {
            //추적 대상이 존재한다면 경로를 갱신하고 AI 이동을 진행
            if (hasTarget)
            {
                //Debug.Log("추적대상 존재");
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(targetEntity.transform.position);
            }
            else
            {
                //추적 대상이 없다면 AI 이동 중지
                //Debug.Log("추적대상 없음");   
                navMeshAgent.isStopped = true;
            }

            //20 유닛의 반지름을 가진 가상의 구를 그렸을 때 구와 겹치는 모든 콜라이더 가져옴
            //단 whatIsTarget 레이어를 가진 콜라이더만 가져오도록 필터링
            //overlapsphere(vector3, float 반지름radious ,(비필수)조건)
            Collider[] colliders = Physics.OverlapSphere(transform.position, 1000f, whatIsTarget);

            //모든 콜라이더를 순회하면서 살아 있는 LivingEntitiy 찾기
            for (int i = 0; i < colliders.Length; i++)
            {
                //콜라이더로부터 LIvingEntitiy 가져오기
                LivingEntity livingEntity = colliders[i].GetComponent<LivingEntity>();
                //Debug.Log($"Collider {i}: {livingEntity}");


                //LivingEntity 컴포넌트가 존재하며 해당 LivingEntity가 살아 있다면
                if (livingEntity != null && !livingEntity.dead)
                {
                    //추적 대상을 해당 LivingEntity로 설정하고 종료 
                    targetEntity = livingEntity;
                    break;
                }
            }

            // 0.25초 주기로 처리 반복
            yield return new WaitForSeconds(0.25f);
        }
    }
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        //아직 사망하지 않은 경우에만 피격효과 재생
        if (!dead)
        {
            //공격받은 지점과 방향으로 파티클 효과 재생
            hitEffect.transform.position = hitPoint;
            hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
            hitEffect.Play();

            //피격 효과음 재생
            zombieAudioPlayer.PlayOneShot(hitSound);
            
            //LivingEntity 의 Ondamage()를 실행하여 대미지 적용
            base.OnDamage(damage, hitPoint, hitNormal);
        }


    }

    //사망처리
    public override void Die()
    {
        //LivingEntitiy의 Die() 메서드 실행 
        base.Die();

        //좀비 컴포넌트에 2개 이상의 콜라이더를 사용해서 GetComponents 사용
        //다른 AI를 방해하지 않도록 자신의 모든 콜라이더를 비활성화 - 시체에 남은 콜라이더가 방해 가능성 야기
        Collider[] zombieColliders = GetComponents<Collider>();
        for (int i = 0; i < zombieColliders.Length; i++)
        {
            zombieColliders[i].enabled = false;

        }

        //AI 추적을 중지하고 내비메시 컴포넌트 비활성화 - 움직임, 추적 비활성화
        navMeshAgent.isStopped = true;
        navMeshAgent.enabled = false; //다른 네비메시 에이전트 방해 안하도록

        //사망 애니메이션 재생
        zombieAnimator.SetTrigger("Die");
        zombieAudioPlayer.PlayOneShot(deathSound);
    }

    //트리거 설정한 콜라이더끼리 겹치는 동안 계속 실행됨 - 물리 주기 0.02초
    //트리거 충돌한 상대방 오브젝트가 추적 대상이라면 공격 실행
    private void OnTriggerStay(Collider other)
    {
        //자신이 사망하지 않았으며 최근 공격 시점에서 timeBetAttack 이상 시간이 지났다면 공격 가능
        if (!dead && Time.time >= lastAttackTime + timeBetAttack)
        {
            //상대방의 Living Entity 타입 가져오기
            LivingEntity attackTarget = other.GetComponent<LivingEntity>();

            //상대방의 LivingEntity가 추적 대상(사람)이라면 공격 실행
            if (attackTarget != null && attackTarget == targetEntity)
            {
                //공격 시간 갱신
                lastAttackTime = Time.time;

                // 상대방의 피격 위치와 피격 방향을 근삿값으로 계산
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 hitNomal = transform.position - other.transform.position;

                //공격실행
                attackTarget.OnDamage(damage, hitPoint, hitNomal);
            }
        }


    }



}
