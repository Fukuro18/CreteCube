using UnityEngine;

public class AudioBrightnessLoader : MonoBehaviour
{
    void Start()
    {
        // Cargar valores guardados (si no existen, usa 1f por defecto)
        float volume = PlayerPrefs.GetFloat("volume", 1f);
        float brightness = PlayerPrefs.GetFloat("brightness", 1f);

        // Aplicar volumen global
        AudioListener.volume = volume;

        // Aplicar brillo global (luz ambiental simple)
        RenderSettings.ambientLight = Color.white * brightness;
    }
}
