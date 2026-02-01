using UnityEngine;

public class cutreScript : MonoBehaviour
{
    public void cancelWatch()
    {
        PlayerController.Instance.cancelWatch();
    }
}
