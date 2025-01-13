using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class birdScript : MonoBehaviour
{
    public float velocity = 1;
    private Rigidbody2D rb;
    public static int score = 0;
    public TextMeshProUGUI inGameScoreText;
    public GameObject gameOverCanvas;
    public Animator birdAnim;
    public float tiltAngle = 45f; // Angle maximum d'inclinaison
    public float rotationSpeed = 5f; // Vitesse de rotation de l'oiseau

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        score = 0;
        rb = GetComponent<Rigidbody2D>();

        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            birdAnim.Play("birdFlap");
            // On applique la force ou la vitesse dans FixedUpdate
        }

        // Mettre à jour le score à l'écran
        inGameScoreText.text = score.ToString();
    }

    // FixedUpdate is called at a fixed interval and is independent of frame rate
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rb.linearVelocity = Vector2.up * velocity; // Gérer la vitesse avec la physique
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
        gameOverCanvas.SetActive(true);
        Time.timeScale = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        score++;
    }

    public void playAgain()
    {
        SceneManager.LoadScene(0);
    }
}