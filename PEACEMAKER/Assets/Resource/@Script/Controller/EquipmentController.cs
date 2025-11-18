using UnityEngine;
using Resource.Script.Managers;

namespace Resource.Script.Controller
{
    public class EquipmentController : MonoBehaviour
    {
        private FirearmController _firearmController;
        public FirearmController CurrentFirearm
        {
            get => _firearmController;
            set
            {
                _firearmController?.gameObject.SetActive(false);
                _firearmController = value;
            }
        }

    }
}