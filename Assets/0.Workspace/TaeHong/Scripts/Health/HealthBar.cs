using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace Tae
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] BaseHealth health;
        [SerializeField] Slider healthSlider;
        [SerializeField] Slider shieldSlider;
        [SerializeField] Image sliderFill;
        [SerializeField] Sprite blueTeamFill;

        public void InitPlayerHealthBar()
        {
            health = Manager.Game.MyPlayer.GetComponent<BaseHealth>();
            //ApplyTeamColor();
            Start();
            OnEnable();
        }

        private void Start()
        {
            if (health == null)
                return;

            healthSlider.maxValue = health.MaxHealth;
            healthSlider.value = health.Health;
            shieldSlider.maxValue = health.MaxShield;
            shieldSlider.value = health.Shield;
            ApplyTeamColor();
        }

        private void OnEnable()
        {
            if (health == null)
                return;

            health.maxhealthChangedEvent += UpdateMaxHealth;
            health.maxShiedChangedEvent += UpdateMaxShield;
            health.healthChangedEvent += UpdateHealth;
            health.shiedChangedEvent += UpdateShield;
        }

        private void OnDisable()
        {
            if (health == null)
                return;

            health.maxhealthChangedEvent -= UpdateMaxHealth;
            health.healthChangedEvent -= UpdateHealth;
            health.shiedChangedEvent -= UpdateShield;
            health.maxShiedChangedEvent -= UpdateMaxShield;
        }

        private void ApplyTeamColor()
        {
            if (!PhotonNetwork.IsConnected)
                return;

            //팀전만 적용
            if (PhotonNetwork.CurrentRoom.GetGameMode())
                return;

            // 블루팀이면 색바꿈
            Team team = health.photonView.Owner.GetTeam();
            Debug.Log($"{gameObject.name} team : {team}");
            if(team == Team.Blue)
            {
                sliderFill.sprite = blueTeamFill;
            }
        }

        public void UpdateHealth(int newHP)
        {
            if (healthSlider == null)
                return;

            healthSlider.value = newHP;
        }

        public void UpdateMaxHealth(int newMaxHP)
        {
            if (healthSlider == null)
                return;

            healthSlider.maxValue = newMaxHP;

        }

        public void UpdateShield(int newShield)
        {
            if (shieldSlider == null)
                return;

            shieldSlider.value = newShield;
        }

        public void UpdateMaxShield(int newMaxShield)
        {
            if (shieldSlider == null)
                return;
            shieldSlider.maxValue = newMaxShield;

        }
    }
}