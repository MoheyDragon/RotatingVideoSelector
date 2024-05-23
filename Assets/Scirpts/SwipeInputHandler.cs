using System;
using UnityEngine;

public class SwipeInputHandler : Singletons<SwipeInputHandler>
{
    public Action <Swipe> OnHorizontalSwipDetected;
    public Action <Swipe>OnVerticalSwipDetected;
    [SerializeField] float verticalSwipeThreshold;
    [SerializeField] float horizontalSwipeThreshold;
    public bool isTouching;
    Vector2 startPos;
    Vector2 endPos;
    float touchingStartingTime;
    void Update()
    {
        GetInputData();
    }
    private void GetInputData()
    {
        if (!isTouching)
        {
            if(Input.GetMouseButtonDown(0))
            {
                isTouching = true;
                startPos = Input.mousePosition;
                touchingStartingTime= Time.time;
            }
        }
        if (isTouching && Input.GetMouseButtonUp(0))
        {
            endPos = Input.mousePosition;
            CheckSwiping();
            isTouching = false;
        }
    }
    private void CheckSwiping()
    {
        float horizontalSwipeDistance = Mathf.Abs(endPos.x- startPos.x);
        float verticalSwipeDistance = Mathf.Abs(endPos.y - startPos.y);
        float swipeDuration = Time.time - touchingStartingTime;

        if (horizontalSwipeDistance>horizontalSwipeThreshold)
        {
            float direction = endPos.x > startPos.x ? 1 : -1;
            OnHorizontalSwipDetected?.Invoke(new Swipe(direction,horizontalSwipeDistance,swipeDuration));
        }
        else if(verticalSwipeDistance>verticalSwipeThreshold)
        {
            float direction = endPos.y > startPos.y ? 1 : -1;
            OnVerticalSwipDetected?.Invoke(new Swipe(direction, verticalSwipeDistance, swipeDuration));
        }
    }
}
