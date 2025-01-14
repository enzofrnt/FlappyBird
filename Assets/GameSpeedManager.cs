using UnityEngine;

public class GameSpeedManager : MonoBehaviour
{
    public static GameSpeedManager Instance; // Singleton pour un accès facile
    public float speed = 1f; // Vitesse initiale
    public float speedIncrement = 0.1f; // Incrémentation de la vitesse par seconde

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
        // Augmenter progressivement la vitesse au fil du temps
        speed += speedIncrement * Time.deltaTime;
    }
}