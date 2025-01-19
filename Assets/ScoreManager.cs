using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SendScore(int score)
    {
        StartCoroutine(SendScoreCoroutine(score));
    }

    private IEnumerator SendScoreCoroutine(int score)
    {
        if (!AuthManager.Instance.IsAuthenticated)
        {
            Debug.LogWarning("Utilisateur non authentifié");
            yield break;
        }

        yield return StartCoroutine(APIManager.Instance.SendScore(score, (success, error) => {
            if (success)
            {
                Debug.Log("Score envoyé avec succès !");
            }
            else
            {
                Debug.LogError($"Erreur lors de l'envoi du score : {error}");
            }
        }));
    }
} 