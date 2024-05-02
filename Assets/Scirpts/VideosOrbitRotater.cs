using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideosOrbitRotater : MonoBehaviour
{
    public float rotateSpeed = 5;
    void Update()
    {
        transform.Rotate(Vector3.up, Time.deltaTime * rotateSpeed);
    }
}
