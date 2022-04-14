using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CAT
{
    public class PlayerAttacker : MonoBehaviour
    {
        AnimatorHandler animatorHandler;
        InputHandler inputHandler;
        public string lastAttack;

        public DamageCollider damageCollider;
        PlayerStats playerStats;

        private void Awake()
        {
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            inputHandler = GetComponent<InputHandler>();
            playerStats = GetComponent<PlayerStats>();
        }
        public void HandleWeaponCombo()
        {   
            if (playerStats.currentStamina <= 0)
                return;

            if (inputHandler.comboFlag)
            {
                animatorHandler.anim.SetBool("canDoCombo", false);
                if (lastAttack == "Light_Attack_1")
                {
                    animatorHandler.PlayTagetAnimation("Light_Attack_2", true);
                    lastAttack = "Light_Attack_2";                    
                }
                else if (lastAttack == "Light_Attack_2")
                {
                    animatorHandler.PlayTagetAnimation("Light_Attack_3", true);
                    lastAttack = "Light_Attack_3";                    
                }
                else if (lastAttack == "Light_Attack_3")
                {
                    animatorHandler.PlayTagetAnimation("Light_Attack_4", true);                    
                }
            }            
        }

        public void HandleLightAttack()
        {
            if (playerStats.currentStamina <= 0)
                return;

            animatorHandler.PlayTagetAnimation("Light_Attack_1", true);
                lastAttack = "Light_Attack_1";                
        }

        public void HandleHeavyAttack()
        {
            if (playerStats.currentStamina <= 0)
                return;

            animatorHandler.PlayTagetAnimation("Heavy_Attack_1", true);
                lastAttack = "Heavy_Attack_1";                
        }
    }
}