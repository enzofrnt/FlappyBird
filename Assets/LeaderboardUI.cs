using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;

public class LeaderboardUI : MonoBehaviour
{
    [System.Serializable]
    private class ScoreData
    {
        public long score;
        public string player_name;
    }

    [System.Serializable]
    private class ScoreListResponse
    {
        public List<ScoreData> scores;
    }

    public GameObject leaderboardPanel;
    public GameObject scoreItemPrefab;
    public Transform scoreContainer;
    public Button leaderboardButton;

    private void Start()
    {
        leaderboardPanel.SetActive(false);
                
        // Configuration du conteneur
        VerticalLayoutGroup layoutGroup = scoreContainer.gameObject.AddComponent<VerticalLayoutGroup>();
        layoutGroup.childAlignment = TextAnchor.UpperCenter;
        layoutGroup.spacing = 25f;
        layoutGroup.padding = new RectOffset(20, 20, 20, 20);
        
        // Ajout et configuration du ContentSizeFitter
        ContentSizeFitter contentFitter = scoreContainer.gameObject.AddComponent<ContentSizeFitter>();
        contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        contentFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        
        // Configuration du ScrollRect
        ScrollRect scrollRect = scoreContainer.GetComponentInParent<ScrollRect>();
        if (scrollRect != null)
        {
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Elastic;
            scrollRect.elasticity = 0.1f;
            scrollRect.inertia = true;
            scrollRect.decelerationRate = 0.135f;
        }

        // S'abonner à l'événement d'authentification
        AuthManager.Instance.OnAuthenticationChanged += UpdateButtonState;
        
        // Mettre à jour l'état initial du bouton
        UpdateButtonState(AuthManager.Instance.IsAuthenticated);
    }

    private void OnDestroy()
    {
        // Se désabonner de l'événement
        if (AuthManager.Instance != null)
            AuthManager.Instance.OnAuthenticationChanged -= UpdateButtonState;
    }

    private void UpdateButtonState(bool isAuthenticated)
    {
        if (leaderboardButton != null)
        {
            leaderboardButton.interactable = isAuthenticated && !AuthManager.Instance.IsSkipMode;
        }
    }

    public void ShowLeaderboard()
    {
        leaderboardPanel.SetActive(true);
        StartCoroutine(GetTopScores());
    }

    public void HideLeaderboard()
    {
        leaderboardPanel.SetActive(false);
    }

    private IEnumerator GetTopScores()
    {
        yield return StartCoroutine(APIManager.Instance.GetScores((jsonArray) => {
            if (jsonArray != null)
            {
                string jsonObject = $"{{\"scores\":{jsonArray}}}";
                ScoreListResponse response = JsonUtility.FromJson<ScoreListResponse>(jsonObject);
                DisplayScores(response.scores);
            }
            else
            {
                Debug.LogError("Erreur lors de la récupération des scores");
            }
        }));
    }

    private void DisplayScores(List<ScoreData> scores)
    {
        // Nettoyer les anciens scores
        foreach (Transform child in scoreContainer)
        {
            Destroy(child.gameObject);
        }
        
        int rank = 1;
        foreach (var score in scores)
        {
            GameObject item = Instantiate(scoreItemPrefab, scoreContainer);
            item.transform.localScale = Vector3.one;
            
            // Configuration du RectTransform
            RectTransform rectTransform = item.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 0);
            rectTransform.sizeDelta = new Vector2(0, 70);
            
            var texts = item.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length >= 3)
            {
                texts[0].text = $"#{rank}";
                texts[1].text = score.player_name;
                texts[2].text = score.score.ToString();
            }
            
            rank++;
        }
    }
} 