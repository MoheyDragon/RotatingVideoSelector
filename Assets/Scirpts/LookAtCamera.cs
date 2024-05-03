using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : Singletons<LookAtCamera>
{
    Transform lookAtTarget;
    [SerializeField] Transform[] videoPlayers;
    private void Awake()
    {
        lookAtTarget = Camera.main.transform;
    }
    void Update()
    {
        foreach (Transform videoPlayer in videoPlayers)
            videoPlayer.forward = lookAtTarget.forward;
    }
}
