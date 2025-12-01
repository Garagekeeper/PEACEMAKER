using Resources.Script.Controller;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using static Resources.Script.Defines;

namespace Resources.Script.UI
{
    public class FirearmHUD : MonoBehaviour
    {
        [Header("Text")]
        public TextMeshProUGUI firearmNameText;
        public TextMeshProUGUI ammoTypeNameText;
        [FormerlySerializedAs("remainingAmmoText")] public TextMeshProUGUI remainingAmmoInMagazineText;
        [FormerlySerializedAs("remainingAmmoTypeText")] public TextMeshProUGUI remainingAmmoInInventoryText;
        public GameObject outOfAmmoAlert;
        public GameObject lowAmmoAlert;

        [Header("Colors")]
        public Color normalColor = Color.white;
        public Color alertColor = Color.red;

        public FirearmController Firearm { get; set; }

        private void Update()
        {
            if (!Firearm)
            {
                return;
            }

            gameObject.SetActive(Firearm.IsHUDActive);

            firearmNameText.SetText(Firearm.fireArmData.firearmName);
            ammoTypeNameText.SetText(AmmoNameList[(int)Firearm.requiredAmmoType.Type]);
            remainingAmmoInMagazineText.SetText(Firearm.AmmoInMagazine.ToString());
            remainingAmmoInInventoryText.SetText(Firearm.currentAmmo.Count.ToString());

            outOfAmmoAlert.SetActive(Firearm.AmmoInMagazine <= 0);
            lowAmmoAlert.SetActive(Firearm.AmmoInMagazine <= Firearm.preset.magazineCapacity / 3 && Firearm.AmmoInMagazine > 0);

            remainingAmmoInMagazineText.color = Firearm.AmmoInMagazine <= Firearm.preset.magazineCapacity / 3 ? alertColor : normalColor;
            remainingAmmoInInventoryText.color = Firearm.currentAmmo.Count <= 0 ? alertColor : normalColor;
        }

        private void LateUpdate()
        {
            if(Firearm == null)
            {
                Destroy(gameObject);
            }
        }
    }
}