using System;

namespace Player
{
    /// <summary>
    /// Interface for weapons
    /// </summary>
    public interface IWeapon
    {
        public event Action OnReloadStart;
        public event Action OnReloadFinished;
        float ReloadTime { get; }

        void HandleShoot();
    }
}
