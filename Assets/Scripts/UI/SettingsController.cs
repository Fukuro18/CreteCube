using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public Slider volumeSlider;
    public Slider brightnessSlider;

    void Start()
    {
        // Cargar valores guardados (si existen)
        volumeSlider.value = PlayerPrefs.GetFloat("volume", 1f);
        brightnessSlider.value = PlayerPrefs.GetFloat("brightness", 1f);

        ApplyVolume();
        ApplyBrightness();
    }

    // Estos métodos reciben el float que envía el slider automáticamente
    public void OnVolumeChange(float value)
    {
        volumeSlider.value = value;
        ApplyVolume();
        PlayerPrefs.SetFloat("volume", value);
    }

    public void OnBrightnessChange(float value)
    {
        brightnessSlider.value = value;
        ApplyBrightness();
        PlayerPrefs.SetFloat("brightness", value);
    }

    void ApplyVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }

    void ApplyBrightness()
    {
        // Ajuste sencillo: cambia luz ambiental. Puedes mejorar usando post-processing.
        RenderSettings.ambientLight = Color.white * brightnessSlider.value;
    }

    public void GoBack()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
