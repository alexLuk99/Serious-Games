using UnityEngine;

public class MouseClickDetector : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))  // 0 steht für die linke Maustaste
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.CompareTag("Deck"))
                {
                    hit.collider.gameObject.GetComponent<Deck>().DisplayCards();
                }
            }
        }
    }
}
