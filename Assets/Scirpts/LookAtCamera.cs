using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] Transform lookAtTarget;
    [SerializeField] Transform []videoPlayers;
    private void Awake()
    {
        if (lookAtTarget==null)
            lookAtTarget = transform;
    }
    // Update is called once per frame
    void Update()
    {
        foreach(Transform videoPlayer in videoPlayers)
            videoPlayer.LookAt(lookAtTarget);
    }
}
