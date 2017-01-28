using System;

namespace Demo.Platformer.Entities.Components
{
    public sealed class CharacterState
    {
        public event Action OnKilled;
        public event Action OnJumped;

        private int _healthPoints = 3;
        public int HealthPoints
        {
            get { return _healthPoints; }
            set
            {
                _healthPoints = value;
                if (!IsAlive)
                    OnKilled?.Invoke();
            }
        }

        private bool _isJumping = false;
        public bool IsJumping
        {
            get { return _isJumping; }
            set
            {
                if (value != _isJumping)
                {
                    OnJumped?.Invoke();
                    _isJumping = value;
                }
            }
        }

        public bool IsAlive => HealthPoints > 0;
    }
}