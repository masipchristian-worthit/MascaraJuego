    using System;
using DS.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using DS;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using System.Text.RegularExpressions;
using DG.Tweening;

public class DSDialogueUI : MonoBehaviour
{
    public static DSDialogueUI Instance;

    [SerializeField] private TextMeshProUGUI textUI; 
    [SerializeField] private TextMeshProUGUI speakerNameTextUI;
    [SerializeField] public Animator dialogueAnimator;
    [SerializeField] public Image leftCharacterImage;

    public GameObject dialogueBox;
    public GameObject optionsBox;
    [SerializeField] private List<GameObject> optionButtons;
    public DSDialogueSO currentDialogue;
    [SerializeField] public bool inDialogue;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private float dialogueSpeed;
    [SerializeField] private string dialogueAction;
    private DialogueVertexAnimator dialogueVertexAnimator;
    InputAction _interact;
    [SerializeField] private GameObject fake;

    [SerializeField] private GameObject _canvas;
    //private EventInstance dialogueAudio;
    //private EventReference dialogueAudioName;
    //public InputManager.ActionMaps actioMapToLoad;

    [SerializeField] private int minSkipableCharacters = 50;  

    [SerializeField] private bool isTextFullyVisible = false;
    private Coroutine fadeInCoroutine;
    
    private void Start()
    {
        _interact = InputManager.Instance.Dialogue.AcceptDialogue;
        _interact.performed += ctx => Interact_Button();
       
    }

    private void Awake()
    {
        Instance = this;
    }

    

    public void Interact_Button()
    {
        if (inDialogue)
        {
            // Si hay opciones de diálogo, no permitir la interacción
            if (optionsBox.activeSelf) // Comprobar si las opciones están activas
            {
                // Si hay opciones, se interactúa con las opciones, no con el texto
                GameObject selectedButton = EventSystem.current.currentSelectedGameObject;

                // Si no hay botón seleccionado, seleccionamos el primero
                if (selectedButton == null)
                {
                    EventSystem.current.SetSelectedGameObject(optionButtons[0]);
                    selectedButton = optionButtons[0];
                }

                // Invoca el clic en el botón seleccionado
                selectedButton.GetComponent<Button>().onClick.Invoke();
                return; // Salir de la función sin hacer nada más
            }

            // Si no hay opciones, procesamos el texto
            string parsedText = DialogueUtility.RemoveAnimationTags(currentDialogue.Text);

            if (parsedText.Length >= minSkipableCharacters)
            {
                if (!isTextFullyVisible)
                {
                    if (dialogueVertexAnimator.textAnimating)
                    {
                        StopAllCoroutines();  // Detiene la animación actual
                        CompleteTextWithEffects();  // Completa el texto de golpe
                    }
                    else
                    {
                        isTextFullyVisible = true;  // Si no está completo, lo marca como visible
                    }
                }
                else
                {
                    NextText();
                }
            }
            else
            {
                // Si el texto es corto, pasa directamente a la siguiente parte del diálogo
                if (isTextFullyVisible)
                {
                    NextText();
                }
            }
        }
    }



    private void CompleteTextWithEffects()
    {
        StopAllCoroutines();  
        dialogueVertexAnimator.textAnimating = false; 

        ApplyTextEffects(currentDialogue.Text);
        
        textUI.ForceMeshUpdate();  
        Color visibleColor = textUI.color;
        visibleColor.a = 1f;  
        textUI.color = visibleColor;

        isTextFullyVisible = true;
        
        checkHint();
    }

    void checkHint()
    {
        dialogueAction = currentDialogue.DialogueActionName;
        if (dialogueAction == "GiveHint")
        {
            HintManager.Instance.registerHint(currentDialogue.HintSo);
            InputManager.Instance.SwitchTo(InputManager.InputMapType.Hint);
        }
    }
    
    public void ShowText(DSDialogueSO dialogue)
    {
        if (inDialogue)
        {
            currentDialogue = dialogue;
            if (currentDialogue != null)
            {
                speakerNameTextUI.text = currentDialogue.SpeakerName;
                

                SetupCharacterImages();

                StopAllCoroutines();
                fadeInCoroutine = StartCoroutine(TypewriterEffect(currentDialogue.Text));
            }
        }
    }

    private void SetupCharacterImages()
    {
        if (currentDialogue.DialogueCharacterLeft != null)
        {
            leftCharacterImage.sprite = currentDialogue.DialogueCharacterLeft;
            Color leftColor = leftCharacterImage.color;
            leftColor.a = 1f; 
            leftCharacterImage.color = leftColor;
        }
        else
        {
            Color leftColor = leftCharacterImage.color;
            leftColor.a = 0f; 
            leftCharacterImage.color = leftColor;
        }
    }

