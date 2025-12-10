using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public Slider volumeSlider;
    public Slider brightnessSlider;
    public AudioSource musicSource;

    private bool isInitializing = true;

    void Start()
    {
        // Cargar valores sin disparar eventos de guardado
        volumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("volume", 1f));
        brightnessSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("brightness", 1f));

        ApplyVolume();
        ApplyBrightness();

        volumeSlider.onValueChanged.AddListener(OnVolumeChange);
        brightnessSlider.onValueChanged.AddListener(OnBrightnessChange);

        isInitializing = false;
    }

    public void OnVolumeChange(float value)
    {
        if (isInitializing) return;
        ApplyVolume();
        PlayerPrefs.SetFloat("volume", value);
        PlayerPrefs.Save();
    }

    public void OnBrightnessChange(float value)
    {
        if (isInitializing) return;
        ApplyBrightness();
        PlayerPrefs.SetFloat("brightness", value);
        PlayerPrefs.Save();
    }

    void ApplyVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }

    void ApplyBrightness()
    {
        float brightness = brightnessSlider.value;
        
        // 1. RenderSettings para objetos 3D
        RenderSettings.ambientLight = Color.white * brightness;

        // 2. Overlay para UI (Simular brillo reduciendo opacidad de un panel negro)
        // Lógica duplicada localmente para garantizar Feedback en Tiempo Real en el menú de ajustes
        GameObject overlayObj = GameObject.Find("BrightnessOverlayCanvas");
        if (overlayObj == null)
        {
            overlayObj = new GameObject("BrightnessOverlayCanvas");
            Canvas canvas = overlayObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 32767; // Encima de todo
            DontDestroyOnLoad(overlayObj);

            overlayObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            overlayObj.AddComponent<UnityEngine.UI.GraphicRaycaster>().blockingObjects = UnityEngine.UI.GraphicRaycaster.BlockingObjects.None; 

            GameObject panelObj = new GameObject("DarknessPanel");
            panelObj.transform.SetParent(overlayObj.transform, false);
            UnityEngine.UI.Image img = panelObj.AddComponent<UnityEngine.UI.Image>();
            img.color = Color.black;
            img.raycastTarget = false; 

            RectTransform rect = img.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        Transform panel = overlayObj.transform.Find("DarknessPanel");
        if (panel != null)
        {
            UnityEngine.UI.Image overlayImage = panel.GetComponent<UnityEngine.UI.Image>();
            if (overlayImage != null)
            {
                float alpha = 1.0f - brightness; 
                alpha = Mathf.Clamp(alpha, 0f, 0.9f);
                overlayImage.color = new Color(0, 0, 0, alpha);
            }
        }
    }

    public void GoBack()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
