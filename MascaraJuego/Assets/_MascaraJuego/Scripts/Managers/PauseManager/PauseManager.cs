using System;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private CanvasGroup _canvasGroup;
    public enum MyEnum { Settings, Hints,Main }
    public MyEnum currentOption;
   public static PauseManager Instance;
   [SerializeField] private GameObject _pausePanel;
    [SerializeField] private CanvasGroup _buttonsGroup;
    InputAction _actionEsc;
    [SerializeField] private GameObject _firstButton;
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

   private void Start()
   {
       _actionEsc = InputManager.Instance.UI.EscapeUI;
       _actionEsc.performed += ctx => escapeFunc();
   }

   public void pauseGame()
    {
        _pausePanel.SetActive(true);
        _animator.Play("Open");
        InputManager.Instance.SwitchTo(InputManager.InputMapType.UI);
        EventSystem.current.SetSelectedGameObject(_firstButton);
        currentOption = MyEnum.Main;
    }

    private void escapeFunc()
    {
        Debug.Log("HOLAAAA");
        switch (currentOption)
        {
            case MyEnum.Hints:
                toMain();
                break;
            case MyEnum.Settings:
                toMain();
                Debug.Log(" AJUSTED ATRAS");
                break;
            case MyEnum.Main:
                resume();
                Debug.Log("PARA juego");
                break;
        }
    }

    private void toMain()
    {
        _buttonsGroup.alpha = 1;
        currentOption = MyEnum.Main;
        Debug.Log("tomain");
        EventSystem.current.SetSelectedGameObject(_firstButton);
    }

    public void settingsMenu()
    {
        _buttonsGroup.alpha = 0;
        currentOption = MyEnum.Settings;
    }
    public void resume()
    {
        _animator.Play("Close");
        InputManager.Instance.SwitchTo(InputManager.InputMapType.Gameplay);
        EventSystem.current.SetSelectedGameObject(null);
        Debug.Log("Selected");
    }

    public void hintMenu()
    {
        HintManager.Instance.ShowHintsInMenu();
        _buttonsGroup.alpha = 0;
        currentOption = MyEnum.Hints;
    }

    public void exitGame()
    {
        Application.Quit();
    }
}
