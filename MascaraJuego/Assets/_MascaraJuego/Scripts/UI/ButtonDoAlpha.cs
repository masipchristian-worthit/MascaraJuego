using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI; 

public class ButtonDoAlpha : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private Image image;
    private Button button;
    
    [SerializeField] private float hoverAlpha = 0.3f;  
    [SerializeField] private float duration = 0.5f;    

    private void Awake()
    {
        image = GetComponent<Image>();  
        button = GetComponent<Button>(); // Obtener el componente Button

        if (button != null)
        {
            // Suscribir el botón al evento OnClick para que también se ejecute el OnDeselect al hacer clic
            button.onClick.AddListener(OnDeselectButton);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        // Iniciar el parpadeo de alpha
        image.DOFade(hoverAlpha, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        // Detener el parpadeo y restaurar alpha a 1 cuando se deselecciona
        image.DOKill();
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);  
    }

    // Método que se llamará cuando se haga clic en el botón
    private void OnDeselectButton()
    {
        OnDeselect(null); // Llamar al mismo comportamiento de deselección cuando se hace clic
    }
}