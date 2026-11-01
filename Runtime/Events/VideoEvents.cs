using DigitalArtsAFAD;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class VideoEvents : MonoBehaviour
{
    [Tooltip("Reference to the monitored video player.")]
    public VideoPlayer player;

    public UnityEvent onStarted, onFinished;

    private void Start()
    {
        if (player != null)
        {
            player.started += Player_started;
            player.loopPointReached += Player_loopPointReached;
        }
    }

    private void Update()
    {
        double pos = player.time;
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
