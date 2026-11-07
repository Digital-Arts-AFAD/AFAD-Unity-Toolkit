using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DigitalArtsAFAD
{
    public enum FadePhase { None, FadeOut, FadeStay, FadeIn };

    public class Fader : MonoBehaviour
    {
        public Image image;

        [Header("Fade Out")]
        public Color fadeOutStart = new Color(0, 0, 0, 0);
        public Color fadeOutEnd = new Color(0, 0, 0, 1);
        public float fadeOutDuration = 1f;

        [Header("Fade Stay")]
        public float fadeStayTime = 0.25f;

        [Header("Fade In")]
        public Color fadeInStart = new Color(0, 0, 0, 1);
        public Color fadeInEnd = new Color(0, 0, 0, 0);
        public float fadeInDuration = 1f;
        public bool fadeInAutomatically = false;

        [Header("Fader events")]
        [Space(5)]
        public UnityEvent OnFadeOutStart;
        public UnityEvent OnFadeOutEnd;
        public UnityEvent OnFadeInStart;
        public UnityEvent OnFadeInEnd;

        public FadePhase phase { get { return _phase; } }
        FadePhase _phase = FadePhase.None;
        float timer = 0;

        bool automaticFadeIn = false;

        private void Awake()
        {
            if (image == null)
            {
                image = GetComponentInChildren<Image>();
            }
        }

        // Start is called before the first frame update
        void Start()
        {            
            if (fadeInAutomatically)
            {
                FadeIn();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (_phase == FadePhase.None) return;

            timer += Time.deltaTime;

            if (_phase == FadePhase.FadeOut)
            {
                if (timer > fadeOutDuration)
                {
                    image.color = fadeOutEnd;

                    if (automaticFadeIn)
                    {
                        automaticFadeIn = false;
                        timer -= fadeOutDuration;
                        _phase = FadePhase.FadeStay;
                    }
                    else
                    {
                        _phase = FadePhase.None;
                    }

                    OnFadeOutEnd.Invoke();
                }
                else
                {
                    image.color = Color.Lerp(fadeOutStart, fadeOutEnd, timer / fadeOutDuration);
                }

            }

            if (_phase == FadePhase.FadeStay)
            {
                if (timer > fadeStayTime)
                {
                    timer -= fadeStayTime;
                    _phase = FadePhase.FadeIn;
                    OnFadeInStart.Invoke();
                }
            }

            if (_phase == FadePhase.FadeIn)
            {
                if (timer > fadeInDuration)
                {
                    image.color = fadeInEnd;
                    _phase = FadePhase.None;
                    OnFadeInEnd.Invoke();
                }
                else
                {
                    image.color = Color.Lerp(fadeInStart, fadeInEnd, timer / fadeInDuration);
                }
            }
        }

        public void FadeOut()
        {
            timer = 0;
            _phase = FadePhase.FadeOut;
            OnFadeOutStart.Invoke();
        }
        public void FadeIn()
        {
            timer = 0;
            _phase = FadePhase.FadeIn;
            OnFadeInStart.Invoke();
        }

        public void Fade()
        {
            automaticFadeIn = true;
            FadeOut();
        }

        /// <summary>
        /// Sets color to the end of fade out. Used e.g. when additive loading the first scene 
        /// and the fader needs to be filled with color because fade in doesn't start
        /// until the scene finished loading. Without Fill(), the world would be empty and visible.
        /// </summary>
        public void Fill()
        {
            image.color = fadeOutEnd;
        }
    }
}