using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private AudioManager audioManager;

    void Start()
    {
        // Tìm đối tượng AudioManager trong Scene
        audioManager = FindObjectOfType<AudioManager>();
        if (audioManager == null)
        {
            Debug.LogError("AudioManager không được tìm thấy trong Scene.");
        }
    }

    public void LoadGame()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.startGameClip);
        }
        Invoke("StartGame",0.7f);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("OptionPlayGame");
    }
    public void ExitGame()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.selectClip);
        }
        Application.Quit();
    }
    public void LoadAbout()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.selectClip);
        }
        Invoke("About", 0.7f);
    }
    public void About()
    {
        SceneManager.LoadScene("About");
    }

}
