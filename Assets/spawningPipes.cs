using System;
using UnityEngine;

public class spawningPipes : MonoBehaviour
{
    public float minTime = 1.5f;
    public float maxTime = 3f;
    private float timer = 0;
    public GameObject pipe;
    public float maxHeight;
    public float minHeight;
    private float nextSpawnTime;

    void Start()
    {
        SetNextSpawnTime();
    }

    void FixedUpdate()
    {
        if (!GameStateManager.Instance.IsGameplayActive)
            return;

        if (timer > nextSpawnTime)
        {
            SpawnPipe();
            timer = 0;
            SetNextSpawnTime();
        }

        timer += Time.fixedDeltaTime;
    }

    private void SpawnPipe()
    {
        GameObject newPipe = Instantiate(pipe);
        newPipe.transform.position = transform.position + new Vector3(0, UnityEngine.Random.Range(minHeight, maxHeight), 0);
    }

    private void SetNextSpawnTime()
    {
        nextSpawnTime = UnityEngine.Random.Range(minTime, maxTime);
    }

    public void ClearExistingPipes()
    {
        // Détruire tous les tuyaux existants
        GameObject[] existingPipes = GameObject.FindGameObjectsWithTag("pipe");
        foreach (GameObject pipe in existingPipes)
        {
            Destroy(pipe);
        }
        timer = 0;
        SetNextSpawnTime();
    }
}