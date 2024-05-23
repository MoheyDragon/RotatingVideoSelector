using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideosOrbitRotater : Singletons<VideosOrbitRotater>
{
    [SerializeField] float rotateSpeed = 5;
    float positiveRotateSpeed;
    float negativeRotateSpeed;
    bool autoRotation;
    private void Start()
    {
        SwipeInputHandler.Singleton.OnHorizontalSwipDetected += RotateOrbit;
        positiveRotateSpeed = rotateSpeed;
        negativeRotateSpeed = -rotateSpeed;
        autoRotation=true;
        PositionVideos();
    }
    private void PositionVideos()
    {
        int videosNum = transform.childCount;
        Transform[] videos = new Transform[videosNum];
        for (int i = 0; i < videosNum; i++)
        {
            videos[i]=transform.GetChild(i);
        }
        List<Vector3> cirularPositions = GetCircularPositions(videosNum);
        for (int i = 0; i < videosNum; i++)
        {
            videos[i].localPosition = cirularPositions[i];
        }

    }
    [SerializeField] private float radius = 1f;
    public List<Vector3> GetCircularPositions(int numberOfObjects)
    {
        List<Vector3> positions = new List<Vector3>();
        float angleStep = 360f / numberOfObjects; 

        for (int i = 0; i < numberOfObjects; i++)
        {
            float angle = i * angleStep;
            float radian = angle * Mathf.Deg2Rad;
            float x = radius * Mathf.Cos(radian);
            float z = radius * Mathf.Sin(radian);
            positions.Add(new Vector3(x, 0.056f, z));
        }

        return positions;
    }
    private void RotateOrbit(Swipe swipe)
    {
        StartCoroutine(CO_ForcedRotation(swipe));
    }
    IEnumerator CO_ForcedRotation(Swipe swipe)
    {
        Quaternion targetRotation = Quaternion.Euler(0, swipe.direction * 90 * -1, 0) * transform.rotation;
        Quaternion startRotation = transform.rotation;

        float totalRotationTime = swipe.duration;
        float rotationSpeedFactor = 1f / totalRotationTime;
        float elapsedTime = 0;

        while (elapsedTime < totalRotationTime)
        {
            float t = Mathf.Sin(elapsedTime *rotationSpeedFactor * Mathf.PI * 0.5f);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
        HandleRotationDircetionAfterUserSwipe(swipe.direction);
        
    }
    private void HandleRotationDircetionAfterUserSwipe(float direction)
    {
        if (direction == 1)
            rotateSpeed = negativeRotateSpeed;
        else
            rotateSpeed = positiveRotateSpeed;
        autoRotation = true;
    }

    public void StopRotating(bool stop=true)
    {
        autoRotation = !stop;
    }
    void Update()
    {
        if (autoRotation)
            transform.Rotate(Vector3.up, Time.deltaTime * rotateSpeed);
    }
}
