using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        bool inputDetected = Input.GetKeyDown(KeyCode.Space) || 
            (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) ||
            Input.GetMouseButtonDown(0); // Ajout du clic gauche

        // => 1) Démarrer le gameplay (et faire un flap) si le jeu n'est pas encore lancé
        if (inputDetected && !GameStateManager.Instance.IsGameplayActive)
        {
            GameStateManager.Instance.StartGameplay();
            rb.simulated = true;
            tutorialCanvas.SetActive(false);

            // On force directement un flap ici
            rb.linearVelocity = Vector2.up * velocity; 
        }

        // => 2) Jouer l'anim/son du flap s’il y a un input et qu'on n'est pas GameOver
        if (inputDetected && !GameStateManager.Instance.IsGameOver)
        {
            birdAnim.Play("birdFlap");
            audioSource.PlayOneShot(flapSound);
            audioSource.pitch = Random.Range(0.9f, 1.1f);
        }

        inGameScoreText.text = score.ToString();
    }

    void FixedUpdate()
    {
        // Ici, on bloque la physique si le jeu n'a pas commencé OU s’il est terminé
        if (!GameStateManager.Instance.IsGameplayActive || GameStateManager.Instance.IsGameOver)
            return;

        if (Input.GetKey(KeyCode.Space) ||
            (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) ||
            Input.GetMouseButtonDown(0))
        {
            rb.linearVelocity = Vector2.up * velocity;
            isFalling = false;
        }
        else if (rb.linearVelocity.y < -6 && !isFalling)
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