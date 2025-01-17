using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    private void Start()
    {
        // Vérifier si déjà authentifié
        if (AuthManager.Instance.IsAuthenticated)
        {
            gameObject.SetActive(false);
            return;
        }

        skipAuthButton.onClick.AddListener(OnSkipAuthClick);
        ShowLoginPanel();
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
        loadingPanel.SetActive(true);
        loginErrorText.text = "";

        AuthManager.Instance.Login(
            loginUsername.text,
            loginPassword.text,
            (success) =>
            {
                loadingPanel.SetActive(false);
                if (success)
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    loginErrorText.text = "Échec de la connexion";
                }
            }
        );
    }

    public void OnRegisterClick()
    {
        loadingPanel.SetActive(true);
        registerErrorText.text = "";

        AuthManager.Instance.Register(
            registerUsername.text,
            registerEmail.text,
            registerPassword.text,
            (success) =>
            {
                loadingPanel.SetActive(false);
                if (success)
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    registerErrorText.text = "Échec de l'inscription";
                }
            }
        );
    }

    public void OnSkipAuthClick()
    {
        AuthManager.Instance.SkipAuth();
        gameObject.SetActive(false);
        SceneLoader.Instance.LoadGame();
    }
}