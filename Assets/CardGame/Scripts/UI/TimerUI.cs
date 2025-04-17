using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CardGame.Scripts.UI
{
    /// <summary>
    /// TimerUI handle time for player to play card
    /// Start, Reset, TimeUp
    /// </summary>
    public class TimerUI : MonoBehaviour
    {
        [SerializeField] private Slider countdownSlider;
        public float countdownTime = 10f;
        public UnityEvent  onTimerComplete;
    
        private float _currentTime;
        private Coroutine _countdownCoroutine;
        private bool _isTimerRunning;
        
        /// <summary>
        /// Starts the countdown timer
        /// </summary>
        public void StartTimer()
        {
            if (_isTimerRunning)
            {
                StopCoroutine(_countdownCoroutine);
            }

            _countdownCoroutine = StartCoroutine(CountdownToStart());
            _isTimerRunning = true;
        }

        /// <summary>
        /// Stops the timer without resetting it
        /// </summary>
        public void StopTimer()
        {
            if (_countdownCoroutine != null)
            {
                StopCoroutine(_countdownCoroutine);
                _isTimerRunning = false;
            }
        }
        
        /// <summary>
        /// Resets the timer to full duration
        /// </summary>
        public void ResetTimer()
        {
            StopTimer();
            _currentTime = countdownTime;
            UpdateCountdownSlider(_currentTime);
        }

        private IEnumerator CountdownToStart()
        {
            _currentTime = countdownTime;

            while (_currentTime > 0)
            {
                UpdateCountdownSlider(_currentTime);
                yield return new WaitForSeconds(1f);
                _currentTime--;
            }
        
            _isTimerRunning = false;
            onTimerComplete?.Invoke();
            UpdateCountdownSlider(0);
        }

        private void UpdateCountdownSlider(float time)
        {
            if (countdownSlider != null)
            {
                float sliderValue = time / countdownTime;
                countdownSlider.value = sliderValue;
            }
        }
    
        // for future use
        // private bool _isPaused;
        //
        // public void PauseTimer()
        // {
        //     _isPaused = true;
        // }
        //
        // public void ResumeTimer()
        // {
        //     _isPaused = false;
        // }
    }
}