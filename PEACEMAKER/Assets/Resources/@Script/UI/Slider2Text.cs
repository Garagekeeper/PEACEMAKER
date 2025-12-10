using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Resources.Script.UI
{
    [ExecuteAlways]
    public class Slider2Text : MonoBehaviour
    {
        public TextMeshProUGUI _text;
        public Slider _slider;
        public int precision;
        
        private void Awake()
        {
            if (_slider == null)
            {
                _slider = this.FindSelfChild<Slider>();
            }

            if (_text == null)
            {
                _text = this.FindSelfChild<TextMeshProUGUI>();
            }
        }

        private void Update()
        {
            _text.text =  Math.Round(_slider.value, precision).ToString(CultureInfo.InvariantCulture);
        }
    }
}