using UnityEngine;
using UnityEngine.Android;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    public bool IsGameplayActive { get; private set; } = false;
    public bool IsGameOver { get; private set; } = false;

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
}