using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CAT
{
    public class DamageCollider : MonoBehaviour
    {
        Collider damageCollider;

        public int currentWepaonDamage = 25;

        void Awake()
        {
            damageCollider = GetComponent<Collider>();
            damageCollider.gameObject.SetActive(true);
            damageCollider.isTrigger = true;
            damageCollider.enabled = false;
        }

        public void EnableDamageCollider()
        {
            damageCollider.enabled = true;
        }

        public void DisableDamageCollider()
        {
            damageCollider.enabled =false;
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.tag == "Hittable")
            {
                PlayerStats playerStats = collision.GetComponent<PlayerStats>();

                if(playerStats != null)
                {
                    playerStats.TakeDamage(currentWepaonDamage);
                }
            }

            if(collision.tag == "Enemy")
            {   
                EnemyStats enemyStats = collision.GetComponent<EnemyStats>();

                if (enemyStats != null)
                {
                    enemyStats.TakeDamage(currentWepaonDamage);
                }
            }

            if (collision.tag == "Player")
            {   
                PlayerStats playerStats = collision.GetComponent<PlayerStats>();

                if (playerStats != null)
                {
                    playerStats.TakeDamage(currentWepaonDamage);
                }                
            }
        }
    }
}