using System;
using Resources.Script.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Resources.Script.UI
{
    public class SettingComponent : MonoBehaviour
    {
        #region UI Elements
        protected Slider slider;
        protected TMP_Dropdown dropdown;
        protected Toggle toggle;
        #endregion

        private void Awake()
        {
            slider = this.FindSelfChild<Slider>();
            dropdown = this.FindSelfChild<TMP_Dropdown>();
            toggle = this.FindSelfChild<Toggle>();
            
            
        }

        public void Apply(float value)
        {
            
        }

        public void Load()
        {
            
        }
    }
}