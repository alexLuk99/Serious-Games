using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // Zentrale Variablen
    [Header("Drag Settings")]
    public float tiltAmount = 5f; // Amount to tilt the card in degrees
    public float speedThreshold = 500f; // Speed threshold to determine tilt
    public float maxTiltAngle = 0.5f; // Maximum tilt angle to further limit the severity of the tilt
    public float snapBackDuration = 0.5f; // Duration of the snap-back animation
    public float tiltSmoothTime = 0.2f; // Smoothing time for tilt
    public float movementThreshold = 0.1f; // Movement threshold to reset tilt when drag movement stops

    private Vector3 offset;
    private RectTransform canvasRectTransform;
    private RectTransform rectTransform;
    private Vector3 originalPosition;
    private Vector3 lastPosition;
    private Coroutine returnCoroutine;
    private Coroutine resetTiltCoroutine;
    private Vector3 currentTilt;
    private Vector3 tiltVelocity;
    private float originalZPosition;
    private bool isDragging = false;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        originalPosition = rectTransform.localPosition;
        originalZPosition = rectTransform.position.z;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRectTransform, eventData.position, eventData.pressEventCamera, out Vector3 worldPoint);
        offset = rectTransform.position - worldPoint;

        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }

        lastPosition = eventData.position;
        isDragging = true;

        // Stop the tilt reset coroutine if it's running
        if (resetTiltCoroutine != null)
        {
            StopCoroutine(resetTiltCoroutine);
            resetTiltCoroutine = null;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRectTransform, eventData.position, eventData.pressEventCamera, out Vector3 worldPoint);
        Vector3 newPosition = worldPoint + offset;
        newPosition.z = originalZPosition; // Keep the original Z position
        rectTransform.position = newPosition;

        // Calculate the movement direction and speed
        Vector3 movement = (Vector3)eventData.position - lastPosition;
        float speed = movement.magnitude / Time.deltaTime;

        // Reset tilt if the card is not moving significantly
        if (speed < movementThreshold)
        {
            currentTilt = Vector3.SmoothDamp(currentTilt, Vector3.zero, ref tiltVelocity, tiltSmoothTime);
        }
        else
        {
            // Tilt the card based on the movement
            float targetTiltX = Mathf.Clamp(movement.y, -speedThreshold, speedThreshold) / speedThreshold * tiltAmount;
            float targetTiltY = Mathf.Clamp(movement.x, -speedThreshold, speedThreshold) / speedThreshold * tiltAmount;

            // Limit the tilt angle to the maxTiltAngle
            targetTiltX = Mathf.Clamp(targetTiltX, -maxTiltAngle, maxTiltAngle);
            targetTiltY = Mathf.Clamp(targetTiltY, -maxTiltAngle, maxTiltAngle);

            Vector3 targetTilt = new Vector3(targetTiltX, -targetTiltY, 0);
            currentTilt = Vector3.SmoothDamp(currentTilt, targetTilt, ref tiltVelocity, tiltSmoothTime);
        }

        rectTransform.localRotation = Quaternion.Euler(currentTilt);
        lastPosition = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        isDragging = false;

        // Start the reset tilt coroutine to smoothly reset the rotation
        if (resetTiltCoroutine == null)
        {
            resetTiltCoroutine = StartCoroutine(ResetTilt());
        }

        // Check if the card is outside the canvas
        if (!IsRectTransformInsideCanvas(rectTransform, canvasRectTransform))
        {
            // Start the return coroutine
            returnCoroutine = StartCoroutine(SnapBackToOriginalPosition());
        }
    }

    private void Update()
    {
        if (isDragging && resetTiltCoroutine == null)
        {
            // Reset tilt if the card is not moving significantly
            currentTilt = Vector3.SmoothDamp(currentTilt, Vector3.zero, ref tiltVelocity, tiltSmoothTime);
            rectTransform.localRotation = Quaternion.Euler(currentTilt);
        }
    }

    private IEnumerator ResetTilt()
    {
        Vector3 startTilt = currentTilt;
        float elapsed = 0f;

        while (elapsed < tiltSmoothTime)
        {
            currentTilt = Vector3.Lerp(startTilt, Vector3.zero, elapsed / tiltSmoothTime);
            rectTransform.localRotation = Quaternion.Euler(currentTilt);
            elapsed += Time.deltaTime;
            yield return null;
        }

        currentTilt = Vector3.zero;
        rectTransform.localRotation = Quaternion.Euler(currentTilt);
        resetTiltCoroutine = null;
    }

    private bool IsRectTransformInsideCanvas(RectTransform rectTransform, RectTransform canvasRectTransform)
    {
        Vector3[] canvasCorners = new Vector3[4];
        Vector3[] objectCorners = new Vector3[4];

        canvasRectTransform.GetWorldCorners(canvasCorners);
        rectTransform.GetWorldCorners(objectCorners);

        for (int i = 0; i < objectCorners.Length; i++)
        {
            if (objectCorners[i].x < canvasCorners[0].x || objectCorners[i].x > canvasCorners[2].x ||
                objectCorners[i].y < canvasCorners[0].y || objectCorners[i].y > canvasCorners[2].y)
            {
                return false;
            }
        }
        return true;
    }

    private IEnumerator SnapBackToOriginalPosition()
    {
        Vector3 startPosition = rectTransform.localPosition;
        float elapsed = 0f;

        while (elapsed < snapBackDuration)
        {
            rectTransform.localPosition = Vector3.Lerp(startPosition, originalPosition, elapsed / snapBackDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rectTransform.localPosition = originalPosition;
    }
}
