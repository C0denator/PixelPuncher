using Unity.Plastic.Newtonsoft.Json.Serialization;

namespace GameProg.Player
{
    public interface IWeapon
    {
        public event Action OnReloadStart;
        public event Action OnReloadFinished;
        float ReloadTime { get; }

        void HandleShoot();
    }
}
