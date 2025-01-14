using UnityEngine;
using System;

public class GroundRepeater : MonoBehaviour
{
    public float resetPosition = -4f;
    private Transform[] groundPieces;
    private float tileWidth;

    // Tableau de sprites variés que vous assignerez depuis l'Inspector
    public Sprite[] possibleGroundSprites;  

    void Start()
    {
        // Récupérer tous les enfants
        groundPieces = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            groundPieces[i] = transform.GetChild(i);
        }

        // (Optionnel) trier les morceaux de gauche à droite
        Array.Sort(groundPieces, (a, b) => a.position.x.CompareTo(b.position.x));

        // Récupérer la largeur depuis le premier SpriteRenderer
        // (ou via un collider, ou bounds cumulés si nécessaire)
        SpriteRenderer sr = groundPieces[0].GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            tileWidth = sr.bounds.size.x;
        }
        else
        {
            Debug.LogError("Pas de SpriteRenderer sur le premier morceau !");
        }
    }

    void FixedUpdate()
    {
        float speed = GameSpeedManager.Instance.speed;

        foreach (Transform piece in groundPieces)
        {
            piece.position += Vector3.left * speed * Time.fixedDeltaTime;

            // Quand le morceau sort de l'écran, on le repositionne
            if (piece.position.x <= resetPosition)
            {
                RepositionPiece(piece);
            }
        }
    }

    private void RepositionPiece(Transform piece)
    {
        // Chercher le morceau le plus à droite
        Transform furthestPiece = GetFurthestPiece();

        // Calculer la nouvelle position x
        float newX = furthestPiece.position.x + tileWidth - 0.05f; // 0.05f pour éviter les espaces
        piece.position = new Vector3(newX, piece.position.y, piece.position.z);

        // Assigner un sprite au hasard si on a un SpriteRenderer
        SpriteRenderer sr = piece.GetComponent<SpriteRenderer>();
        if (sr != null && possibleGroundSprites.Length > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, possibleGroundSprites.Length);
            sr.sprite = possibleGroundSprites[randomIndex];
        }
    }

    private Transform GetFurthestPiece()
    {
        Transform furthestPiece = groundPieces[0];
        foreach (Transform piece in groundPieces)
        {
            if (piece.position.x > furthestPiece.position.x)
            {
                furthestPiece = piece;
            }
        }
        return furthestPiece;
    }
}