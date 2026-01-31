using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HintUI : MonoBehaviour
{
    [SerializeField] private HintSO _hint;
    [SerializeField] private TMP_Text _hintText;
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _hintDescription;

    public void SetHint(HintSO hint)
    {
        _hint = hint;
        _hintText.text = _hint._name;
        _hintDescription.text = _hint._description;
        _image.sprite = _hint._image;
    }
}
