using System;

namespace Player
{
    public interface IWeapon
    {
        public event Action OnReloadStart;
        public event Action OnReloadFinished;
        float ReloadTime { get; }

        void HandleShoot();
    }
}
