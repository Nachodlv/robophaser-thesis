using System;
using System.Collections;
using Photon;
using TMPro;
using UnityEngine;

namespace UI.Combat
{
    public class WaitTimeUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timeDisplay;
        [SerializeField] private string messageAtEnd = "Battle!";
        [SerializeField] private float timeMessageAtEnd = 2f;
        [SerializeField] private Animator animator;


        private WaitForSeconds _waitOneSecond;
        private WaitForSeconds _waitMessageAtEnd;
        private Func<int, IEnumerator> _startCountdownCouroutineCached;
        private static readonly int Show = Animator.StringToHash("show");
        private static readonly int Hide = Animator.StringToHash("hide");

        private void Awake()
        {
            _waitOneSecond = new WaitForSeconds(1);
            _waitMessageAtEnd = new WaitForSeconds(timeMessageAtEnd);
            _startCountdownCouroutineCached = Countdown;
            timeDisplay.gameObject.SetActive(false);
        }

        private void Start()
        {
            CountdownManager.Instance.OnStartCountdown += StartCountdown;
        }

        private void StartCountdown(float time)
        {
            StartCoroutine(_startCountdownCouroutineCached((int) Math.Ceiling(time)));
        }

        private IEnumerator Countdown(int time)
        {
            timeDisplay.gameObject.SetActive(true);
            timeDisplay.text = time.ToString();
            var timeRemaining = time - 1;
            while (timeRemaining > 0)
            {
                yield return _waitOneSecond;
                animator.SetTrigger(Show);
                timeDisplay.text = timeRemaining.ToString();
                timeRemaining--;
                animator.SetTrigger(Hide);
            }
            yield return _waitOneSecond;
            animator.SetTrigger(Show);
            timeDisplay.text = messageAtEnd;
            yield return _waitMessageAtEnd;
            animator.SetTrigger(Hide);
        }

    }
}
