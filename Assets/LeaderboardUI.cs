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
        public int score;
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

    private void Start()
    {
        leaderboardPanel.SetActive(false);
        
        // Configuration du conteneur
        VerticalLayoutGroup layoutGroup = scoreContainer.gameObject.AddComponent<VerticalLayoutGroup>();
        layoutGroup.childAlignment = TextAnchor.UpperCenter;
        layoutGroup.spacing = 25f;
        layoutGroup.padding = new RectOffset(20, 20, 20, 20);
        
        // Configuration du ScrollRect
        ScrollRect scrollRect = scoreContainer.GetComponentInParent<ScrollRect>();
        if (scrollRect != null)
        {
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
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
        using (UnityWebRequest www = UnityWebRequest.Get(AuthManager.API_URL + "api/scores/"))
        {
            if (AuthManager.Instance.IsAuthenticated)
            {
                www.SetRequestHeader("Authorization", $"Token {AuthManager.Instance.PlayerToken}");
            }

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonArray = www.downloadHandler.text;
                // Convertir le tableau JSON en objet JSON
                string jsonObject = $"{{\"scores\":{jsonArray}}}";
                ScoreListResponse response = JsonUtility.FromJson<ScoreListResponse>(jsonObject);
                DisplayScores(response.scores);
            }
            else
            {
                Debug.LogError($"Erreur lors de la récupération des scores : {www.error}");
            }
        }
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