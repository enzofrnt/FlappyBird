using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    public bool IsGameplayActive { get; private set; } = false;
    public bool IsGameOver { get; private set; } = false;
    public GameObject authCanvas;
    public GameObject gameCanvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GameOver()
    {
        IsGameOver = true;
    }

    public void RestartGame()
    {
        IsGameOver = false;
    }

    public void StartGameplay()
    {
        // Empêche de relancer le gameplay si c'est le game over
        if (IsGameOver)
            return;

        IsGameplayActive = true;
    }

    public void StopGameplay()
    {
        // Si on est en game over, on ne change pas IsGameplayActive ?
        // => C'est un choix, mais en général on arrête quand même le gameplay
        IsGameplayActive = false;
    }

    private void Start()
    {
        // On ne redirige vers l'auth que si on arrive directement sur la scène de jeu
        // sans être passé par l'écran de login
        if (!AuthManager.Instance.IsAuthenticated && SceneManager.GetActiveScene().name == "Game")
        {
            // Vérifier si c'est le premier chargement de la scène
            if (!AuthManager.Instance.IsSkipMode)
            {
                SceneLoader.Instance.LoadAuth();
                return;
            }
        }
    }

    public void ShowAuthCanvas()
    {
        authCanvas.SetActive(true);
        gameCanvas.SetActive(false);
    }

    public void ShowGameCanvas()
    {
        authCanvas.SetActive(false);
        gameCanvas.SetActive(true);
    }
}