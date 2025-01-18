using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class AuthUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject loginPanel;
    public GameObject registerPanel;
    public GameObject loadingPanel;

    [Header("Login Fields")]
    public TMP_InputField loginUsername;
    public TMP_InputField loginPassword;
    public Button loginButton;
    public TextMeshProUGUI loginErrorText;

    [Header("Register Fields")]
    public TMP_InputField registerUsername;
    public TMP_InputField registerEmail;
    public TMP_InputField registerPassword;
    public Button registerButton;
    public TextMeshProUGUI registerErrorText;

    [Header("Skip Auth")]
    public Button skipAuthButton;

    private Coroutine currentErrorCoroutine = null;

    private void Start()
    {
        if (AuthManager.Instance.IsAuthenticated)
        {
            // Par exemple, charger la scène du jeu directement.
            SceneLoader.Instance.LoadGame();
            return;
        }

        skipAuthButton.onClick.AddListener(OnSkipAuthClick);
        ShowLoginPanel();

        // Masquer les messages d'erreur au démarrage
        loginErrorText.gameObject.SetActive(false);
        registerErrorText.gameObject.SetActive(false);
    }

    
    public void ShowLoginPanel()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
        loadingPanel.SetActive(false);
    }

    public void ShowRegisterPanel()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
        loadingPanel.SetActive(false);
    }

    public void OnLoginClick()
    {
        if (string.IsNullOrEmpty(loginUsername.text) || string.IsNullOrEmpty(loginPassword.text))
        {
            DisplayError(loginErrorText, "Veuillez remplir tous les champs");
            return;
        }

        loginButton.interactable = false;
        loadingPanel.SetActive(true);

        AuthManager.Instance.Login(
            loginUsername.text,
            loginPassword.text,
            HandleLoginResponse
        );
    }

    private void HandleLoginResponse(bool success, string errorMessage)
    {
        // S'assurer que ces opérations sont effectuées avant toute désactivation potentielle
        loadingPanel.SetActive(false);
        loginButton.interactable = true;
        
        if (success)
        {
            loginErrorText.gameObject.SetActive(false);
            SceneLoader.Instance.LoadGame();
        }
        else
        {
            DisplayError(loginErrorText, errorMessage);
            loginPassword.text = "";
            loginPassword.Select();
        }
    }

    
    private void DisplayError(TextMeshProUGUI errorText, string message)
    {
        if (currentErrorCoroutine != null)
        {
            // Stop toute précédente coroutine d’erreur 
            StopCoroutine(currentErrorCoroutine);
        }
        currentErrorCoroutine = StartCoroutine(ShowErrorTemporarily(errorText, message, 5f));
    }

    private IEnumerator ShowErrorTemporarily(TextMeshProUGUI errorText, string message, float duration = 5f)
    {
        if (errorText == null) yield break;

        // Affichage du message
        errorText.text = message;
        errorText.gameObject.SetActive(true);

        // Attendre X secondes
        yield return new WaitForSeconds(duration);

        // Si l’objet existe encore, on cache le message
        if (errorText != null)
        {
            errorText.gameObject.SetActive(false);
        }
    }

    private void HideLoginError()
    {
        if (loginErrorText != null)
        {
            loginErrorText.gameObject.SetActive(false);
        }
    }

    private void HideRegisterError()
    {
        if (registerErrorText != null)
        {
            registerErrorText.gameObject.SetActive(false);
        }
    }

    public void OnRegisterClick()
    {
        if (string.IsNullOrEmpty(registerUsername.text) || 
            string.IsNullOrEmpty(registerEmail.text) || 
            string.IsNullOrEmpty(registerPassword.text))
        {
            DisplayError(registerErrorText, "Veuillez remplir tous les champs");
            return;
        }

        registerButton.interactable = false;
        loadingPanel.SetActive(true);

        AuthManager.Instance.Register(
            registerUsername.text,
            registerEmail.text,
            registerPassword.text,
            HandleRegisterResponse
        );
    }

    private void HandleRegisterResponse(bool success, string errorMessage)
    {
        loadingPanel.SetActive(false);
        registerButton.interactable = true;
        
        if (success)
        {
            registerErrorText.gameObject.SetActive(false);
            SceneLoader.Instance.LoadGame();
        }
        else
        {
            DisplayError(registerErrorText, errorMessage);
            registerPassword.text = "";
            registerPassword.Select();
        }
    }

    public void OnSkipAuthClick()
    {
        AuthManager.Instance.SkipAuth();
        gameObject.SetActive(false);
        SceneLoader.Instance.LoadGame();
    }
}