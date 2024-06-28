using UnityEngine;

public class MouseClickDetector : MonoBehaviour
{
    public GameObject fridge;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Raycast hit: " + hit.transform.name); // Debugging-Ausgabe
                if (hit.transform.CompareTag("Fridge"))
                {
                    Debug.Log("Fridge clicked"); // Debugging-Ausgabe
                    fridge.GetComponent<FridgeController>().OnClick();
                }
            }
            else
            {
                Debug.Log("Raycast did not hit anything"); // Debugging-Ausgabe
            }
        }
    }
}
