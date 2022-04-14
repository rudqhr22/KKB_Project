using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CAT
{
    public class PlayerStats : CharacterStats
    {
        public float lightAtkStamina = 20;
        public float heavyAtkStamina = 40;
        public float statimaRegenerationAmout = 100;
        public float staminaRegenTimer = 0f;

        
        public HealthBar healthBar;
        public StaminaBar staminaBar;

        AnimatorHandler animatorHandler;
        PlayerManager playerManager;

        private void Awake()
        {   
            healthBar = FindObjectOfType<HealthBar>();
            staminaBar = FindObjectOfType<StaminaBar>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            playerManager = GetComponent<PlayerManager>();
        }

        void Start()
        {
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetCurrentHealth(currentHealth);

            maxStamina = SetMaxStaminaFromHealthLevel();
            currentStamina = maxStamina;
            staminaBar.SetmaxStamina(maxStamina);
            staminaBar.SetCurrentStamina(currentStamina);            
        }

        private int SetMaxHealthFromHealthLevel()
        {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }
        private float SetMaxStaminaFromHealthLevel()
        {
            maxStamina = staminaLevel * 10;
            return maxStamina;
        }
        public void RegenerateStamina()
        {
            if (isDead) return;
            

            if (playerManager.isInteracting)
            {
                staminaRegenTimer = 0;
            }
            else            
            {
                staminaRegenTimer += Time.deltaTime;                
                if (currentStamina < maxStamina && staminaRegenTimer > 0.01f)
                {
                    currentStamina += (statimaRegenerationAmout * Time.deltaTime) + 0.08f;
                    staminaBar.SetCurrentStamina(currentStamina);
                }
            }
        }
        public void TakeDamage(int damage)
        {
            if (playerManager.isInvulnerable) return;
            if (isDead) return;

            currentHealth = currentHealth - damage;
            healthBar.SetCurrentHealth(currentHealth);

            animatorHandler.PlayTagetAnimation("Damage01", true);

            if(currentHealth <= 0)
            {
                isDead = true;
                currentHealth = 0;
                animatorHandler.PlayTagetAnimation("Dead01", true);                
            }
        }
        public void TakeStaminaDamage(float damage)
        {
            if (isDead) return;

            currentStamina = currentStamina - damage;            
            staminaBar.SetCurrentStamina(currentStamina);
        }
    }
}