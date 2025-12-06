using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PlayButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Settings")]
    public string sceneToLoad = "Cube";

    [Header("Visuals")]
    public Color accentColor = new Color(1f, 0.647f, 0f); // Naranja #FFA500
    public Color buttonBackgroundColor = new Color(0f, 0f, 0f, 0.3f); // Negro semitransparente
    public Color primaryTextColor = Color.white;
    public float scaleSpeed = 8f;
    public float fadeSpeed = 10f;

    // Componentes
    private Button buttonComponent;
    private Image buttonImage;
    private TextMeshProUGUI buttonText;
    private Outline textOutline;

    // Estado
    private Vector3 originalScale;
    private bool isHovered;

    void Start()
    {
        // Obtener componentes
        buttonComponent = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        if (buttonImage == null)
        {
            Debug.LogError("PlayButton: Falta el componente Image.");
            return;
        }

        // Inicializar visuales
        InitializeVisuals();

        // Configurar listener del botón
        if (buttonComponent != null)
        {
            buttonComponent.onClick.AddListener(OnClick);
        }
    }

    private void InitializeVisuals()
    {
        // Guardar escala original
        originalScale = transform.localScale;

        // Configurar colores iniciales
        buttonImage.color = buttonBackgroundColor;
        if (buttonText != null)
        {
            buttonText.color = primaryTextColor;

            // Configurar Outline
            textOutline = buttonText.GetComponent<Outline>();
            if (textOutline == null)
            {
                textOutline = buttonText.gameObject.AddComponent<Outline>();
            }
            textOutline.effectColor = new Color(0f, 0f, 0f, 1f);
            textOutline.effectDistance = new Vector2(4f, -4f);
        }

        // Configurar colores del componente Button para que no interfieran demasiado
        if (buttonComponent != null)
        {
            var colors = buttonComponent.colors;
            colors.normalColor = buttonBackgroundColor;
            colors.highlightedColor = accentColor;
            colors.pressedColor = new Color(accentColor.r, accentColor.g, accentColor.b, 0.7f);
            colors.selectedColor = accentColor;
            colors.colorMultiplier = 1f;
            buttonComponent.colors = colors;
        }
    }

    void Update()
    {
        if (buttonImage == null) return;

        // Animación de escala
        Vector3 targetScale = isHovered ? originalScale * 1.05f : originalScale;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);

        // Animación de color de fondo
        buttonImage.color = Color.Lerp(buttonImage.color, isHovered ? accentColor : buttonBackgroundColor, Time.deltaTime * fadeSpeed);

        // Animación de texto y outline
        if (buttonText != null)
        {
            buttonText.color = Color.Lerp(buttonText.color, isHovered ? Color.white : primaryTextColor, Time.deltaTime * fadeSpeed);
            
            if (textOutline != null)
            {
                textOutline.effectColor = isHovered ? Color.black : new Color(0f, 0f, 0f, 0.8f);
            }
        }
    }

    // Lógica de carga de escena
    public void OnClick()
    {
        Debug.Log($"Botón PLAY presionado. Intentando cargar escena '{sceneToLoad}'...");
        
        GameSceneManager sceneManager = FindObjectOfType<GameSceneManager>();
        if (sceneManager != null)
        {
            sceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("No se encontró GameSceneManager en la escena. Asegúrate de que existe un objeto con este script.");
        }
    }

    // Eventos de puntero
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        buttonImage.color = new Color(accentColor.r, accentColor.g, accentColor.b, 0.7f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        buttonImage.color = isHovered ? accentColor : buttonBackgroundColor;
    }
}
