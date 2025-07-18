﻿using System;
using UnityEngine;

namespace Assets.Scripts.Util
{
    public class GameTimer
    {
        private TimeSpan m_timerSpan = TimeSpan.MaxValue;
        private float m_currSeconds;
        private bool m_startTimer;
        private bool m_elapsed;

        private bool m_autoReset;

        public event EventHandler<TimerElapsedEventArgs> OnTimerElapsed;

        public bool Started
        {
            get
            {
                return m_currSeconds > 0;
            }
        }

        public bool AutoReset
        {
            get
            {
                return m_autoReset;
            }
            set
            {
                m_autoReset = value;
            }
        }

        public bool Elapsed
        {
            get
            {
                return m_elapsed;
            }
        }

        public GameTimer()
        {
            m_autoReset = true;
            m_elapsed = false;
        }

        public GameTimer(TimeSpan timeSpan)
            : this()
        {
            SetTimeSpan(timeSpan);
        }

        public void Start()
        {
            m_startTimer = true;

        }

        public void Stop()
        {
            m_startTimer = false;
        }

        public void Tick()
        {
            if (m_startTimer)
            {
                m_currSeconds += Time.deltaTime;
            }

            if (TimeSpan.FromSeconds(m_currSeconds) > m_timerSpan)
            {
                OnTimerElapsed?.Invoke(this, new TimerElapsedEventArgs());
                m_elapsed = true;

                if (m_autoReset)
                {
                    ResetTimer();
                }
            }
        }

        public void SetTimeSpan(TimeSpan timeSpan, bool reset = true)
        {
            m_timerSpan = timeSpan;

            if (reset)
            {
                ResetTimer();
            }
        }

        public void ResetTimer()
        {
            m_currSeconds = 0.0f;
            m_elapsed = false;
        }

    }
}
