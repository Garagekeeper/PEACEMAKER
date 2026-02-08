using Resources.Script.Controller;
using Resources.Script.Inventory;

namespace Resources.Script.UI.Firearm
{
    public class FirearmHUDPresenter : Presenter<FirearmHUD, InventoryCore>
    {
        private FirearmController _activeFirearmController;
        
        public FirearmHUDPresenter(FirearmHUD view, InventoryCore model) : base(view, model)
        {
            
        }

        public override void Init()
        {
            model.OnItemSwapped += HandleWeaponSwap;
        }

        private void HandleWeaponSwap(FirearmController newFirearmController)
        {
            //0. 기존 이벤트 해제
            if (_activeFirearmController != null)
                _activeFirearmController.OnAmmoChanged -= UpdateAmmoUI;
            
            //1. 새 무기로 교체
            _activeFirearmController = newFirearmController;

            if (_activeFirearmController != null)
            {
                //2. 새 무기의 이벤트 구독
                _activeFirearmController.OnAmmoChanged += UpdateAmmoUI;
                
                //3. UI갱신
                UpdateAmmoUI(_activeFirearmController.AmmoInMagazine, _activeFirearmController.ammoItemInInventory.Count, _activeFirearmController.fireArmData.magazineCapacity);
                UpdateFirearmNameUI(_activeFirearmController.fireArmData.firearmName);
            }
        }
        
        private void UpdateAmmoUI(int mag, int inv, int maxCap)
        {
            var isLow = mag > 0 && mag <= maxCap / 3;
            view.UpdateAmmoDisplay(mag, inv, maxCap, isLow);
        }

        private void UpdateFirearmNameUI(string firearmName)
        {
            view.UpdateNameDisplay(firearmName);
        }

        public override void Release()
        {
            model.OnItemSwapped -= HandleWeaponSwap;
            if (_activeFirearmController != null)
            {
                _activeFirearmController.OnAmmoChanged -= UpdateAmmoUI;
                _activeFirearmController.OnFirearmNameChanged -= UpdateFirearmNameUI;
            }
        }
    }
}