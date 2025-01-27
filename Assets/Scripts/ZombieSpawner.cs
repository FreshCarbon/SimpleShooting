using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 적 게임 오브젝트를 주기적으로 생성
public class ZombieSpawner : MonoBehaviour
{
    public Zombie zombiePrefab; // 생성할 좀비 원본 프리펩

    public ZombieDatas[] zombieDatas; //사용할 좀비 셋업 데이터
    public Transform[] spawnPoints; // 적 AI를 소환할 위치들

    private List<Zombie> zombies = new List<Zombie>(); // 생성된 적들을 담는 리스트
    private int wave; // 현재 웨이브

    private void Update()
    {
        // 게임 오버 상태일때는 생성하지 않음
        if (GameManager.instance != null && GameManager.instance.isGameover)
        {
            return;
        }

        // 적을 모두 물리친 경우 다음 스폰 실행
        if (zombies.Count <= 0)
        {
            SpawnWave();
        }

        // UI 갱신
        UpdateUI();
    }

    // 웨이브 정보를 UI로 표시
    private void UpdateUI()
    {
        //현재 웨이브와 남은 적의 수 표시
        UIManager.instance.UpdateWaveText(wave, zombies.Count);
    }

    // 현재 웨이브에 맞춰 적을 생성
    private void SpawnWave()
    {
        // 웨이브 1 증가
        wave++;

        // 현재 웨이브 * 1.5에 반올림 한 개수 만큼 적을 생성
        int spawnCount = Mathf.RoundToInt(wave * 1.5f);

        // spawnCount 만큼 적을 생성
        for (int i = 0; i < spawnCount; i++)
        {
            CreateZombie();

        }
    }

    // 적을 생성하고 생성한 적에게 추적할 대상을 할당
    private void CreateZombie()
    {
        //사용할 좀비 데이터를 랜덤으로 결정
        ZombieDatas zombieData = zombieDatas[Random.Range(0, zombieDatas.Length)];

        //생성할 위치를 랜덤으로 결정
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        //좀비 프리팹으로 좀비 생성
        Zombie zombie = Instantiate(zombiePrefab, spawnPoint.position, spawnPoint.rotation);

        //생성한 좀비의 능력치 결정
        zombie.Setup(zombieData);

        //생성된 좀비를 리스트에 추가
        zombies.Add(zombie);

        //좀비의 onDeath 이벤트에 익명 메서드 등록
        // 사망한 좀비를 리스트에서 제거
        zombie.onDeath += () => zombies.Remove(zombie);

        //사망한 좀비를 10초 뒤 파괴
        zombie.onDeath += () => Destroy(zombie.gameObject, 10f);

        //좀비 사망 시 점수 상승
        zombie.onDeath += () => GameManager.instance.AddScore(100);
    }
}