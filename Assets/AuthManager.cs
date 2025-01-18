using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Text;

[Serializable]
public class AuthResponse
{
    public int id;
    public string username;
    public string email;
    public string token;
}

public class AuthManager : MonoBehaviour
{
    public const string API_URL = "http://localhost:8000/";
    private const string PLAYER_TOKEN_KEY = "PlayerToken";
    private const string PLAYER_USERNAME_KEY = "PlayerUsername";
    
    public static AuthManager Instance { get; private set; }
    public bool IsAuthenticated => !string.IsNullOrEmpty(PlayerToken);
    public string PlayerToken { get; private set; }
    public string PlayerUsername { get; private set; }
    public bool IsSkipMode { get; private set; } = false;
    
    public event Action<bool> OnAuthenticationChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSavedAuth();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadSavedAuth()
    {
        PlayerToken = PlayerPrefs.GetString(PLAYER_TOKEN_KEY, "");
        PlayerUsername = PlayerPrefs.GetString(PLAYER_USERNAME_KEY, "");
        OnAuthenticationChanged?.Invoke(IsAuthenticated);
    }

    public void SaveAuth(string token, string username)
    {
        PlayerToken = token;
        PlayerUsername = username;
        PlayerPrefs.SetString(PLAYER_TOKEN_KEY, token);
        PlayerPrefs.SetString(PLAYER_USERNAME_KEY, username);
        PlayerPrefs.Save();
        OnAuthenticationChanged?.Invoke(true);
    }

    public void Logout()
    {
        PlayerToken = "";
        PlayerUsername = "";
        PlayerPrefs.DeleteKey(PLAYER_TOKEN_KEY);
        PlayerPrefs.DeleteKey(PLAYER_USERNAME_KEY);
        PlayerPrefs.Save();
        OnAuthenticationChanged?.Invoke(false);
    }

    public void Register(string username, string email, string password, Action<bool, string> callback)
    {
        StartCoroutine(RegisterCoroutine(username, email, password, callback));
    }

    public void Login(string username, string password, Action<bool, string> callback)
    {
        StartCoroutine(LoginCoroutine(username, password, callback));
    }

    private IEnumerator RegisterCoroutine(string username, string email, string password, Action<bool, string> callback)
    {
        string jsonData = $"{{\"username\": \"{username}\", \"email\": \"{email}\", \"password\": \"{password}\"}}";
        using (UnityWebRequest www = new UnityWebRequest(API_URL + "api/users/", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AuthResponse response = JsonUtility.FromJson<AuthResponse>(www.downloadHandler.text);
                SaveAuth(response.token, response.username);
                callback(true, "");
            }
            else
            {
                string errorMessage = "Erreur d'inscription. Vérifiez vos informations.";
                Debug.LogWarning($"Erreur d'inscription : {www.error}");
                callback(false, errorMessage);
            }
        }
    }

    private IEnumerator LoginCoroutine(string username, string password, Action<bool, string> callback)
    {
        using (UnityWebRequest www = new UnityWebRequest(API_URL + "api/token-auth/", "POST"))
        {
            string jsonData = $"{{\"username\": \"{username}\", \"password\": \"{password}\"}}";
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AuthResponse response = JsonUtility.FromJson<AuthResponse>(www.downloadHandler.text);
                SaveAuth(response.token, username);
                callback(true, "");
            }
            else
            {
                string errorMessage = "Identifiants incorrects";
                if (www.responseCode == 400)
                {
                    errorMessage = "Nom d'utilisateur ou mot de passe incorrect";
                }
                Debug.LogError($"Erreur de connexion : {www.error}");
                callback(false, errorMessage);
            }
        }
    }

    public void SkipAuth()
    {
        IsSkipMode = true;
        PlayerUsername = "Joueur Anonyme";
        OnAuthenticationChanged?.Invoke(true);
    }
} 