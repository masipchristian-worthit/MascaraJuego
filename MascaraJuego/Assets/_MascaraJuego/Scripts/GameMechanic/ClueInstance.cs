using UnityEngine;

public class ClueInstance : MonoBehaviour
{
    public CharacterDataSO ownerData; // De quién es esta pista (ej: Kanye)
    
    private string currentDescription;
    private bool isIncriminating;

    public void SetupClue(bool makeIncriminating)
    {
        gameObject.SetActive(true); // Aseguramos que se ve
        isIncriminating = makeIncriminating;

        if (makeIncriminating)
        {
            currentDescription = ownerData.incriminatingClueDescription;
        }
        else
        {
            currentDescription = ownerData.normalClueDescription;
        }
    }

    public string GetDescription()
    {
        return currentDescription;
    }
}