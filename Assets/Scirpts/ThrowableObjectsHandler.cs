using System;
using UnityEngine;

public class ThrowableObjectsHandler : Singletons<ThrowableObjectsHandler>
{
    [SerializeField] float smoothTouchFollowing = 0.05f;
    [SerializeField] float throwVideoDuration = 1.5f;
    [SerializeField] float throwAcceptedY;
    [SerializeField] float throwVideoY;
    public Action<int> OnVideoThrown;
    const float  orbitalYPostion= 0.056f;
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
                if (Input.mousePosition.y > throwAcceptedY)
                    ThrowVideoToPlayer();
                else
                    ReturnVideoToOrbit();
            }
            if (!isMovingVideos)
                if (currentTouchedObject != null)
            {
                Vector3 mousePositionScreenSpace = Input.mousePosition;
                Vector3 mousePositionWorldSpace = mainCamera.ScreenToWorldPoint(new Vector3(
                    mousePositionScreenSpace.x, mousePositionScreenSpace.y, mainCamera.transform.position.y));
                currentTouchedObject.transform.LeanMoveY(mousePositionWorldSpace.y, smoothTouchFollowing);
            }
        }
        if (SwipeInputHandler.isTouching) return;
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.CompareTag(throwableObjectTag))
            {
                OnObjectClicked(hit.collider.gameObject);
            }
        }
    }
    private void OnObjectClicked(GameObject throwableObject)
    {
        SwipeInputHandler.isTouching = false;
        VideosOrbitRotater.Singleton.StopRotating();
        currentTouchedObject = throwableObject;
    }
    private void ThrowVideoToPlayer()
    {
        isMovingVideos = true;
        LeanTween.moveY(currentTouchedObject, throwVideoY, throwVideoDuration).setOnComplete(() =>
        {
            int videoIndex = currentTouchedObject.transform.GetSiblingIndex();
            OnVideoThrown?.Invoke(videoIndex);
            ReturnVideoToOrbit();
        });
    }

    private void ReturnVideoToOrbit()
    {
        isMovingVideos = true;
        LeanTween.moveY(currentTouchedObject, orbitalYPostion, smoothTouchFollowing).setOnComplete(() =>
        {
            isMovingVideos = false;
            print("returned");
            currentTouchedObject = null;
            SwipeInputHandler.isTouching = true;
            VideosOrbitRotater.Singleton.StopRotating(false);
        });
    }
}
