using System;
using Resources.Script.Controller;
using Resources.Script.Creatures;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Resources.Script.UI.PlayerCard
{
    public class PlayerCardHUD : MonoBehaviour, IView
    {
        public TextMeshProUGUI playerNameText;
        public Slider playerHealthBar;
        public Slider playerDamageBar;
        public Slider playerNextExpBar;
        public float damageFollowSpeed = 1;
        
        
        private float _currentPlayerExp;
        private float _maxPlayerExp;
        private float _maxPlayerHp;
        private float _currentPlayerHp;
        private float _currentDamage;
        private float _expTarget;
        private float _currentBarExp;


        public void Update()
        {
            // 하얀바가 줄어든 만큼 빨간 바가 천천히 줄어듬
            // (빨간 바)
            _currentDamage = Mathf.Lerp(_currentDamage, playerHealthBar.value, Time.deltaTime * 5 * damageFollowSpeed);
            playerDamageBar.value = _currentDamage;
            
            
            _expTarget = GetExpRatio();
            _currentBarExp = Mathf.Lerp(_currentBarExp, _expTarget, Time.deltaTime * 5 * damageFollowSpeed);
            playerNextExpBar.value = _currentBarExp;
            
        }

        private float GetExpRatio()
        {
            if (_maxPlayerExp == 0) return 0;
            var ratio = _currentPlayerExp / (float)_maxPlayerExp;

            return ratio;
        }
        
        // 하얀 바가 줄어들고, 그 만큼 빨간바가 줄어듬
        public void UpdateHpValue(float currentValue, float maxValue)
        {
           _maxPlayerHp = maxValue;
           _currentPlayerHp = currentValue;
           
           // 하얀 바가 줄어들고, 그 만큼 빨간바가 줄어듬
           //최대 값 변경 (하얀 바)
           playerHealthBar.maxValue = _maxPlayerHp;
           
           // 실제 체력으로 슬라이더 조정
           playerHealthBar.value = _currentPlayerHp;
        }

        public void UpdateExpValue(float currentValue, float  maxValue)
        {
            _maxPlayerExp = maxValue;
            _currentPlayerExp = currentValue;
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