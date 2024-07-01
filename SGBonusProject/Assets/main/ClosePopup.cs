using UnityEngine;

public class ClosePopup : MonoBehaviour
{
    void OnMouseDown()
    {
        gameObject.SetActive(false);
        AudioManager.Instance.PlaySound("click1");
    }
}
