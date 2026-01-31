using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "HintSO", menuName = "Scriptable Objects/HintSO")]
public class HintSO : ScriptableObject
{
    public string _name;
    public string _description;
    public Sprite _image;
}
