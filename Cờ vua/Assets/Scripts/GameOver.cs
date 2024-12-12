using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public void LoadExit()
    {
        Application.Quit();
    }

    public void LoadRestart3mins()
    {
        SceneManager.LoadScene("Game3minsMode");
    }
    public void LoadRestart15mins()
    {
        SceneManager.LoadScene("Game15minsMode");
    }
    public void LoadRestart90mins()
    {
        SceneManager.LoadScene("Game90minsMode");
    }
    public void LoadRestartCountless()
    {
        SceneManager.LoadScene("GameCountless");
    }
    public void LoadRestartPvECountless()
    {
        SceneManager.LoadScene("GamePvEMode");
    }
    public void LoadRestartPvECountlessMed()
    {
        SceneManager.LoadScene("GamePvEModeMed");
    }
}
