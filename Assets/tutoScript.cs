using TMPro;
using UnityEngine;
using DG.Tweening;

public class JouerAnimationAuDemarrage : MonoBehaviour
{
    void Start()
    {
        //TODO DOTween pour que ça marche j'ai besoin d'une pause moins merdique que celle qui utilise le temps qui ets mis à0 
        Debug.Log("L'animation commence " + GetComponent<TextMeshProUGUI>().text);
        GetComponent<TextMeshProUGUI>().DOColor(Color.red, 3f).SetLoops(-1, LoopType.Yoyo);
    }
}