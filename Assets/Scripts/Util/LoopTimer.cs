using UnityEngine;

namespace Util
{

    /// <summary>
    /// Simple looping timer that can be run from a client's Update.
    /// </summary>
    public struct LoopTimer
    {
        private float _loopDuration;
        /// <summary>
        /// Gets or sets the duration of the loop (before factoring in variation).
        /// </summary>
        public float LoopDuration
        {
            get { return _loopDuration; }
            set
            {
                if (_loopDuration != value)
                {
                    _loopDuration = value;
                    _currentLoopDuration = RecalculateLoopDuration();
                }
            }
        }
        
        private float _variation;
        /// <summary>
        /// Percentage time variation on either side of the time.
        /// </summary>
        public float Variation { get { return _variation; } set { _variation = value; } }

        private float _currentLoopDuration;
        
        private float _currentTime;
        /// <summary>
        /// Gets or sets the current time in the loop.
        /// </summary>
        public float CurrentTime { get { return _currentTime; } set { _currentTime = value; } }

        private bool _running;
        /// <summary>
        /// Gets if the timer is running or starts/stops the timer.
        /// </summary>
        public bool Running { get { return _running; } set { _running = value; } }

        /// <summary>
        /// Call this function in Update. Returns the number of times we have looped in
        /// the current frame. Will also return 0 if the timer was not running.
        /// </summary>
        public int Update(float deltaTime)
        {
            int loopCount;
            if (_running)
            {
                _currentTime += deltaTime;

                if (_currentTime >= _currentLoopDuration)
                {
                    //calculate how many loops have happened since last time
                    loopCount = (int)(_currentTime / _currentLoopDuration);

                    // Calculate how much we overshot the last loop and start the timer for the next loop with that duration having already passed.
                    _currentTime -= loopCount * _currentLoopDuration;

                    // Set a new duration for the next loop to factor in our variance parameter.
                    float nextLoopDuration = RecalculateLoopDuration();

                    // In a case where the duration of our next loop is now shorter than our current time,
                    // we mod the current time again to avoid looping on 2 frames in a row.
                    if (_currentTime >= nextLoopDuration)
                    {
                        int additionalLoopCount = (int)(_currentTime / nextLoopDuration);
                        _currentTime -= additionalLoopCount * nextLoopDuration;
                        loopCount += additionalLoopCount;
                    }

                    _currentLoopDuration = nextLoopDuration;
                }
                else
                {
                    loopCount = 0;
                }
            }
            else
            {
                loopCount = 0;
            }

            return loopCount;
        }

        /// <summary>
        /// Sets the current time to a random point within the loop.
        /// </summary>
        public void SetRandomTime()
        {
            _currentTime = Random.Range(0.0f, _currentLoopDuration);
        }
        
        /// <summary>
        /// Determines the duration for the current loop.
        /// </summary>
        private float RecalculateLoopDuration()
        {
            return _loopDuration + (_loopDuration * Random.Range(-_variation, _variation));
        }
    }
}