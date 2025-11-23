using Resource.Script.Controller;
using UnityEngine;

namespace Resources.Script.Controller
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