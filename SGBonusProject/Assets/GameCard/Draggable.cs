using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Drag Settings")]
    public float tiltAmount = 5f; 
    public float speedThreshold = 500f;
    public float maxTiltAngle = 0.5f;
    public float snapBackDuration = 0.5f;
    public float tiltSmoothTime = 0.2f;
    public float movementThreshold = 0.1f;

    private Vector3 offset;
    private RectTransform canvasRectTransform;
    private RectTransform rectTransform;
    private Vector3 originalPosition;
    private Vector3 lastPosition;
    private Coroutine returnCoroutine;
    private Coroutine resetTiltCoroutine;
    private Vector3 currentTilt;
    private Vector3 tiltVelocity;
    private bool isDragging = false;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        // Store the initial position only once
        originalPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRectTransform, eventData.position, eventData.pressEventCamera, out Vector3 worldPoint);
        offset = rectTransform.position - worldPoint;

        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }

        lastPosition = eventData.position;
        isDragging = true;

        if (resetTiltCoroutine != null)
        {
            StopCoroutine(resetTiltCoroutine);
            resetTiltCoroutine = null;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRectTransform, eventData.position, eventData.pressEventCamera, out Vector3 worldPoint);
        Vector3 newPosition = worldPoint + offset;
        newPosition.z = rectTransform.position.z; // Ensure z position remains unchanged
        rectTransform.position = newPosition;

        Debug.Log($"OnDrag: newPosition = {newPosition}, rectTransform.position = {rectTransform.position}");

        Vector3 movement = (Vector3)eventData.position - lastPosition;
        float speed = movement.magnitude / Time.deltaTime;

        if (speed < movementThreshold)
        {
            currentTilt = Vector3.SmoothDamp(currentTilt, Vector3.zero, ref tiltVelocity, tiltSmoothTime);
        }
        else
        {
            float targetTiltX = Mathf.Clamp(movement.y, -speedThreshold, speedThreshold) / speedThreshold * tiltAmount;
            float targetTiltY = Mathf.Clamp(movement.x, -speedThreshold, speedThreshold) / speedThreshold * tiltAmount;

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
        isDragging = false;

        if (resetTiltCoroutine == null)
        {
            resetTiltCoroutine = StartCoroutine(ResetTilt());
        }

        // Always snap back to the original position
        if (returnCoroutine == null)
        {
            returnCoroutine = StartCoroutine(SnapBackToOriginalPosition());
        }
    }

    private void Update()
    {
        if (isDragging && resetTiltCoroutine == null)
        {
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

    private IEnumerator SnapBackToOriginalPosition()
    {
        Vector2 startPosition = rectTransform.anchoredPosition;
        float startZ = rectTransform.position.z;
        float elapsed = 0f;

        while (elapsed < snapBackDuration)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, originalPosition, elapsed / snapBackDuration);
            rectTransform.position = new Vector3(rectTransform.position.x, rectTransform.position.y, startZ); // Ensure z position remains unchanged
            elapsed += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = originalPosition;
        rectTransform.position = new Vector3(rectTransform.position.x, rectTransform.position.y, startZ); // Ensure z position remains unchanged
    }
}
