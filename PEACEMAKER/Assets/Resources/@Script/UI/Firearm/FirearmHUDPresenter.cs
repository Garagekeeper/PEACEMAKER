using Resources.Script.Controller;
using Resources.Script.Inventory;

namespace Resources.Script.UI.Firearm
{
    public class FirearmHUDPresenter : Presenter<FirearmHUD, InventoryCore>
    {
        private FirearmController _activeFirearm;
        
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
            if (_activeFirearm != null)
                _activeFirearm.OnAmmoChanged -= UpdateAmmoUI;
            
            //1. 새 무기로 교체
            _activeFirearm = newFirearmController;

            if (_activeFirearm != null)
            {
                //2. 새 무기의 이벤트 구독
                _activeFirearm.OnAmmoChanged += UpdateAmmoUI;
                
                //3. UI갱신
                UpdateAmmoUI(_activeFirearm.AmmoInMagazine, _activeFirearm.ammoItemInInventory.Count, _activeFirearm.fireArmData.magazineCapacity);
                UpdateFirearmNameUI(_activeFirearm.fireArmData.firearmName);
            }
        }
        
        private void UpdateAmmoUI(int mag, int inv, int maxCap)
        {
            view.UpdateAmmoDisplay(mag, inv, maxCap);
        }

        private void UpdateFirearmNameUI(string firearmName)
        {
            view.UpdateNameDisplay(firearmName);
        }

        public override void Release()
        {
            model.OnItemSwapped -= HandleWeaponSwap;
            if (_activeFirearm != null)
            {
                _activeFirearm.OnAmmoChanged -= UpdateAmmoUI;
                _activeFirearm.OnFirearmNameChanged -= UpdateFirearmNameUI;
            }
        }
    }
}