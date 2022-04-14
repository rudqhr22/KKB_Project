﻿using System.Collections;

using System.Collections.Generic;
using UnityEngine;
namespace CAT
{
    public class EnemyStats : CharacterStats
    {   
        Animator animator;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
        }

        void Start()
        {
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;            
        }

        private int SetMaxHealthFromHealthLevel()
        {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        public void TakeDamage(int damage)
        {   
            currentHealth = currentHealth - damage;

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                animator.Play("Dead01");
                animator.SetBool("isDead", true);
                return;
            }

            if (animator.GetBool("isHitting")== false)
            {
                animator.Play("Damage01");
                animator.SetBool("isHitting", true);
            }
            else
            {
                animator.Play("Damage02");
                animator.SetBool("isHitting", false);
            }
        }
    }
}