    public IEnumerator TypewriterEffect(string sentence)
    {
        dialogueVertexAnimator = new DialogueVertexAnimator(textUI);
        dialogueVertexAnimator.textAnimating = true;

        List<DialogueCommand> commands = DialogueUtility.ProcessInputString(sentence, out string totalTextMessage);


        float dialogueSpeed;
        if (float.TryParse(currentDialogue.DialogueSpeed, out dialogueSpeed))
        {
            yield return StartCoroutine(dialogueVertexAnimator.AnimateTextIn(commands, totalTextMessage, OnTextAnimationComplete, dialogueSpeed,currentDialogue.Audio));
        }
        else
        {
            dialogueSpeed = 150f; 
            yield return StartCoroutine(dialogueVertexAnimator.AnimateTextIn(commands, totalTextMessage, OnTextAnimationComplete, dialogueSpeed,currentDialogue.Audio));
        }
    }
    private void onOptionsShow()
{
    if (currentDialogue.Choices.Count >= 2)
    {
        optionsBox.SetActive(true);
        EventSystem.current.SetSelectedGameObject(optionButtons[0]);

        int choiceCount = currentDialogue.Choices.Count;
        optionsBox.SetActive(choiceCount > 0);

        for (int i = 0; i < optionButtons.Count; i++)
        {
            if (i < choiceCount)
            {
                optionButtons[i].SetActive(true);
                optionButtons[i].transform.DOScale(1f, 0.3f);
                var buttonText = optionButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                var button = optionButtons[i].GetComponent<Button>();
                
                if (string.IsNullOrEmpty(buttonText.text))
                {
                    optionButtons[i].SetActive(false);
                }
                else
                {
                    buttonText.text = currentDialogue.Choices[i].Text;
                    
                    button.onClick.RemoveAllListeners();
                    
                    int optionIndex = i;
                    button.onClick.AddListener(() => OnOptionChosen(optionIndex));
                }
            }
            else
            {
                optionButtons[i].SetActive(false);
            }
        }
    }
    else
    {
        if (optionsBox != null)
        {
            optionsBox.SetActive(false);
        }
    }
}


    public void NextText()
    {
        if (!isTextFullyVisible)
        {
            CompleteTextWithEffects();
        }

        
        DSDialogueSO nextDialogue = currentDialogue.Choices.Count > 0 ? currentDialogue.Choices[0].NextDialogue : null;

        if (nextDialogue == null)
        {
            isTextFullyVisible = false;
            StartCoroutine(EndDialogue());
        }
        else
        {
            isTextFullyVisible = false;
            currentDialogue = nextDialogue;
            ShowText(currentDialogue);
        }

       
    }

    private IEnumerator EndDialogue()
    {
        dialogueAnimator.Play("Close");
        yield return new WaitForSeconds(0.1f);
        inDialogue = false;
        InputManager.Instance.ReturnToPreviousMap();
        Debug.Log("PENE");
        //actioMapToLoad = InputManager.ActionMaps.None;
    }
    
    private void ApplyTextEffects(string sentence)
    {

        List<DialogueCommand> commands = DialogueUtility.ProcessInputString(sentence, out string totalTextMessage);

        StartCoroutine(dialogueVertexAnimator.DisplayTextInstantly(totalTextMessage, commands, null));

        string cleanedText = DialogueUtility.RemoveAnimationTags(totalTextMessage);

        textUI.text = cleanedText;

        textUI.ForceMeshUpdate();
        onOptionsShow();
    }

    public void OnTextAnimationComplete()
    {
        isTextFullyVisible = true;  
        onOptionsShow();
        checkHint();
    }

    public void OnOptionChosen(int choiceIndex)
    {
        if (inDialogue)
        {
            if (choiceIndex >= 0 && choiceIndex < currentDialogue.Choices.Count)
            {
                DSDialogueSO nextDialogue = currentDialogue.Choices[choiceIndex].NextDialogue;

                if (nextDialogue == null)
                {
                    StartCoroutine(EndDialogue());
                }
                else
                {
                    currentDialogue = nextDialogue;
                    ShowText(currentDialogue);
                }

                onOptionsShow();
            }
        }
    }

    public void PlayDialogueNPC(DSDialogueContainerSO dialogueContainer)
    {
        dialogueAnimator.Play("Open");
        currentDialogue = dialogueContainer.GetFirstDialogue();
        inDialogue = true;
        EventSystem.current.SetSelectedGameObject(fake);
        Debug.Log(EventSystem.current.currentSelectedGameObject.name);
        InputManager.Instance.SwitchTo(InputManager.InputMapType.Dialogue);
        if (currentDialogue != null && !string.IsNullOrEmpty(currentDialogue.Text))
        {
            ShowText(currentDialogue);
        }
        else
        {
            Debug.LogError("El di�logo no tiene texto o es nulo.");
        }
    }
}
