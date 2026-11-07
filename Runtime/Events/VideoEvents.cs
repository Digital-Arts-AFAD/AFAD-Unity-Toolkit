using DigitalArtsAFAD;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class VideoEvents : MonoBehaviour
{
    [Tooltip("Reference to the monitored video player.")]
    public VideoPlayer videoPlayer;

    [Space(5)]
    public UnityEvent onStarted, onFinished;

    private void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.started += Player_started;
            videoPlayer.loopPointReached += Player_loopPointReached;
        }
    }

    private void Update()
    {
        double pos = videoPlayer.time;
    }

    private void Player_loopPointReached(VideoPlayer source)
    {
        onFinished.Invoke(); 
    }

    private void Player_started(VideoPlayer source)
    {
        onStarted.Invoke();
    }
}
