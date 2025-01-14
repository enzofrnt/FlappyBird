using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    public bool IsGameplayActive { get; private set; } = false;

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

    public void StartGameplay()
    {
        IsGameplayActive = true;
    }

    public void StopGameplay()
    {
        IsGameplayActive = false;
    }
}
