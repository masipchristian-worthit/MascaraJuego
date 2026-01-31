using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HintButton : MonoBehaviour
{
    private HintSO Hint;
    private TMP_Text _text;
    private Button _button;
    
    
    public void SetTextButton(HintSO hint)
    {
        _text = GetComponentInChildren<TMP_Text>();
        Hint = hint;
        _text.text = hint._name;
        _button = this.GetComponent<Button>();
        _button.onClick.AddListener(() => HintManager.Instance.ShowHintsInMenu(Hint));
    }
}
