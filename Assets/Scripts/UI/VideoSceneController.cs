using UnityEngine;

/// <summary>
/// Controlador para la escena Video
/// Maneja la navegación desde la escena Video hacia otras escenas
/// </summary>
public class VideoSceneController : MonoBehaviour
{
    // Referencia al GameSceneManager
    public GameSceneManager gameSceneManager;

    void Start()
    {
        // Si no se asignó en el Inspector, intentar encontrarlo en la escena
        if (gameSceneManager == null)
        {
            gameSceneManager = FindObjectOfType<GameSceneManager>();
            
            if (gameSceneManager == null)
            {
                Debug.LogError("No se encontró GameSceneManager en la escena Video. Asegúrate de agregarlo al GameObject.");
            }
        }
    }

    /// <summary>
    /// Método para regresar al menú principal desde la escena Video
    /// Este método debe ser llamado por el botón "Exit"
    /// </summary>
    public void ExitToMainMenu()
    {
        if (gameSceneManager != null)
        {
            Debug.Log("Regresando al MainMenu desde Video...");
            gameSceneManager.LoadMainMenu();
        }
        else
        {
            Debug.LogError("No se puede regresar al MainMenu: GameSceneManager no está asignado.");
        }
    }

    /// <summary>
    /// Método alternativo para cargar cualquier escena desde Video
    /// </summary>
    public void LoadScene(string sceneName)
    {
        if (gameSceneManager != null)
        {
            gameSceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"No se puede cargar la escena {sceneName}: GameSceneManager no está asignado.");
        }
    }
}
