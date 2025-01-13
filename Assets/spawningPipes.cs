using System;
using UnityEngine;

public class spawningPipes : MonoBehaviour
{
    public float minTime = 1f; // Temps minimal entre les spawns
    public float maxTime = 100000000f; // Temps maximal entre les spawns
    private float timer = 0;
    public GameObject pipe;
    public float height;

    private float nextSpawnTime; // Temps défini pour le prochain spawn

    // Start is called before the first frame update
    void Start()
    {
        SetNextSpawnTime(); // Initialiser le temps du prochain spawn
        SpawnPipe(); // Créer un premier tuyau
    }

    // FixedUpdate is called at a fixed interval and is independent of frame rate
    void FixedUpdate()
    {
        if (timer > nextSpawnTime)
        {
            SpawnPipe(); // Créer un nouveau tuyau
            timer = 0;
            SetNextSpawnTime(); // Définir le temps pour le prochain spawn
        }

        timer += Time.fixedDeltaTime; // Incrémenter le timer
    }

    private void SpawnPipe()
    {
        GameObject newPipe = Instantiate(pipe);
        newPipe.transform.position = transform.position + new Vector3(0, UnityEngine.Random.Range(-height, height), 0);
        Destroy(newPipe, 15); // Détruire le tuyau après 15 secondes
    }

    private void SetNextSpawnTime()
    {
        // Définir aléatoirement le temps pour le prochain spawn entre minTime et maxTime
        nextSpawnTime = UnityEngine.Random.Range(minTime, maxTime);
    }
}