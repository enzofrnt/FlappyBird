using UnityEngine;

public class movingPipes : MonoBehaviour
{
    void FixedUpdate()
    {
        // Utiliser la vitesse depuis le GameSpeedManager
        float speed = GameSpeedManager.Instance.speed;
        transform.position += Vector3.left * speed * Time.fixedDeltaTime;
    }
}