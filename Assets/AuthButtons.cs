using UnityEngine;
using UnityEngine.UI;

public class AuthButtons : MonoBehaviour
{
    public Button loginButton;
    public Button logoutButton;

    private void Start()
    {
        // S'abonner aux événements des boutons
        if (loginButton != null)
            loginButton.onClick.AddListener(OnLoginClick);
        if (logoutButton != null)
            logoutButton.onClick.AddListener(OnLogoutClick);

        // S'abonner à l'événement d'authentification
        AuthManager.Instance.OnAuthenticationChanged += UpdateButtonsVisibility;
        
        // Mettre à jour l'état initial des boutons
        UpdateButtonsVisibility(AuthManager.Instance.IsAuthenticated);
    }

    private void OnDestroy()
    {
        // Se désabonner des événements pour éviter les fuites de mémoire
        if (AuthManager.Instance != null)
            AuthManager.Instance.OnAuthenticationChanged -= UpdateButtonsVisibility;
    }

    private void UpdateButtonsVisibility(bool isAuthenticated)
    {
        if (loginButton != null)
            loginButton.gameObject.SetActive(!isAuthenticated);
        if (logoutButton != null)
            logoutButton.gameObject.SetActive(isAuthenticated);
    }

    public void OnLoginClick()
    {
        SceneLoader.Instance.LoadAuth();
    }

    public void OnLogoutClick()
    {
        AuthManager.Instance.Logout();
        // Ne pas recharger la scène, juste mettre à jour l'interface
        UpdateButtonsVisibility(false);
    }
}
