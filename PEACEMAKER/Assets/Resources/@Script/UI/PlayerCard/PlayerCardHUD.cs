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
        private bool _isLevelUp; // 레벨업 상태인지 확인


        public void Update()
        {
            UpdateHpBar();
            UpdateExpBar();
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

        public void UpdateLevelUpState(bool state)
        {
            _isLevelUp = state;
        }
        
        private void UpdateHpBar()
        {
            // 하얀바가 줄어든 만큼 빨간 바가 천천히 줄어듬
            // (빨간 바)
            _currentDamage = Mathf.Lerp(_currentDamage, playerHealthBar.value, Time.deltaTime * 5 * damageFollowSpeed);
            playerDamageBar.value = _currentDamage;
        }

        private void UpdateExpBar()
        {
            // 레벨업 중이라면 목표치를 강제로 1(100%)로 설정
            float target = _isLevelUp ? 1f : GetExpRatio();
            
            // Time.unscaledDeltaTime을 사용해야 Time.timeScale = 0(일시정지)에서도 바가 움직입니다.
            _currentBarExp = Mathf.Lerp(_currentBarExp, target, Time.unscaledDeltaTime * 5 * damageFollowSpeed);
            playerNextExpBar.value = _currentBarExp;

            // [연출 핵심] 바가 99% 이상 찼고 레벨업 상태라면, 0으로 즉시 초기화하여 다음 레벨업 준비
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