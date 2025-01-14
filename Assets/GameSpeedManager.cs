using UnityEngine;

public class GameSpeedManager : MonoBehaviour
{
    public static GameSpeedManager Instance;
    public float speed = 1f;
    public float speedIncrement = 0.1f;

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

    private void Update()
    {
        if (!GameStateManager.Instance.IsGameplayActive)
            return;

        speed += speedIncrement * Time.deltaTime;
    }

    public void ResetSpeed()
    {
        speed = 1f;
    }
}