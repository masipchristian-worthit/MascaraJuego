using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private CanvasGroup _canvasGroup;
   // public enum { Settings, Hints, };
   public static PauseManager Instance;
   [SerializeField] private GameObject _pausePanel;

   private void Awake()
   {
       if (Instance == null)
       {
           Instance = this;
       }
       else
       {
           Destroy(gameObject);
       }
       
   }

   public void pauseGame()
    {
        _pausePanel.SetActive(true);
        _animator.Play("Open");
        
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
