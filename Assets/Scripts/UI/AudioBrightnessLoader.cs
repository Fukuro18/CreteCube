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

#if UNITY_EDITOR
        // Implementación de música desde Assets/Audio (Solo Editor)
        // Solo reproducir si estamos en el MainMenu
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu")
        {
            GameObject musicObj = GameObject.Find("BackgroundMusic");
            if (musicObj == null)
            {
                musicObj = new GameObject("BackgroundMusic");
                AudioSource audioSource = musicObj.AddComponent<AudioSource>();
                
                // Cargar archivo específico sin moverlo
                AudioClip clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/musica.wav");
                
                if (clip != null)
                {
                    audioSource.clip = clip;
                    audioSource.loop = true;
                    audioSource.volume = volume; 
                    audioSource.Play();
                    
                    // Agregar el script que controla la persistencia inteligente
                    // Este script se encargará de "DontDestroyOnLoad" y de destruir el objeto si sale de las escenas permitidas
                    musicObj.AddComponent<MusicPersistence>();
                }
                else
                {
                    Debug.LogWarning("No se encontró 'Assets/Audio/musica.wav'.");
                }
            }
        }
#endif

        // Aplicar brillo global (luz ambiental)
        UpdateBrightness(brightness);
    }

    public void UpdateBrightness(float brightness)
    {
        // Delegar toda la lógica al Manager centralizado para evitar colisiones
        BrightnessManager.SetBrightness(brightness);
    }
}
