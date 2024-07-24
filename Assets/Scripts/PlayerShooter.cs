using UnityEngine;

// 주어진 Gun 오브젝트를 쏘거나 재장전
// 알맞은 애니메이션을 재생하고 IK를 사용해 캐릭터 양손이 총에 위치하도록 조정
public class PlayerShooter : MonoBehaviour
{
    public Gun gun; // 사용할 총    

    private PlayerInput playerInput; // 플레이어의 입력

    private void Start()
    {
        // 사용할 컴포넌트들을 가져오기
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        // 슈터가 활성화될 때 총도 함께 활성화
        gun.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        // 슈터가 비활성화될 때 총도 함께 비활성화
        gun.gameObject.SetActive(false);
    }

    private void Update()
    {
        // 입력을 감지하고 총 발사하거나 재장전
        if (playerInput.fire)
        {
            //발사 입력 감지 시 총 발사
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

    // 탄약 UI 갱신
    private void UpdateUI()
    {
        if (gun != null && UIManager.instance != null)
        {
            // UI 매니저의 탄약 텍스트에 탄창의 탄약과 남은 전체 탄약을 표시
            UIManager.instance.UpdateAmmoText(gun.magAmmo);
        }
    }

}