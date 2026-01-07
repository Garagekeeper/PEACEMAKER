using Resources.Script.InteractiveObject;
using UnityEngine;

namespace Resources.Script.Creatures
{
    public class DetectPickup : MonoBehaviour
    {
        
        private void OnTriggerEnter(Collider other)
        {
            // 상대가 Pickup 아이템이고, 내가 픽업을 수집하면
            if (other.transform.TryGetComponent<IPickup>(out var pickup))
            {
                pickup.SetAttract(true);
            }
        }
    }
}