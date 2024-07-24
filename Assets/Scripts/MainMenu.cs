using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioClip clickSound;
    public AudioClip hoverSound;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void StartGame()
    {
        audioSource.PlayOneShot(clickSound);
        SceneManager.LoadScene("Game");
    }

    public void QuitGame()
    {
        audioSource.PlayOneShot(clickSound);

        // ����Ƽ ������ ���� ����, ����� ����
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void PlayHoverSound()
    {
        audioSource.PlayOneShot(hoverSound);
    }
}
