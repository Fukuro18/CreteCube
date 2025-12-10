using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public Slider volumeSlider;
    public Slider brightnessSlider;
    public AudioSource musicSource;

    void Start()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("volume", 1f);
        brightnessSlider.value = PlayerPrefs.GetFloat("brightness", 1f);

        ApplyVolume();
        ApplyBrightness();

        volumeSlider.onValueChanged.AddListener(OnVolumeChange);
        brightnessSlider.onValueChanged.AddListener(OnBrightnessChange);
    }

    public void OnVolumeChange(float value)
    {
        ApplyVolume();
        PlayerPrefs.SetFloat("volume", value);
    }

    public void OnBrightnessChange(float value)
    {
        ApplyBrightness();
        PlayerPrefs.SetFloat("brightness", value);
    }

    void ApplyVolume()
    {
        musicSource.volume = volumeSlider.value;
    }

    void ApplyBrightness()
    {
        RenderSettings.ambientLight = Color.white * brightnessSlider.value;
    }

    public void GoBack()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
