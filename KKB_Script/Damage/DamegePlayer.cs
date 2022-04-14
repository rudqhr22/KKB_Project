using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CAT
{
    public class DamegePlayer : MonoBehaviour
    {
        public int damage = 10;

        private void OnTriggerEnter(Collider other)
        {
           PlayerStats playerStats =  other.GetComponent<PlayerStats>();

            if(playerStats != null)
            {
                playerStats.TakeDamage(damage);
            }
        }
    }
}