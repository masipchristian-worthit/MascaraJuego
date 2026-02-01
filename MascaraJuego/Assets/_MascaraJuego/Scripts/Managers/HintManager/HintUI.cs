using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HintUI : MonoBehaviour
{
    [SerializeField] private HintSO _hint;
    [SerializeField] private TMP_Text _hintText;
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _hintDescription;
    private CanvasGroup _canvasGroup;
    RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetHint(HintSO hint)
    {
        _canvasGroup.alpha = 1;
        _hint = hint;
        _hintText.text = _hint._name;
        _hintDescription.text = _hint._description;
        _image.sprite = _hint._image;    
    }

    public void SetHintDiscover(HintSO hint)
    {
        _canvasGroup.DOFade(1, 1f);
        rectTransform.DOAnchorPos(Vector2.zero, 0.4f).SetEase(Ease.OutBack);
        _hint = hint;
        _hintText.text = _hint._name;
        _hintDescription.text = _hint._description;
        _image.sprite = _hint._image;  
    }

    public void CloseHint()
    {
        _canvasGroup.DOFade(0, 0.5f);
        rectTransform.DOAnchorPos(Vector2.zero, 0.2f).SetEase(Ease.OutBack);
    }
}
