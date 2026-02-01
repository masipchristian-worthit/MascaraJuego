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
    [SerializeField] private GameObject _fakeButton;
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
                HintManager.Instance.HideHintMenu();
                break;
            case MyEnum.Settings:
                toMain();
                Debug.Log(" AJUSTED ATRAS");
                break;
            case MyEnum.Main:
                resume();
                Debug.Log("PARA juego");
                EventSystem.current.SetSelectedGameObject(_fakeButton);
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
        EventSystem.current.SetSelectedGameObject(_fakeButton);
        Debug.Log("Selected");
        PlayerController.Instance.cancelWatch();
    }

    public void hintMenu()
    {
        if (HintManager.Instance != null)
        {
            HintManager.Instance.ShowHintsInMenu();
        }
        else
        {
            Debug.Log("No hints found");
        }
        _buttonsGroup.alpha = 0;
        currentOption = MyEnum.Hints;
    }

    public void exitGame()
    {
        Application.Quit();
    }
}
