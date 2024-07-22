using Unity.Plastic.Newtonsoft.Json.Serialization;

namespace GameProg.Player
{
    public interface IWeapon
    {
        public event Action OnReloadStart;
        
        void HandleShoot();
    }
}
