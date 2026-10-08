using System;
using System.Collections.Generic;
using UnityEngine;

namespace BillGameCore
{
    public sealed class TweenSequence
    {
        private enum StepType : byte { Tween, Interval, Callback }

        private struct Step
        {
            public StepType Type;
            public Tween Tween;          // for Tween steps
            public float Interval;       // for Interval steps
            public Action Callback;      // for Callback steps
            public bool IsJoined;        // runs parallel with previous
        }

        private readonly List<Step> _steps = new(8);
        private int _current;
        private float _intervalElapsed;
        private bool _stepStarted;       // joined steps of the current group were collected
        private bool _mainDone;          // the group's leading step finished
        private int _groupEnd;           // index of the first step after the current group
        private bool _alive;
        private bool _useUnscaled;
        private int _loopTotal;          // 0 = play once, -1 = infinite
        private Action _onComplete;
        private Action<int> _onStepComplete;

        // Active joined tweens running in parallel
        private readonly List<Tween> _activeJoined = new(4);

        internal bool IsAlive => _alive;

        internal TweenSequence()
        {
            _alive = true;
            _current = 0;
        }

        // -------------------------------------------------------
        // Builder API
        // -------------------------------------------------------

        /// <summary>Add a tween that plays after the previous step finishes.</summary>
        public TweenSequence Append(Tween tween)
        {
            if (tween == null) return this;
            tween.InSequence = true;
            tween.State = TweenState.Running;
            _steps.Add(new Step { Type = StepType.Tween, Tween = tween, IsJoined = false });
            return this;
        }

        /// <summary>Add a tween that plays in parallel with the previous Append.</summary>
        public TweenSequence Join(Tween tween)
        {
            if (tween == null) return this;
            tween.InSequence = true;
            tween.State = TweenState.Running;
            _steps.Add(new Step { Type = StepType.Tween, Tween = tween, IsJoined = true });
            return this;
        }

        /// <summary>Wait for a duration before the next step.</summary>
        public TweenSequence AppendInterval(float seconds)
        {
            _steps.Add(new Step { Type = StepType.Interval, Interval = seconds, IsJoined = false });
            return this;
        }

        /// <summary>Fire a callback between steps.</summary>
        public TweenSequence AppendCallback(Action callback)
        {
            _steps.Add(new Step { Type = StepType.Callback, Callback = callback, IsJoined = false });
            return this;
        }

        /// <summary>Insert a tween at a specific time position (sugar for delay).</summary>
        public TweenSequence Insert(float atTime, Tween tween)
        {
            if (tween == null) return this;
            tween.InSequence = true;
            tween.SetDelay(atTime);
            tween.State = TweenState.Delayed;
            _steps.Add(new Step { Type = StepType.Tween, Tween = tween, IsJoined = true });
            return this;
        }

        public TweenSequence SetLoops(int count)
        {
            _loopTotal = count;
            return this;
        }

        public TweenSequence SetUnscaled()
        {
            _useUnscaled = true;
            return this;
        }

        public TweenSequence OnComplete(Action cb)
        {
            _onComplete = cb;
            return this;
        }

        public TweenSequence OnStepComplete(Action<int> cb)
        {
            _onStepComplete = cb;
            return this;
        }

        public void Kill()
        {
            _alive = false;
            // Kill all tweens still referenced
            for (int i = 0; i < _steps.Count; i++)
            {
                var s = _steps[i];
                if (s.Type == StepType.Tween && s.Tween != null)
                    s.Tween.Kill();
            }
            _activeJoined.Clear();
        }

        // -------------------------------------------------------
        // Tick - called by TweenService
        // -------------------------------------------------------

        internal bool HasTarget(object target)
        {
            for (int i = 0; i < _steps.Count; i++)
            {
                var s = _steps[i];
                if (s.Type == StepType.Tween && s.Tween != null && ReferenceEquals(s.Tween.Owner, target))
                    return true;
            }
            return false;
        }

        // A group is one leading step plus the Join/Insert steps right after it. Every tween in the
        // group starts together; the next Append starts once the whole group has finished.
        internal bool Tick(float dt)
        {
            if (!_alive) return false;
            float delta = _useUnscaled ? Time.unscaledDeltaTime : dt;

            while (true)
            {
                if (_current >= _steps.Count)
                {
                    TickJoined(delta);
                    if (_activeJoined.Count > 0) return true;
                    return HandleSequenceEnd();
                }

                if (!_stepStarted) StartGroup();

                if (!_mainDone) _mainDone = ProcessStep(_current, delta);
                TickJoined(delta);

                if (!_mainDone || _activeJoined.Count > 0) return true;

                try { _onStepComplete?.Invoke(_current); } catch (Exception e) { Debug.LogException(e); }
                _current = _groupEnd;
                _stepStarted = false;
                _mainDone = false;
                _intervalElapsed = 0f;

                // Callbacks and zero-length steps chain within the same frame; anything with
                // duration starts next frame.
                delta = 0f;
                if (_current < _steps.Count && _steps[_current].Type != StepType.Callback) return true;
            }
        }

        private void StartGroup()
        {
            _stepStarted = true;
            _groupEnd = _current + 1;
            while (_groupEnd < _steps.Count && _steps[_groupEnd].IsJoined)
            {
                var joined = _steps[_groupEnd];
                if (joined.Type == StepType.Tween && joined.Tween != null)
                    _activeJoined.Add(joined.Tween);
                else if (joined.Type == StepType.Callback)
                {
                    try { joined.Callback?.Invoke(); } catch (Exception e) { Debug.LogException(e); }
                }
                _groupEnd++;
            }
        }

        private void TickJoined(float delta)
        {
            for (int i = _activeJoined.Count - 1; i >= 0; i--)
            {
                if (!_activeJoined[i].Tick(delta))
                    _activeJoined.RemoveAt(i);
            }
        }

        private bool ProcessStep(int index, float delta)
        {
            var step = _steps[index];

            switch (step.Type)
            {
                case StepType.Tween:
                    return !step.Tween.Tick(delta);

                case StepType.Interval:
                    _intervalElapsed += delta;
                    return _intervalElapsed >= step.Interval;

                case StepType.Callback:
                    try { step.Callback?.Invoke(); } catch (Exception e) { Debug.LogException(e); }
                    return true;

                default:
                    return true;
            }
        }

        private bool HandleSequenceEnd()
        {
            if (_loopTotal == 0)
            {
                _alive = false;
                try { _onComplete?.Invoke(); } catch (Exception e) { Debug.LogException(e); }
                return false;
            }

            // Loop: restart
            if (_loopTotal > 0) _loopTotal--;
            _current = 0;
            _intervalElapsed = 0f;
            _stepStarted = false;
            _mainDone = false;

            // Reset all tweens
            for (int i = 0; i < _steps.Count; i++)
            {
                var s = _steps[i];
                if (s.Type == StepType.Tween && s.Tween != null)
                {
                    s.Tween.Elapsed = 0f;
                    s.Tween.State = TweenState.Running;
                }
            }

            return true;
        }
    }
}
