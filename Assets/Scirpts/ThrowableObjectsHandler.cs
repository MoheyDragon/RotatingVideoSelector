using System;
using System.Collections;
using UnityEngine;

public class ThrowableObjectsHandler : Singletons<ThrowableObjectsHandler>
{
    [SerializeField] float holdThresholdDuration=.5f;
    WaitForSeconds holdThreshold;
    [SerializeField] float smoothTouchFollowing = 0.05f;
    [Space]
    [SerializeField] float throwVideoThreshold;
    [SerializeField] float throwVideoDuration = 1.5f;
    [SerializeField] float throwTargetYPosition;
    [Space]
    [SerializeField] float returnAfterThrowDelay;
    [SerializeField] float returnAfterThrowDuration;
    [Space]
    [SerializeField] float returnOnFailedSwipeDuration;
    public Action<int> OnVideoThrown;
    Vector3 positionInOrbit;
    const string throwableObjectTag = "throwableObject";
    private GameObject currentTouchedObject;
    private GameObject possibleTouchedObject;
    private Camera mainCamera;

    protected override void Awake()
    {
        base.Awake();
        mainCamera = Camera.main; ;
        holdThreshold= new WaitForSeconds(holdThresholdDuration);
    }
    bool isMovingVideos;
    void Update()
    {
        if (!isMovingVideos)
        {
            if (Input.GetMouseButtonUp(0) && currentTouchedObject)
            {
                if (Input.mousePosition.y > throwVideoThreshold)
                    ThrowVideoToSecondScreen();
                else
                    ReturnVideoOnFailedSwipe();
            }
            if (!isMovingVideos)
            {
                if (currentTouchedObject != null)
                {
                    Vector3 mousePositionScreenSpace = Input.mousePosition;
                    Vector3 mousePositionWorldSpace = mainCamera.ScreenToWorldPoint(new Vector3(
                    mousePositionScreenSpace.x, mousePositionScreenSpace.y, mainCamera.transform.position.y));
                    currentTouchedObject.transform.LeanMoveY(mousePositionWorldSpace.y, smoothTouchFollowing);
                }
            }
        }
        if (currentTouchedObject == null&&!isWaiting)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.CompareTag(throwableObjectTag))
                {
                    possibleTouchedObject = hit.collider.gameObject;
                    StartCoroutine(CO_StartWaitForSelectingVideo());
                }
            }
        }
    }
    bool isWaiting;
    IEnumerator CO_StartWaitForSelectingVideo()
    {
        isWaiting = true;
        yield return holdThreshold;
        isWaiting = false;
        if(Input.GetMouseButton(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.CompareTag(throwableObjectTag))
            {
                if(possibleTouchedObject == hit.collider.gameObject)
                    OnObjectClicked(hit.collider.gameObject);
            }
        }
    }
    private void OnObjectClicked(GameObject throwableObject)
    {
        positionInOrbit = throwableObject.transform.localPosition;
        SwipeInputHandler.Singleton.isTouching = true;
        VideosOrbitRotater.Singleton.StopRotating();
        currentTouchedObject = throwableObject;
    }
    private void ThrowVideoToSecondScreen()
    {
        isMovingVideos = true;
        LeanTween.moveLocalY(currentTouchedObject, throwTargetYPosition, throwVideoDuration).setOnComplete(() =>
        {
            int videoIndex = currentTouchedObject.transform.GetSiblingIndex();
            OnVideoThrown?.Invoke(videoIndex);
            Invoke(nameof(ReturnVideoAfterThrow), returnAfterThrowDelay);
        });
    }
    private void ReturnVideoAfterThrow()
    {
        ReturnVideoToOrbit(returnAfterThrowDuration);
    }
    private void ReturnVideoOnFailedSwipe()
    {
        ReturnVideoToOrbit(returnOnFailedSwipeDuration);
    }
    private void ReturnVideoToOrbit(float returnDuration)
    {
        isMovingVideos = true;
        LeanTween.moveLocal(currentTouchedObject, positionInOrbit, returnDuration).setOnComplete(() =>
        {
            isMovingVideos = false;
            currentTouchedObject = null;
            SwipeInputHandler.Singleton.isTouching = false;
            VideosOrbitRotater.Singleton.StopRotating(false);
        });
    }
}
