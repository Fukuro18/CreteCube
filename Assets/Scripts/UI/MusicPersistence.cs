using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPersistence : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        // Lista Blanca: Escenas donde la música PUEDE existir
        // 1. MainMenu: Donde empieza
        // 2. Cube: El juego (al dar Play)
        if (sceneName != "MainMenu" && sceneName != "Cube")
        {
            // Si estamos en cualquier otra escena (ej. Video, Settings), destruir la música
            Destroy(gameObject);
        }
    }
}
