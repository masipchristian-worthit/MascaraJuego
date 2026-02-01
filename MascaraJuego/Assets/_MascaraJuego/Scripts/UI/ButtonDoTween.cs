using UnityEngine;
using DG.Tweening; 
using UnityEngine.EventSystems; 

public class ButtonDoTween : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private RectTransform rectTransform;
    
    [SerializeField] private Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1f);
    [SerializeField] private float duration = 0.5f; 

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    
    public void OnSelect(BaseEventData eventData)
    {
        rectTransform.DOScale(hoverScale, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
    
    public void OnDeselect(BaseEventData eventData)
    {
        rectTransform.DOKill(); 
        rectTransform.localScale = Vector3.one;
    }
}