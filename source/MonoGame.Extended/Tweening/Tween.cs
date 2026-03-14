using System;

using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tweening
{
    /// <summary>
    /// Abstract base for a typed tween that animates a value of type <typeparamref name="T"/>
    /// on a target object's member.
    /// </summary>
    /// <typeparam name="T">The value type being animated.</typeparam>
    public abstract class Tween<T> : Tween
        where T : struct
    {
        internal Tween(object target, float duration, float delay, TweenMember<T> member, T endValue)
            : base(target, duration, delay)
        {
            Member = member;
            _endValue = endValue;
        }

        /// <summary>
        /// Gets the <see cref="TweenMember{T}"/> representing the property or field being animated.
        /// </summary>
        public TweenMember<T> Member { get; }

        /// <summary>
        /// Gets the name of the member being animated.
        /// </summary>
        public override string MemberName => Member.Name;

        /// <summary>
        /// The value of the member at the start of the current animation cycle.
        /// </summary>
        protected T _startValue;

        /// <summary>
        /// The target value to animate towards.
        /// </summary>
        protected T _endValue;

        /// <summary>
        /// Captures the current member value as the start value of the animation.
        /// </summary>
        protected override void Initialize()
        {
            _startValue = Member.Value;
        }

        /// <summary>
        /// Swaps the start and end values to reverse the animation direction.
        /// Called at the start of each reversed repeat cycle when <see cref="Tween.IsAutoReverse"/> is enabled.
        /// </summary>
        protected override void Swap()
        {
            _endValue = _startValue;
            Initialize();
        }
    }

    /// <summary>
    /// Abstract base class managing timing, lifecycle, easing, callbacks, and
    /// repeat behavior. Subclasses implement the actual value interpolation.
    /// </summary>
    public abstract class Tween
    {
        internal Tween(object target, float duration, float delay)
        {
            Target = target;
            Duration = duration;
            Delay = delay;
            IsAlive = true;

            _remainingDelay = delay;
        }

        /// <summary>
        /// Gets the object whose member is being animated.
        /// </summary>
        public object Target { get; }

        /// <summary>
        /// Gets the name of the member being animated.
        /// </summary>
        public abstract string MemberName { get; }

        /// <summary>
        /// Gets the total duration of one complete animation cycle, in seconds.
        /// </summary>
        public float Duration { get; }

        /// <summary>
        /// Gets the delay before the animation begins, in seconds.
        /// </summary>
        public float Delay { get; }

        /// <summary>
        /// Gets or sets whether the tween is currently paused. When paused, <see cref="Update"/> has no effect.
        /// </summary>
        public bool IsPaused { get; set; }

        /// <summary>
        /// Gets whether the tween is configured to repeat after completing a cycle.
        /// </summary>
        public bool IsRepeating => _remainingRepeats != 0;

        /// <summary>
        /// Gets whether the tween is configured to repeat indefinitely.
        /// </summary>
        public bool IsRepeatingForever => _remainingRepeats < 0;

        /// <summary>
        /// Gets whether the tween reverses direction on alternate repeat cycles.
        /// </summary>
        public bool IsAutoReverse { get; private set; }

        /// <summary>
        /// Gets whether the tween is active and should continue receiving updates.
        /// </summary>
        public bool IsAlive { get; private set; }

        /// <summary>
        /// Gets whether the tween has completed the current animation cycle.
        /// </summary>
        public bool IsComplete { get; private set; }

        /// <summary>
        /// Gets the time remaining in the current animation cycle, in seconds.
        /// </summary>
        public float TimeRemaining => Duration - _elapsedDuration;

        /// <summary>
        /// Gets the normalized progress of the current animation cycle, clamped to [0, 1].
        /// This value is not affected by easing.
        /// </summary>
        public float Completion => MathHelper.Clamp(_completion, 0, 1);

        private Func<float, float> _easingFunction;
        private bool _isInitialized;
        private float _completion;
        private float _elapsedDuration;
        private float _remainingDelay;
        private float _repeatDelay;
        private int _remainingRepeats;
        private Action<Tween> _onBegin;
        private Action<Tween> _onEnd;
        private Action<Tween> _onUpdate;

        /// <summary>
        /// Sets the easing function applied to the animation progress each frame.
        /// </summary>
        /// <param name="easingFunction">
        /// A function that transforms a linear [0, 1] progress value into a non-linear one.
        /// Use members of <see cref="EasingFunctions"/> or supply a custom delegate.
        /// </param>
        /// <returns>This tween instance, for fluent chaining.</returns>
        public Tween Easing(Func<float, float> easingFunction) { _easingFunction = easingFunction; return this; }

        /// <summary>
        /// Registers a callback to invoke when the animation begins its first cycle.
        /// Cast the <see cref="Tween"/> argument to <see cref="Tween{T}"/> and read
        /// <c>Member.Value</c> to retrieve the typed current value.
        /// </summary>
        /// <param name="action">The callback to invoke, receiving this tween as its argument.</param>
        /// <returns>This tween instance, for fluent chaining.</returns>
        public Tween OnBegin(Action<Tween> action) { _onBegin = action; return this; }

        /// <summary>
        /// Registers a callback to invoke when the animation completes each cycle.
        /// Cast the <see cref="Tween"/> argument to <see cref="Tween{T}"/> and read
        /// <c>Member.Value</c> to retrieve the typed current value.
        /// </summary>
        /// <param name="action">The callback to invoke, receiving this tween as its argument.</param>
        /// <returns>This tween instance, for fluent chaining.</returns>
        public Tween OnEnd(Action<Tween> action) { _onEnd = action; return this; }

        /// <summary>
        /// Registers a callback to invoke after each update step, once the interpolated value
        /// has been applied to the target member.
        /// Cast the <see cref="Tween"/> argument to <see cref="Tween{T}"/> and read
        /// <c>Member.Value</c> to retrieve the typed current value.
        /// </summary>
        /// <param name="action">The callback to invoke, receiving this tween as its argument.</param>
        /// <returns>This tween instance, for fluent chaining.</returns>
        public Tween OnUpdate(Action<Tween> action) { _onUpdate = action; return this; }

        /// <summary>
        /// Pauses the animation. Has no effect if already paused.
        /// </summary>
        /// <returns>This tween instance, for fluent chaining.</returns>
        public Tween Pause() { IsPaused = true; return this; }

        /// <summary>
        /// Resumes a paused animation. Has no effect if not paused.
        /// </summary>
        /// <returns>This tween instance, for fluent chaining.</returns>
        public Tween Resume() { IsPaused = false; return this; }

        /// <summary>
        /// Configures the animation to repeat a fixed number of times after its first cycle.
        /// </summary>
        /// <param name="count">The number of additional cycles to play.</param>
        /// <param name="repeatDelay">An optional delay between cycles, in seconds.</param>
        /// <returns>This tween instance, for fluent chaining.</returns>
        public Tween Repeat(int count, float repeatDelay = 0f)
        {
            _remainingRepeats = count;
            _repeatDelay = repeatDelay;
            return this;
        }

        /// <summary>
        /// Configures the animation to repeat indefinitely.
        /// </summary>
        /// <param name="repeatDelay">An optional delay between cycles, in seconds.</param>
        /// <returns>This tween instance, for fluent chaining.</returns>
        public Tween RepeatForever(float repeatDelay = 0f)
        {
            _remainingRepeats = -1;
            _repeatDelay = repeatDelay;
            return this;
        }

        /// <summary>
        /// Configures the animation to reverse direction on alternate repeat cycles, creating
        /// a ping-pong effect. Implicitly sets the repeat count to at least 1 if not already repeating.
        /// </summary>
        /// <returns>This tween instance, for fluent chaining.</returns>
        public Tween AutoReverse()
        {
            if (_remainingRepeats == 0)
                _remainingRepeats = 1;

            IsAutoReverse = true;
            return this;
        }

        /// <summary>
        /// Called once before the first update to capture the initial animation state.
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        /// Called each update to apply the interpolated value for the given progress.
        /// </summary>
        /// <param name="n">The interpolation progress in the range [0, 1], after easing has been applied.</param>
        protected abstract void Interpolate(float n);

        /// <summary>
        /// Called at the start of a reversed cycle to swap the start and end values.
        /// </summary>
        protected abstract void Swap();

        /// <summary>
        /// Stops the animation immediately without applying the final value.
        /// </summary>
        public void Cancel()
        {
            _remainingRepeats = 0;
            IsAlive = false;
        }

        /// <summary>
        /// Stops the animation and immediately applies the end value, invoking the
        /// <see cref="OnEnd"/> callback if the tween was still alive.
        /// </summary>
        public void CancelAndComplete()
        {
            if (IsAlive)
            {
                _completion = 1;

                Interpolate(1);
                IsComplete = true;
                _onEnd?.Invoke(this);
            }

            Cancel();
        }

        /// <summary>
        /// Advances the animation by the given elapsed time and applies the interpolated value.
        /// Animations that complete and are not configured to repeat are automatically deactivated.
        /// </summary>
        /// <param name="elapsedSeconds">The time elapsed since the last update, in seconds.</param>
        public void Update(float elapsedSeconds)
        {
            if(IsPaused || !IsAlive)
                return;

            if (_remainingDelay > 0)
            {
                _remainingDelay -= elapsedSeconds;

                if (_remainingDelay > 0)
                    return;
            }

            if (!_isInitialized)
            {
                _isInitialized = true;
                Initialize();
                _onBegin?.Invoke(this);
            }

            if (IsComplete)
            {
                IsComplete = false;
                _elapsedDuration = 0;
                _onBegin?.Invoke(this);

                if (IsAutoReverse)
                    Swap();
            }

            _elapsedDuration += elapsedSeconds;

            var n = _completion = _elapsedDuration / Duration;

            if (_easingFunction != null)
                n = _easingFunction(n);

            if (_elapsedDuration >= Duration)
            {
                if (_remainingRepeats != 0)
                {
                    if(_remainingRepeats > 0)
                        _remainingRepeats--;

                    _remainingDelay = _repeatDelay;
                }
                else if (_remainingRepeats == 0)
                {
                    IsAlive = false;
                }

                n = _completion = 1;
                IsComplete = true;
            }

            Interpolate(n);
            _onUpdate?.Invoke(this);

            if (IsComplete)
                _onEnd?.Invoke(this);
        }

    }
}
