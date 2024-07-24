using System.Collections;
using UnityEngine;
using UnityEngine.UI; // UI 관련 코드

// 플레이어 캐릭터의 생명체로서의 동작을 담당
public class PlayerHealth : LivingEntity
{
    //public Slider healthSlider; // 체력을 표시할 UI 슬라이더
    public Text playerHealth;

    public AudioClip deathClip; // 사망 소리
    public AudioClip hitClip; // 피격 소리

    private AudioSource playerAudioPlayer; // 플레이어 소리 재생기
    private Animator playerAnimator; // 플레이어의 애니메이터

    private PlayerMovement playerMovement; // 플레이어 움직임 컴포넌트
    private PlayerShooter playerShooter; // 플레이어 슈터 컴포넌트

    private void Awake()
    {
        // 사용할 컴포넌트를 가져오기
        playerAnimator = GetComponent<Animator>();
        playerAudioPlayer = GetComponent<AudioSource>();

        playerMovement = GetComponent<PlayerMovement>();
        playerShooter = GetComponent<PlayerShooter>();

        // 디버그 로그 추가
        Debug.Assert(playerHealth != null, "playerHealth가 할당되지 않았습니다.");
        Debug.Assert(playerAudioPlayer != null, "AudioSource가 할당되지 않았습니다.");
        Debug.Assert(playerAnimator != null, "Animator가 할당되지 않았습니다.");
        Debug.Assert(playerMovement != null, "PlayerMovement가 할당되지 않았습니다.");
        Debug.Assert(playerShooter != null, "PlayerShooter가 할당되지 않았습니다.");
    }

    protected override void OnEnable()
    {
        // LivingEntity의 OnEnable() 실행 (상태 초기화)
        base.OnEnable();

        playerHealth.text = "HP: " + health;

        //플레이어 조작을 받는 컴포넌트 활성화
        playerMovement.enabled = true;
        playerShooter.enabled = true;

        StartCoroutine(RestoreHealthOverTime());

    }
    // 체력 회복 코루틴
    private IEnumerator RestoreHealthOverTime()
    {
        while (!dead)
        {
            RestoreHealth(3);
            //playerHealth.text = "HP: " + health; // UI 업데이트
            yield return new WaitForSeconds(2f);
        }
    }

    // 체력 회복
    public override void RestoreHealth(float newHealth)
    {
        // LivingEntity의 RestoreHealth() 실행 (체력 증가)
        base.RestoreHealth(newHealth);

        //체력 100미만 제한
        if(health > 100f)
        {
            health = 100f;
        }

        playerHealth.text = "HP: " + health;
    }

    // 데미지 처리
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (!dead)
        {
            
            //사망하지 않은 경우 효과음 발생
            playerAudioPlayer.PlayOneShot(hitClip);

            // LivingEntity의 OnDamage() 실행(데미지 적용)    
            base.OnDamage(damage, hitPoint, hitDirection);
            playerHealth.text = "HP:" + health;

            
        }
   

    }

    // 사망 처리
    public override void Die()
    {
        // LivingEntity의 Die() 실행(사망 적용)
        base.Die();
    
        //사망음 재생
        playerAudioPlayer.PlayOneShot(deathClip);
        //애니메이터의 Die 트리거를 발동시켜 사망 애니메이션 재생
        playerAnimator.SetTrigger("Die");
        //플레이어 조작을 받는 컴포넌트 비활성화
        playerMovement.enabled = false;
        playerShooter.enabled = false;
    }

    
}