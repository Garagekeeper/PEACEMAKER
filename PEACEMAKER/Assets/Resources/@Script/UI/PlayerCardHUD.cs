using Resources.Script.Controller;
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
        public float damageFollowSpeed = 1;

        
        private Player _player;
        
        private float currentDamage;

        private bool isEnabled;

        private float maxHealth;

        public void Setup(Player player)
        {
            if (!CheckIfIsPlayer(player))
                return;
            
            _player = player;
            IDamageable damageable = player.gameObject.GetComponent<IDamageable>();

            if (damageable == null)
            {
                Debug.LogError("Couldn't find 'IDamagale' on Actor. HealthBar update aborted.");
                return;
            }

            maxHealth = damageable.Hp;
        }

        // 하얀 바가 줄어들고, 그 만큼 빨간바가 줄어듬
        public void UpdateCard()
        {
            if (!playerHealthBar || !playerDamageBar) return;
            // 하얀바가 줄어든 만큼 빨간 바가 천천히 줄어듬
            currentDamage = Mathf.Lerp(currentDamage, playerHealthBar.value, Time.deltaTime * 5 * damageFollowSpeed);
            playerDamageBar.value = currentDamage;
            
            //TODO 캐릭별로 이름 만들기
            string playerName = string.IsNullOrEmpty(_player.gameObject.name) ? "UNKNOWN_ACTOR_NAME" : _player.gameObject.name;
            playerNameText.SetText(playerName);
            
            IDamageable damageable = _player.gameObject.GetComponent<IDamageable>();
            if (damageable == null)
            {
                Debug.LogError("Couldn't find 'IDamagale' on Actor. HealthBar update aborted.");
                return;
            }
            
            if (playerHealthBar == null)
            {
                Debug.LogError("PlayerHealthBar is null.");
                return;
            }

            //최대 값 변경
            playerHealthBar.maxValue = maxHealth;
            // 실제 체력으로 슬라이더 조정
            playerHealthBar.value = damageable.Hp;
        }

        public void Enable()
        {
            if (!_player) return;

            isEnabled = true;
            foreach (Transform t in transform)
                t.gameObject.SetActive(isEnabled);
        }

        public void Disable(Player player)
        {
            if (!_player) return;

            isEnabled = false;
            foreach (Transform t in transform)
                t.gameObject.SetActive(isEnabled);
        }

        public virtual bool CheckIfIsPlayer(Player player)
        {
            if (player == null) return false;
            if (player.PController == null) return false;

            return true;
        }
    }
}