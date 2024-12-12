using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionGamePlay : MonoBehaviour
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

    public void LoadPvP3mins()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.startGameClip);
        }
        Invoke("PvP3mins", 0.7f);
    }

    public void PvP3mins()
    {
        SceneManager.LoadScene("Game3minsMode");
    }
    public void LoadPvP15mins()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.startGameClip);
        }
        Invoke("PvP15mins", 0.7f);
    }

    public void PvP15mins()
    {
        SceneManager.LoadScene("Game15minsMode");
    }
    public void LoadPvP90mins()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.startGameClip);
        }
        Invoke("PvP90mins", 0.7f);
    }

    public void PvP90mins()
    {
        SceneManager.LoadScene("Game90minsMode");
    }
    public void LoadPvPCountless()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.startGameClip);
        }
        Invoke("PvPCountless", 0.7f);
    }

    public void PvPCountless()
    {
        SceneManager.LoadScene("GameCountless");
    }
    public void PvECountless()
    {
        SceneManager.LoadScene("GamePvEMode");
    }
    public void LoadPvECountless()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.startGameClip);
        }
        Invoke("PvECountless", 0.7f);
    }
    public void PvECountlessMed()
    {
        SceneManager.LoadScene("GamePvEModeMed");
    }
    public void LoadPvECountlessMed()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.startGameClip);
        }
        Invoke("PvECountlessMed", 0.7f);
    }
}
