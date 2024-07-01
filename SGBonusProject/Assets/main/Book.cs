using UnityEngine;
using UnityEngine.UI;

public class Book : MonoBehaviour
{
    public GameObject recipePopup;

    void Start()
    {
        recipePopup.SetActive(false);
    }

    void OnMouseDown()
    {
        recipePopup.SetActive(true);
        AudioManager.Instance.PlaySound("click1");
    }
}
