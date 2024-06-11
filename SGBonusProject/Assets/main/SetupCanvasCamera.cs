using UnityEngine;

public class SetupCanvasCamera : MonoBehaviour
{
    public Canvas worldSpaceCanvas;
    public Camera camera;
    public float distanceFromCanvas = 10f;

    void Start()
    {
        if (worldSpaceCanvas.renderMode != RenderMode.WorldSpace)
        {
            Debug.LogError("The Canvas is not in World Space render mode.");
            return;
        }

        // Position the camera in front of the canvas
        RectTransform canvasRectTransform = worldSpaceCanvas.GetComponent<RectTransform>();
        Vector3 canvasCenter = canvasRectTransform.position;
        Vector3 cameraPosition = canvasCenter - canvasRectTransform.forward * distanceFromCanvas;

        camera.transform.position = cameraPosition;
        camera.transform.LookAt(canvasCenter);

        // Adjust the camera's orthographic size or field of view to fit the canvas
        float canvasHeight = canvasRectTransform.rect.height;
        float canvasWidth = canvasRectTransform.rect.width;

        if (camera.orthographic)
        {
            camera.orthographicSize = canvasHeight / 2;
        }
        else
        {
            float aspectRatio = canvasWidth / canvasHeight;
            camera.fieldOfView = 2 * Mathf.Atan(canvasHeight / (2 * distanceFromCanvas)) * Mathf.Rad2Deg;
            camera.aspect = aspectRatio;
        }
    }
}
