using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CAT
{
    public class StaminaBar : MonoBehaviour
    {
        public Slider slider;

        private void Start()
        {
            slider = GetComponent<Slider>();
        }
        public void SetmaxStamina(float maxHealth)
        {
            slider.maxValue = maxHealth;
            slider.value = maxHealth;
        }

        public void SetCurrentStamina(float currentHealth)
        {
            slider.value = currentHealth;
        }
    }
}