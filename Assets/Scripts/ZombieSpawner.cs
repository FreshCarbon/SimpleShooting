using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// �� ���� ������Ʈ�� �ֱ������� ����
public class ZombieSpawner : MonoBehaviour
{
    public Zombie zombiePrefab; // ������ ���� ���� ������

    public ZombieDatas[] zombieDatas; //����� ���� �¾� ������
    public Transform[] spawnPoints; // �� AI�� ��ȯ�� ��ġ��

    private List<Zombie> zombies = new List<Zombie>(); // ������ ������ ��� ����Ʈ
    private int wave; // ���� ���̺�

    private void Update()
    {
        // ���� ���� �����϶��� �������� ����
        if (GameManager.instance != null && GameManager.instance.isGameover)
        {
            return;
        }

        // ���� ��� ����ģ ��� ���� ���� ����
        if (zombies.Count <= 0)
        {
            SpawnWave();
        }

        // UI ����
        UpdateUI();
    }

    // ���̺� ������ UI�� ǥ��
    private void UpdateUI()
    {
        //���� ���̺�� ���� ���� �� ǥ��
        UIManager.instance.UpdateWaveText(wave, zombies.Count);
    }

    // ���� ���̺꿡 ���� ���� ����
    private void SpawnWave()
    {
        // ���̺� 1 ����
        wave++;

        // ���� ���̺� * 1.5�� �ݿø� �� ���� ��ŭ ���� ����
        int spawnCount = Mathf.RoundToInt(wave * 1.5f);

        // spawnCount ��ŭ ���� ����
        for (int i = 0; i < spawnCount; i++)
        {
            CreateZombie();

        }
    }

    // ���� �����ϰ� ������ ������ ������ ����� �Ҵ�
    private void CreateZombie()
    {
        //����� ���� �����͸� �������� ����
        ZombieDatas zombieData = zombieDatas[Random.Range(0, zombieDatas.Length)];

        //������ ��ġ�� �������� ����
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        //���� ���������� ���� ����
        Zombie zombie = Instantiate(zombiePrefab, spawnPoint.position, spawnPoint.rotation);

        //������ ������ �ɷ�ġ ����
        zombie.Setup(zombieData);

        //������ ���� ����Ʈ�� �߰�
        zombies.Add(zombie);

        //������ onDeath �̺�Ʈ�� �͸� �޼��� ���
        // ����� ���� ����Ʈ���� ����
        zombie.onDeath += () => zombies.Remove(zombie);

        //����� ���� 10�� �� �ı�
        zombie.onDeath += () => Destroy(zombie.gameObject, 10f);

        //���� ��� �� ���� ���
        zombie.onDeath += () => GameManager.instance.AddScore(100);
    }
}