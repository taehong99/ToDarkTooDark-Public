using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Tae
{
    public class ExcaliburSkillUI : BaseUI
    {
        private ExcaliburOwner owner;
        [SerializeField] float delta;
        [SerializeField] GameObject skillEntryPrefab;

        private HorizontalLayoutGroup skillQueue;
        private Image cooldownTimer;
        float originalX;

        private void Start()
        {
            skillQueue = GetUI<HorizontalLayoutGroup>("SkillQueue");
            cooldownTimer = GetUI<Image>("CooldownTimer");
            gameObject.SetActive(false);
        }

        public void SetOwner(ExcaliburOwner owner)
        {
            this.owner = owner;
        }

        private void OnEnable()
        {
            if(owner != null)
                owner.cooldownChangedEvent += OnCooldownChanged;
        }

        private void OnDisable()
        {
            if (owner != null)
                owner.cooldownChangedEvent -= OnCooldownChanged;
        }

        private void OnCooldownChanged(float cooldownRatio)
        {
            cooldownTimer.fillAmount = cooldownRatio;
        }

        public void AddSkill(ExcaliburSkillSO skill)
        {
            GameObject entry = Instantiate(skillEntryPrefab);
            entry.GetComponent<Image>().sprite = skill.icon;
            entry.transform.SetParent(skillQueue.transform);
        }

        private ExcaliburSkillSO skillToAdd;
        public void NextSkill(ExcaliburSkillSO newSkill)
        {
            originalX = skillQueue.transform.position.x;
            float targetX = skillQueue.transform.position.x - delta;
            skillToAdd = newSkill;
            cooldownTimer.enabled = false;
            skillQueue.transform.DOMoveX(targetX, 1).SetEase(Ease.Linear).OnComplete(RemoveAndAddSkill);
        }

        private void RemoveAndAddSkill()
        {
            cooldownTimer.enabled = true;
            Destroy(skillQueue.transform.GetChild(0).gameObject);
            AddSkill(skillToAdd);
            skillQueue.transform.DOMoveX(originalX, 0).SetEase(Ease.Linear);
        }
    }
}