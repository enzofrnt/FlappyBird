using UnityEngine;

public class movingPipes : MonoBehaviour
{
    public float speed = 2f; // Vitesse initiale des tuyaux
    public float speedIncrement = 1f; // Quantité d'accélération par seconde

    // FixedUpdate is called at a fixed interval and is independent of frame rate
    void FixedUpdate()
    {
        // Déplacement physique dans FixedUpdate
        transform.position += Vector3.left * speed * Time.fixedDeltaTime;

        // Augmenter progressivement la vitesse
        speed += speedIncrement * Time.fixedDeltaTime;
    }
}