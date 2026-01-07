using Resources.Script.InteractiveObject;

namespace Resources.Script.Creatures
{
    public interface IPickupCollector
    {
        void Collect(IPickup pickup);
    }
}