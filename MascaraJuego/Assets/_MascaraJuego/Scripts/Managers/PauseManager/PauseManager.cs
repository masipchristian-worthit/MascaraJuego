using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private CanvasGroup _canvasGroup;
   // public enum { Settings, Hints, };

    public void pauseGame()
    {
        _animator.Play("Open");
        InputManager.Instance.SwitchTo(InputManager.InputMapType.UI);
    }


    public void resume()
    {
        _animator.Play("Close");
        InputManager.Instance.ReturnToPreviousMap();
    }

    public void hintMenu()
    {
        
    }
}
