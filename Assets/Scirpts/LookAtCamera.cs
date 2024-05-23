using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : Singletons<LookAtCamera>
{
    Transform lookAtTarget;
    Transform[] videoPlayers;
    protected override void Awake()
    {
        base.Awake();
        lookAtTarget = Camera.main.transform;
    }
    private void Start()
    {
        SetVideoPlayersTransforms();
    }
    private void SetVideoPlayersTransforms()
    {
        Transform videosParent= VideosOrbitRotater.Singleton.transform;
        int videosCount = videosParent.childCount;
        videoPlayers = new Transform[videosCount];
        for (int i = 0; i < videosCount; i++)
        {
            videoPlayers[i] = videosParent.GetChild(i).transform;
        }
    }
    void Update()
    {
        foreach (Transform videoPlayer in videoPlayers)
            videoPlayer.forward = lookAtTarget.forward;
    }
}
