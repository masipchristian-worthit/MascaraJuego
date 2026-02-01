using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI; 

public class ButtonDoAlpha : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private Image image;
    
    [SerializeField] private float hoverAlpha = 0.5f;  
    [SerializeField] private float duration = 0.5f;    

    private void Awake()
    {
        image = GetComponent<Image>();  
    }

    public void OnSelect(BaseEventData eventData)
    {
        image.DOFade(hoverAlpha, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        image.DOKill();  
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);  
    }
}