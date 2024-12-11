using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public GameObject Sound;
    public GameObject turnOffButton;
    public GameObject turnOnButton;
    public void turnOff()
    {
        Sound.SetActive(false);
        turnOffButton.SetActive(false);
        turnOnButton.SetActive(true);
    }
    public void turnOn()
    {
        Sound.SetActive(true);
        turnOffButton.SetActive(true);
        turnOnButton.SetActive(false);
    }
}
