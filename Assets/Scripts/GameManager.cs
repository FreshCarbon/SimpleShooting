using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
#endif



public class GameManager : MonoBehaviour
{
    private static GameManager m_instance; // �̱����� �Ҵ�� static ����
    // �̱��� ���ٿ� ������Ƽ
    public static GameManager instance
    {
        get
        {
            // ���� �̱��� ������ ���� ������Ʈ�� �Ҵ���� �ʾҴٸ�
            if (m_instance == null)
            {
                // ������ GameManager ������Ʈ�� ã�� �Ҵ�
                m_instance = FindObjectOfType<GameManager>();
            }

            // �̱��� ������Ʈ�� ��ȯ
            return m_instance;
        }
    }


    private int score = 0; // ���� ���� ����
    public bool isGameover { get; private set; } // ���� ���� ����, �ܺο��� �б⸸ ���� ����Ұ�

    private void Awake()
    {
        // ���� �̱��� ������Ʈ�� �� �ٸ� GameManager ������Ʈ�� �ִٸ�
        if (instance != this)
        {
            // �ڽ��� �ı�, ������Ʈ�� �ϳ��� �����ؾ� �ϱ� ����
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // �÷��̾� ĳ������ ��� �̺�Ʈ �߻��� ���� ����
        FindObjectOfType<PlayerHealth>().onDeath += EndGame;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            quitGame();
        }
    }


    private void quitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // ����� ���ӿ����� ���ø����̼� ����
        Application.Quit();
        #endif


        Application.Quit();
    }
    // ������ �߰��ϰ� UI ����
    public void AddScore(int newScore)
    {
        // ���� ������ �ƴ� ���¿����� ���� ���� ����
        if (!isGameover)
        {
            // ���� �߰�
            score += newScore;
            // ���� UI �ؽ�Ʈ ����
            UIManager.instance.UpdateScoreText(score);
        }
    }

    // ���� ���� ó��
    public void EndGame()
    {
        // ���� ���� ���¸� ������ ����
        isGameover = true;
        // ���� ���� UI�� Ȱ��ȭ
        UIManager.instance.SetActiveGameoverUI(true);
        
    }
}
