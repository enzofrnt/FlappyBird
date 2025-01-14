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
        Time.timeScale = 0;
        gameStarted = false;
    }

    void Update()
    {
        bool inputDetected = Input.GetKeyDown(KeyCode.Space) || 
            (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);

        if (inputDetected)
        {
            if (!gameStarted)
            {
                // Premier appui - démarrage du jeu
                gameStarted = true;
                Time.timeScale = 1;
            }

            birdAnim.Play("birdFlap");
            audioSource.PlayOneShot(flapSound);
            audioSource.pitch = Random.Range(0.9f, 1.1f);
        }

        inGameScoreText.text = score.ToString();
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Space) || 
            (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            rb.linearVelocity = Vector2.up * velocity; // Gérer la vitesse avec la physique
            isFalling = false; // L'oiseau n'est plus en chute
        }
        else if (rb.linearVelocity.y < -6 && !isFalling)
        {
            // Si l'oiseau est en chute libre et le son n'est pas joué
            isFalling = true;
            audioSource.PlayOneShot(fallingSound);
        }
        else if (rb.linearVelocity  .y >= 0)
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
        audioSource.PlayOneShot(gameOverSound);
        gameOverCanvas.SetActive(true);
        Time.timeScale = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        audioSource.PlayOneShot(pointSound);
        score++;
    }

    public void playAgain()
    {
        SceneManager.LoadScene(0);
    }
}