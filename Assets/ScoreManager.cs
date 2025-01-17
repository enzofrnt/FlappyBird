using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class ScoreManager : MonoBehaviour
{
    private const string API_URL = AuthManager.API_URL;


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
            Debug.LogError("Utilisateur non authentifié");
            yield break;
        }

        string jsonData = $"{{\"score\": {score}}}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest www = new UnityWebRequest(API_URL + "api/scores/", "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", $"Token {AuthManager.Instance.PlayerToken}");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Score envoyé avec succès !");
            }
            else
            {
                Debug.LogError($"Erreur lors de l'envoi du score : {www.error}");
            }
        }
    }
} 