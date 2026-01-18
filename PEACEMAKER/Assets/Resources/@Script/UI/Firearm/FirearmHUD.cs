using TMPro;
using UnityEngine;

namespace Resources.Script.UI.Firearm
{
    public class FirearmHUD : UIBase, IView
    {
        [Header("Text")]
        public TextMeshProUGUI firearmNameText;
        public TextMeshProUGUI ammoTypeNameText;
        public TextMeshProUGUI ammoInMagazineText;
        public TextMeshProUGUI ammoInInventoryText;
        public GameObject outOfAmmoAlert;
        public GameObject lowAmmoAlert;
        
        [Header("Colors")]
        public Color normalColor = Color.white;
        public Color alertColor = Color.red;

        public void UpdateAmmoDisplay(int inMagazine, int inInventory, int maxCap)
        {
            ammoInMagazineText.text = inMagazine.ToString();
            ammoInInventoryText.text = inInventory.ToString();
        
            outOfAmmoAlert.SetActive(inMagazine <= 0);
            var isLow = inMagazine > 0 && inMagazine <= maxCap / 3;
            lowAmmoAlert.SetActive(isLow);
        
            ammoInMagazineText.color = isLow ? alertColor : normalColor;
        }

        public void UpdateNameDisplay(string firearmName)
        {
            firearmNameText.text = firearmName;
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}