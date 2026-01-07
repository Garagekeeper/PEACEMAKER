using Resources.Script.Creatures;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Resources.Script.UI
{
    public class PlayerCardHUD : MonoBehaviour
    {
        public TextMeshProUGUI playerNameText;
        public Slider playerHealthBar;
        public Slider playerDamageBar;
        public Slider playerNextExpBar;
        public float damageFollowSpeed = 1;
        
        private Player _player;
        
        private float _currentDamage;
        private float _currentExp; 
        private bool _isEnabled;
        private float _maxHealth;
        private float _expTarget;

        public void Setup(Player player)
        {
            if (!CheckIfIsPlayer(player))
                return;
            
            _player = player;
            IDamageable damageable = player.gameObject.GetComponent<IDamageable>();

            if (damageable == null)
            {
                Debug.LogError("Couldn't find 'IDamageable' on Actor. HealthBar update aborted.");
                return;
            }
            
            //TODO 캐릭별로 이름 만들기
            string playerName = string.IsNullOrEmpty(_player.gameObject.name) ? "UNKNOWN_ACTOR_NAME" : _player.gameObject.name;
            playerNameText.SetText(playerName);

            _maxHealth = damageable.Hp;
            playerNextExpBar.value = 0;
        }

        // 하얀 바가 줄어들고, 그 만큼 빨간바가 줄어듬
        public void UpdateCardHp()
        {
            if (!playerHealthBar || !playerDamageBar) return;
            //최대 값 변경 (하얀 바)
            playerHealthBar.maxValue = _maxHealth;
            // 실제 체력으로 슬라이더 조정
            playerHealthBar.value = _player.Hp;
            
            // 하얀바가 줄어든 만큼 빨간 바가 천천히 줄어듬
            // (빨간 바)
            _currentDamage = Mathf.Lerp(_currentDamage, playerHealthBar.value, Time.deltaTime * 5 * damageFollowSpeed);
            playerDamageBar.value = _currentDamage;
            
           
        }

        public void UpdateCardExp()
        {
            _expTarget = GetExpRatio();
            
            _currentExp = Mathf.Lerp(_currentExp, _expTarget, Time.deltaTime * 5 * damageFollowSpeed);
            playerNextExpBar.value = _currentExp;
        }   

        private float GetExpRatio()
        {
            var ratio = _player.CurrExp / (float)_player.MaxExp;

            return ratio;
        }
        

        public void Enable()
        {
            if (!_player) return;

            _isEnabled = true;
            foreach (Transform t in transform)
                t.gameObject.SetActive(_isEnabled);
        }

        public void Disable()
        {
            if (!_player) return;

            _isEnabled = false;
            foreach (Transform t in transform)
                t.gameObject.SetActive(_isEnabled);
        }

        public bool CheckIfIsPlayer(Player player)
        {
            if (player == null) return false;
            return player.PController != null;
        }
    }
}