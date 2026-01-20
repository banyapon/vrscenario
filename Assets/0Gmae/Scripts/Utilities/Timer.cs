using System;
using System.Collections.Generic;
using UnityEngine;

namespace Boy {

    [Serializable]
    public class Timer : MonoBehaviour
    {
        //Setting-----------------------------------
        [SerializeField] private bool running;
        [SerializeField] private bool startOnEnable;
        [SerializeField] private bool forceRemoveTimerEvent;
        public float startingTime;
        public float timeLeft;

        //Action-----------------------------------
        public Action OnStartCount;
        public Action OnTimerCounted;
        private List<TimerEvent> timerEvents = new List<TimerEvent>();

        public bool Running
        {
            get => running;
            set
            {
                running = value;
                if (running) OnStartCount?.Invoke();
            }
        }

        private void Awake()
        {
            Setup(startingTime);
        }

        private void OnEnable()
        {
            if (startOnEnable) ReStart();
        }

        public void Setup(float time)
        {
            timeLeft = time;
            startingTime = time;
            running = false;
        }

        public void StartCount()
        {
            Running = true;
        }

        public void StopCount()
        {
            Running = false;
        }

        public void ResetTime()
        {
            Running = false;
            timeLeft = startingTime;
        }
        public void ReStart()
        {
            Running = false;
            timeLeft = startingTime;
            ResetTimerEventTrigger();
            Running = true;
        }

        public void ChangeTime(float newTime)
        {
            startingTime = newTime;
        }

        public float GetTimerProgress()
        {
            return 1 - (timeLeft / startingTime);
        }

        public void AddTimerEvent(float x, Action callback, bool removeAfterCallback = false)
        {
            timerEvents.Add(new TimerEvent(x, callback, removeAfterCallback));
        }

        public void ResetTimerEventTrigger()
        {
            foreach (var timerEvent in timerEvents)
            {
                timerEvent.hasTriggered = false;
            }
        }

        public void ClearTimerEvent()
        {
            timerEvents.Clear();
        }

        public void Update()
        {
            if (!Running) return;

            timeLeft -= Time.deltaTime;

            if (timeLeft <= 0)
            {
                timeLeft = 0;
                Running = false;
                OnTimerCounted?.Invoke();
            }

            for (int i = timerEvents.Count - 1; i >= 0; i--)
            {
                if (timeLeft <= timerEvents[i].time && !timerEvents[i].hasTriggered)
                {
                    timerEvents[i].callback?.Invoke();
                    timerEvents[i].hasTriggered = true;
                    if (timerEvents[i].removeAfterCallback || forceRemoveTimerEvent) timerEvents.RemoveAt(i);
                }
            }
        }
    }

    [Serializable]
    public class TimerEvent
    {
        public float time;
        public Action callback;
        public bool hasTriggered;
        public bool removeAfterCallback;

        public TimerEvent(float time, Action callback, bool removeAfterCallback)
        {
            this.time = time;
            this.callback = callback;
            this.hasTriggered = false;
            this.removeAfterCallback = removeAfterCallback;
        }
    }
}

