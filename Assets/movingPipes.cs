using UnityEngine;

public class movingPipes : MonoBehaviour
{
    void FixedUpdate()
    {
        if (!GameStateManager.Instance.IsGameplayActive)
            return;
        
        float speed = GameSpeedManager.Instance.speed;
        transform.position += Vector3.left * speed * Time.fixedDeltaTime;
    }
}