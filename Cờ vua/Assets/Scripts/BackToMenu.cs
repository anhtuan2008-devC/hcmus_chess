using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenu : MonoBehaviour
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
    public void LoadBack()
    {
        audioManager.PlaySFX(audioManager.selectClip);
        Invoke("Back", 0.5f);
    }
    public void Back()
    {
        SceneManager.LoadScene("Menu");
    }
}
