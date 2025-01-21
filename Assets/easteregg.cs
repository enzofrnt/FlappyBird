using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class easteregg : MonoBehaviour
{
    [Header("Configuration")]
    public AudioClip secretSound;
    private AudioSource audioSource;
    private bool isPlaying = false;

    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        // Quand on appuie sur K
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (secretSound != null && audioSource != null)
            {
                audioSource.clip = secretSound;
                audioSource.loop = true;
                audioSource.Play();
                isPlaying = true;
                Debug.Log("Son secret commencé!");
            }
        }

        // Quand on relâche K
        if (Input.GetKeyUp(KeyCode.K))
        {
            if (audioSource != null && isPlaying)
            {
                audioSource.Stop();
                isPlaying = false;
                Debug.Log("Son secret arrêté!");
            }
        }
    }
}
