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

        // Aplicar brillo global (luz ambiental)
        UpdateBrightness(brightness);
    }

    public void UpdateBrightness(float brightness)
    {
        // 1. RenderSettings para objetos 3D
        RenderSettings.ambientLight = Color.white * brightness;

        // 2. Overlay para UI (Simular brillo reduciendo opacidad de un panel negro)
        // Buscamos o creamos el canvas de overlay
        GameObject overlayObj = GameObject.Find("BrightnessOverlayCanvas");
        UnityEngine.UI.Image overlayImage = null;

        if (overlayObj == null)
        {
            overlayObj = new GameObject("BrightnessOverlayCanvas");
            Canvas canvas = overlayObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 32767; // Encima de todo
            DontDestroyOnLoad(overlayObj);

            overlayObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            overlayObj.AddComponent<UnityEngine.UI.GraphicRaycaster>().blockingObjects = UnityEngine.UI.GraphicRaycaster.BlockingObjects.None; // Importante para no bloquear clicks

            GameObject panelObj = new GameObject("DarknessPanel");
            panelObj.transform.SetParent(overlayObj.transform, false);
            overlayImage = panelObj.AddComponent<UnityEngine.UI.Image>();
            overlayImage.color = Color.black;
            overlayImage.raycastTarget = false; // No bloquear clicks

            RectTransform rect = overlayImage.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
        else
        {
            Transform panel = overlayObj.transform.Find("DarknessPanel");
            if (panel != null) overlayImage = panel.GetComponent<UnityEngine.UI.Image>();
        }

        // Ajustar opacidad: Brillo 1.0 -> Alpha 0.0 (Invisible). Brillo 0.0 -> Alpha 0.8 (Oscuro)
        if (overlayImage != null)
        {
            float alpha = 1.0f - brightness; 
            // Limitamos para que no sea negro total si bajan todo el brillo (max alpha 0.9)
            alpha = Mathf.Clamp(alpha, 0f, 0.9f);
            overlayImage.color = new Color(0, 0, 0, alpha);
        }
    }
}
