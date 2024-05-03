using UnityEngine.Video;
using UnityEngine;
public class SecondScreenManager : Singletons<SecondScreenManager>
{
    [SerializeField] VideoPlayer contentVideoPlayer;
    [SerializeField] VideoClip[] videoClips;
    [SerializeField] float fadingVideoDuration = 1;
    [SerializeField] CanvasGroup idleVideoCG, contentVideoCG;
    private void Start()
    {
        ThrowableObjectsHandler.Singleton.OnVideoThrown += RecieveVideoAndPlayIt;
        contentVideoPlayer.loopPointReached += FadeInIdleVideo;
    }

    private void FadeInIdleVideo(VideoPlayer source)
    {
        LeanTween.alphaCanvas(contentVideoCG, 0, fadingVideoDuration);
        LeanTween.alphaCanvas(idleVideoCG, 1, fadingVideoDuration);
    }

    private void RecieveVideoAndPlayIt(int videoIndex)
    {
        LeanTween.alphaCanvas(contentVideoCG, 1, fadingVideoDuration);
        LeanTween.alphaCanvas(idleVideoCG, 0, fadingVideoDuration);
        contentVideoPlayer.clip = videoClips[videoIndex];
        contentVideoPlayer.Play();
    }
}
