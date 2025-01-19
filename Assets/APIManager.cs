using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Text;

public class APIManager : MonoBehaviour
{
    private const string API_URL = "https://flappybird-score.enzo-frnt.fr/";
    public static APIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[APIManager] Initialized");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private UnityWebRequest CreateRequest(string endpoint, string method, string body = null)
    {
        string fullUrl = API_URL + endpoint;
        Debug.Log($"[APIManager] Creating {method} request to: {fullUrl}");
        
        UnityWebRequest request = new UnityWebRequest(fullUrl, method);
        request.downloadHandler = new DownloadHandlerBuffer();
        
        if (body != null)
        {
            Debug.Log($"[APIManager] Request body: {body}");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(body);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        }

        // Headers de base
        request.SetRequestHeader("Content-Type", "application/json");
        Debug.Log("[APIManager] Added Content-Type header: application/json");
        
        // Headers anti-cache
        request.SetRequestHeader("Cache-Control", "no-cache, no-store, must-revalidate, max-age=0");
        request.SetRequestHeader("Pragma", "no-cache");
        request.SetRequestHeader("Expires", "0");
        Debug.Log("[APIManager] Added cache prevention headers");
        
        // Header d'authentification si connecté
        if (AuthManager.Instance.IsAuthenticated)
        {
            request.SetRequestHeader("Authorization", $"Token {AuthManager.Instance.PlayerToken}");
            Debug.Log("[APIManager] Added Authorization header with token");
        }

        return request;
    }

    public IEnumerator Login(string username, string password, Action<bool, string, string> callback)
    {
        Debug.Log($"[APIManager] Attempting login for user: {username}");
        string jsonData = $"{{\"username\": \"{username}\", \"password\": \"{password}\"}}";
        
        using (UnityWebRequest www = CreateRequest("api/token-auth/", "POST", jsonData))
        {
            yield return www.SendWebRequest();
            LogResponse("Login", www);

            if (www.result == UnityWebRequest.Result.Success)
            {
                AuthResponse response = JsonUtility.FromJson<AuthResponse>(www.downloadHandler.text);
                Debug.Log("[APIManager] Login successful");
                callback(true, response.token, "");
            }
            else
            {
                string errorMessage = "Erreur de connexion. Vérifiez vos identifiants.";
                Debug.LogError($"[APIManager] Login failed: {www.error}");
                callback(false, "", errorMessage);
            }
        }
    }

    public IEnumerator Register(string username, string email, string password, Action<bool, string, string> callback)
    {
        string jsonData = $"{{\"username\": \"{username}\", \"email\": \"{email}\", \"password\": \"{password}\"}}";
        using (UnityWebRequest www = CreateRequest("api/users/", "POST", jsonData))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AuthResponse response = JsonUtility.FromJson<AuthResponse>(www.downloadHandler.text);
                callback(true, response.token, "");
            }
            else
            {
                string errorMessage = "Erreur d'inscription. Vérifiez vos informations.";
                Debug.LogWarning($"Erreur d'inscription : {www.error}");
                callback(false, "", errorMessage);
            }
        }
    }

    [System.Serializable]
    public class ScoreData
    {
        public int score;
    }

    public IEnumerator SendScore(int score, Action<bool, string> callback)
    {
        Debug.Log($"[APIManager] Sending score: {score}");
        ScoreData scoreData = new ScoreData { score = score };
        string jsonData = JsonUtility.ToJson(scoreData);
        Debug.Log($"[APIManager] JSON data: {jsonData}");  // Pour débugger
        
        using (UnityWebRequest www = CreateRequest("api/scores/", "POST", jsonData))
        {
            yield return www.SendWebRequest();
            LogResponse("SendScore", www);
            HandleResponse(www, callback);
        }
    }

    public IEnumerator GetScores(Action<string> callback)
    {
        Debug.Log("[APIManager] Fetching scores");
        using (UnityWebRequest www = CreateRequest("api/scores/", "GET"))
        {
            yield return www.SendWebRequest();
            LogResponse("GetScores", www);
            
            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"[APIManager] Scores retrieved successfully: {www.downloadHandler.text}");
                callback(www.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"[APIManager] Failed to get scores: {www.error}");
                callback(null);
            }
        }
    }

    private void LogResponse(string operation, UnityWebRequest www)
    {
        Debug.Log($"[APIManager] {operation} Response:");
        Debug.Log($"Status Code: {www.responseCode}");
        Debug.Log($"Response Headers: {www.GetResponseHeaders()}");
        Debug.Log($"Response Body: {www.downloadHandler?.text}");
        
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error: {www.error}");
            Debug.LogError($"Detailed error: {www.downloadHandler?.text}");
        }
    }

    private void HandleResponse(UnityWebRequest www, Action<bool, string> callback)
    {
        bool success = www.result == UnityWebRequest.Result.Success;
        Debug.Log($"[APIManager] Request completed. Success: {success}");
        callback(success, www.error);
    }
} 