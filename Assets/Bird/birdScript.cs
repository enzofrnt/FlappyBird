using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;

public class birdScript : MonoBehaviour
{
    private bool gameStarted = false;
    public float velocity = 1;
    private Rigidbody2D rb;
    public static int score = 0;
    public TextMeshProUGUI inGameScoreText;
    public GameObject gameOverCanvas;
    public GameObject tutorialCanvas;
    public Animator birdAnim;
    public float tiltAngle = 45f; // Angle maximum d'inclinaison
    public float rotationSpeed = 5f; // Vitesse de rotation de l'oiseau
    public AudioSource audioSource;
    public AudioClip flapSound, gameOverSound, hitSound, pointSound, fallingSound;

    private bool isFalling = false; // État pour détecter la chute
    private PlayerInputActions controls;

    private void Awake()
    {
        controls = new PlayerInputActions();
        
        // Configurer le callback pour l'action Flap
        controls.Gameplay.Flap.performed += ctx => OnFlapInput();
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    private void OnFlapInput()
    {
        // Utiliser un délai d'une frame pour la vérification UI
        StartCoroutine(CheckUIAndFlap());
    }

    private IEnumerator CheckUIAndFlap()
    {
        yield return null; // Attendre une frame
        
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            yield break;

        // Si le jeu n'est pas encore lancé
        if (!GameStateManager.Instance.IsGameplayActive && !GameStateManager.Instance.IsGameOver)
        {
            GameStateManager.Instance.StartGameplay();
            rb.simulated = true;
            tutorialCanvas.SetActive(false);
            rb.linearVelocity = Vector2.up * velocity;
        }
        // Si le jeu est en cours
        else if (!GameStateManager.Instance.IsGameOver)
        {
            birdAnim.Play("birdFlap");
            audioSource.PlayOneShot(flapSound);
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            rb.linearVelocity = Vector2.up * velocity;
        }
    }

    void Start()
    {
        score = 0;
        rb = GetComponent<Rigidbody2D>();
        rb.simulated = false; // Désactive la physique au démarrage
        GameStateManager.Instance.StopGameplay();
        tutorialCanvas.SetActive(true);
        gameOverCanvas.SetActive(false);
    }

    void Update()
    {
        // On ne fait rien si le jeu est terminé
        if (GameStateManager.Instance.IsGameOver) 
            return;

        // Mise à jour du score uniquement
        if (inGameScoreText != null)
            inGameScoreText.text = score.ToString();
    }

    void FixedUpdate()
    {
        // Ici, on bloque la physique si le jeu n'a pas commencé OU s'il est terminé
        if (!GameStateManager.Instance.IsGameplayActive || GameStateManager.Instance.IsGameOver)
            return;

        // La gestion des inputs est maintenant gérée par OnFlapInput()
        if (rb.linearVelocity.y < -6 && !isFalling)
        {
            // Si l'oiseau est en chute libre et le son n'est pas joué
            isFalling = true;
            audioSource.PlayOneShot(fallingSound);
        }
        else if (rb.linearVelocity.y >= 0)
        {
            // Réinitialiser l'état si l'oiseau arrête de tomber
            isFalling = false;
        }

        // Limiter la position Y de l'oiseau
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, -4.5f, 4.5f);
        transform.position = pos;

        // Ajuster l'inclinaison en fonction de la vitesse verticale
        AdjustBirdRotation();
    }

    private void AdjustBirdRotation()
    {
        // Calculer l'angle cible en fonction de la vitesse verticale
        float targetAngle = Mathf.Lerp(-tiltAngle, tiltAngle, (rb.linearVelocity.y + velocity) / (2 * velocity));

        // Appliquer une rotation en douceur vers l'angle cible
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        audioSource.PlayOneShot(hitSound);
        
        if (GameStateManager.Instance.IsGameplayActive) 
        {
            audioSource.PlayOneShot(gameOverSound);
            // N'envoyer le score que si on n'est pas en mode skip
            if (!AuthManager.Instance.IsSkipMode)
            {
                ScoreManager.Instance.SendScore(score);
            }
        }

        gameOverCanvas.SetActive(true);
        GameStateManager.Instance.StopGameplay();
        GameStateManager.Instance.GameOver();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        audioSource.PlayOneShot(pointSound);
        score++;
    }

    public void playAgain()
    {
        // => Réinitialiser le score, la vitesse, etc.
        GameSpeedManager.Instance.ResetSpeed();
        score = 0;

        // => Réinitialiser la position et l'orientation de l'oiseau
        transform.position = new Vector3(0f, 0f, 0f);
        transform.rotation = Quaternion.identity;
        rb.simulated = false; // Désactive la physique quand on recommence
        rb.linearVelocity = Vector2.zero;

        // => Réactiver l'écran de tutoriel, masquer l'écran de game over
        gameOverCanvas.SetActive(false);
        tutorialCanvas.SetActive(true);

        // => Dire à GameStateManager qu'on n'est plus en game over
        GameStateManager.Instance.RestartGame();
        // => Et on arrête le gameplay (en attendant le prochain input)
        GameStateManager.Instance.StopGameplay();

        // => Nettoyer les tuyaux existants
        FindObjectOfType<spawningPipes>().ClearExistingPipes();
    }
}