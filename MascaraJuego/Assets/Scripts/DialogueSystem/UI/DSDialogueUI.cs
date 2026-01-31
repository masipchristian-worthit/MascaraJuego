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

    private DialogueVertexAnimator dialogueVertexAnimator;

    //private EventInstance dialogueAudio;
    //private EventReference dialogueAudioName;
    //public InputManager.ActionMaps actioMapToLoad;

    [SerializeField] private int minSkipableCharacters = 50;  

    [SerializeField] private bool isTextFullyVisible = false;
    private Coroutine fadeInCoroutine;
    
    private void Start()
    {
        //InputManager.Instance.skipDialogueAction += Interact_Button;
       
    }

    private void Awake()
    {
        Instance = this;
    }

    

    private void Interact_Button()
    {
        if (inDialogue)
        {
            string parsedText = DialogueUtility.RemoveAnimationTags(currentDialogue.Text);

            if (parsedText.Length >= minSkipableCharacters)
            {
                if (!isTextFullyVisible)
                {
                    if (dialogueVertexAnimator.textAnimating)
                    {
                        StopAllCoroutines();
                        CompleteTextWithEffects();
                    }
                    else
                    {
                        isTextFullyVisible = true;
                    }
                }
                else
                {
                    if (currentDialogue.Choices.Count >= 2)
                    {
                        GameObject selectedButton = EventSystem.current.currentSelectedGameObject;
                        if (selectedButton != null)
                        {
                            selectedButton.GetComponent<Button>().onClick.Invoke();
                        }
                    }
                    else
                    {
                        NextText();
                    }
                }
            }
            else
            {
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
        dialogueBox.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        inDialogue = false;
        //InputManager.Instance.ChangeCurrentActionMap(actioMapToLoad);
        //actioMapToLoad = InputManager.ActionMaps.None;
    }
    
    private void ApplyTextEffects(string sentence)
    {

        List<DialogueCommand> commands = DialogueUtility.ProcessInputString(sentence, out string totalTextMessage);

        StartCoroutine(dialogueVertexAnimator.DisplayTextInstantly(totalTextMessage, commands, null));

        string cleanedText = DialogueUtility.RemoveAnimationTags(totalTextMessage);

        textUI.text = cleanedText;

        textUI.ForceMeshUpdate();
    }

    public void OnTextAnimationComplete()
    {
        isTextFullyVisible = true;  
    }


    public void PlayDialogueNPC(DSDialogueContainerSO dialogueContainer)
    {
        dialogueBox.SetActive(true);
        currentDialogue = dialogueContainer.GetFirstDialogue();
        inDialogue = true;

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
