using System;
using UnityEngine;

public class ThrowableObjectsHandler : Singletons<ThrowableObjectsHandler>
{
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
    float  orbitalYPostion= 0.056f;
    const string throwableObjectTag = "throwableObject";
    private GameObject currentTouchedObject;
    private Camera mainCamera;

    protected override void Awake()
    {
        base.Awake();
        mainCamera = Camera.main; ;
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
        if (currentTouchedObject == null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.CompareTag(throwableObjectTag))
                {
                    OnObjectClicked(hit.collider.gameObject);
                }
            }
        }
    }
    private void OnObjectClicked(GameObject throwableObject)
    {
        orbitalYPostion = throwableObject.transform.localPosition.y;
        SwipeInputHandler.isTouching = false;
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
        LeanTween.moveLocalY(currentTouchedObject, orbitalYPostion, returnDuration).setOnComplete(() =>
        {
            isMovingVideos = false;
            currentTouchedObject = null;
            SwipeInputHandler.isTouching = true;
            VideosOrbitRotater.Singleton.StopRotating(false);
        });
    }
}
