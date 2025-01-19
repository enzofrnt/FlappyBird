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
            
            // S'assurer que APIManager existe
            if (APIManager.Instance == null)
            {
                GameObject apiManagerObj = new GameObject("APIManager");
                apiManagerObj.AddComponent<APIManager>();
                DontDestroyOnLoad(apiManagerObj);
            }
            
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
        yield return StartCoroutine(APIManager.Instance.Register(username, email, password, (success, token, error) => {
            if (success)
            {
                IsSkipMode = false;
                SaveAuth(token, username);
            }
            callback(success, error);
        }));
    }

    private IEnumerator LoginCoroutine(string username, string password, Action<bool, string> callback)
    {
        yield return StartCoroutine(APIManager.Instance.Login(username, password, (success, token, error) => {
            if (success)
            {
                IsSkipMode = false;
                SaveAuth(token, username);
            }
            callback(success, error);
        }));
    }

    public void SkipAuth()
    {
        IsSkipMode = true;
        PlayerUsername = "Joueur Anonyme";
        OnAuthenticationChanged?.Invoke(true);
    }
} 