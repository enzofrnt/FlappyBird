using TMPro;
using UnityEditor.Tilemaps;
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
            rb.linearVelocity = Vector2.up * velocity;
        }

        // Limiter la position Y de l'oiseau
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, -4.5f, 4.5f);
        transform.position = pos;

        inGameScoreText.text = score.ToString();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        gameOverCanvas.SetActive(true);
        Time.timeScale = 0;
    }

    private  void OnTriggerEnter2D(Collider2D collision)
    {
        score++;
    }

    public void playAgain()
    {
        SceneManager.LoadScene(0);
    }
}
