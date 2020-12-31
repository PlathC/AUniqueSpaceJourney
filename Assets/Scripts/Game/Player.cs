using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace AUSJ
{
    public class Player : MonoBehaviour
    {
        [SerializeField]
        private float startThirst = 80f;
        
        [SerializeField]
        private float startHunger = 80f;

        //[SerializeField]
        //private float startEnergy = 90f; // NOT USED YET
        
        [SerializeField]
        private float decreaseStep = 2f;
        
        [SerializeField]
        private float decreaseRandomMax = 3f;

        [SerializeField]
        [Tooltip("Decrease stats every X seconds")]
        private float decreaseEvery = 120f; // in sec

        private float decreaseStart = 10; // in sec

        //private Hand hand;
        private Watch playerWatch;
        private FlashLight flashLight;
        private TextMeshPro playerScreen;
        private GameObject holsters;
        private float currentThirst;
        private float currentHunger;
        //private float currentEnergy; // NOT USED YET

        public Watch PlayerWatch { get => playerWatch; }
        public FlashLight FlashLight { get => flashLight; set => flashLight = value; }
        public GameObject Holsters { get => holsters; set => holsters = value; }
        public float CurrentThirst { get => currentThirst; }
        public float CurrentHunger { get => currentHunger; }

        void Start()
        {
            try
            {
                playerWatch = GameObject.FindGameObjectsWithTag("playerWatch")[0].GetComponent<Watch>();
            }
            catch (Exception)
            {
                throw new Exception("Player watch TAG cannot be found");
            }

            try
            {
                FlashLight = GameObject.FindGameObjectsWithTag("flashlight")[0].GetComponent<FlashLight>();
            }
            catch (Exception)
            {
                throw new Exception("flashLight TAG cannot be found");
            }

            try
            {
                Holsters = GameObject.Find("Holsters");
            }
            catch (Exception)
            {
                throw new Exception("Holsters cannot be found");
            }

            // Store player screen reference
            playerScreen = playerWatch.PlayerScreen;

            // Disable watch by default
            playerWatch.DisableWatch();

            // Disable flashlight by default
            FlashLight.Locked = true;

            // Disable holsters by default
            Holsters.SetActive(false);

            // Set initial stats
            currentThirst = startThirst;
            currentHunger = startHunger;
            //currentEnergy = startEnergy; // NOT USED
        }

        public void StartUpdatePlayerConditions()
        {
            // Show stats
            playerScreen.text = UpdatePlayerScreen();

            // Decrease hunger and thirst every minute
            InvokeRepeating(nameof(UpdatePlayerCondition), decreaseStart, decreaseEvery);
        }

        public string UpdatePlayerScreen()
        {
            string colorThirst = "blue";
            string colorHunger = "green";

            // Red color if stats are low
            if (CurrentThirst <= 10)
            {
                colorThirst = "red";
            }

            if (CurrentHunger <= 10)
            {
                colorHunger = "red";
            }

            string res = "# Statut du joueur" + "\n";
            res += "<color=\"" + colorThirst + "\">Soif       [" + ComputeBarStat(CurrentThirst) + "] " + (int)CurrentThirst + "% </color>" + "\n";
            res += "<color=\"" + colorHunger + "\">Faim     [" + ComputeBarStat(CurrentHunger) + "] " + (int)CurrentHunger + "% </color>" + "\n";
            //playerScreen.text += "<color=\"red\">Energie [" + ComputeBarStat(currentEnergy) + "] " + (int)currentEnergy + "% </color>" + "\n";
            return res;
        }

        private string ComputeBarStat(float percent)
        {
            string res = "";
            int nbBar = (int)(percent / 5);
            // Add one bar for each 5%
            for (int i = 0; i < nbBar; i++)
            {
                res += 'I';
            }
            // Complete bar with white spaces
            for (int i = 0; i < 20 - nbBar; i++)
            {
                res += ' ';
            }
            return res;
        }

        /// <summary>
        /// Decrease player conditions periodically by "decreaseStep" + Random between 1 and 6 %
        /// </summary>
        private void UpdatePlayerCondition()
        {
            // Hunger
            currentHunger -= decreaseStep;
            currentHunger -= UnityEngine.Random.Range(0f, decreaseRandomMax);
            if (CurrentHunger < 0) currentHunger = 0;
            currentHunger = CurrentHunger % 100;

            // Thirst
            currentThirst -= decreaseStep; 
            currentThirst -= UnityEngine.Random.Range(0f, decreaseRandomMax);
            if (CurrentThirst < 0) currentThirst = 0;
            currentThirst = CurrentThirst % 100;

            if (CurrentHunger <= 10 && CurrentThirst <= 10)
            {
                playerWatch.PlayWarningSound();
            }

            // Update screen
            playerScreen.text = UpdatePlayerScreen();
        }

        /// <summary>
        /// Restore hunger by X percent in parameter
        /// </summary>
        /// <param name="percent">Percentage to restore</param>
        public void RestoreHunger(int percent)
        {
            // Restore hunger
            currentHunger += percent;
            
            // Cap current hunger
            currentHunger = CurrentHunger % 100;

            // Update player stats screen
            playerScreen.text = UpdatePlayerScreen();
        }

        /// <summary>
        /// Restore thirst by X percent in parameter
        /// </summary>
        /// <param name="percent">Percentage to restore</param>
        public void RestoreThirst(int percent)
        {
            // Restore thirst
            currentThirst += percent;

            // Cap current thirst
            currentThirst = CurrentThirst % 100;

            // Update player stats screen
            playerScreen.text = UpdatePlayerScreen();
        }
    }
}
