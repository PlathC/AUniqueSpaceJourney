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
        public float startThirst = 30f;
        public float startHunger = 45f;
        //public float startEnergy = 90f; // NOT USED YET
        public float decreaseStep = 2f;
        public float decreaseRandomMax = 3f;
        
        //private Hand hand;
        private Watch playerWatch;
        private FlashLight flashLight;
        private TextMeshPro playerScreen;
        private float currentThirst;
        private float currentHunger;
        //private float currentEnergy; // NOT USED YET

        public Watch PlayerWatch { get => playerWatch; }
        public FlashLight FlashLight { get => flashLight; set => flashLight = value; }

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

            // Store player screen reference
            playerScreen = playerWatch.PlayerScreen;

            // Disable watch by default
            playerWatch.DisableWatch();

            // Disable flashlight by default
            //FlashLight.Locked = true;

            // Disable cave tp
            GameObject.Find("BigPartGround").GetComponent<TeleportAreaCustom>().SetLocked(false);

            // Set initial stats
            this.currentThirst = this.startThirst;
            this.currentHunger = this.startHunger;
            //this.currentEnergy = this.startEnergy; // NOT USED
        }

        public IEnumerator StartUpdatePlayerConditions()
        {
            // First stats appearence animation
            float speed = 10f;
            yield return StartCoroutine(TextUtils.FadeInText(speed, playerScreen, UpdatePlayerScreen()));

            // Decrease hunger and thirst every minute
            InvokeRepeating("UpdatePlayerCondition", 0, 60);
        }

        public string UpdatePlayerScreen()
        {
            string res = "# Statut du joueur" + "\n";
            res += "<color=\"blue\">Soif       [" + this.ComputeBarStat(this.currentThirst) + "] " + (int)this.currentThirst + "% </color>" + "\n";
            res += "<color=\"green\">Faim     [" + this.ComputeBarStat(this.currentHunger) + "] " + (int)this.currentHunger + "% </color>" + "\n";
            //playerScreen.text += "<color=\"red\">Energie [" + this.ComputeBarStat(this.currentEnergy) + "] " + (int)this.currentEnergy + "% </color>" + "\n";
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
            this.currentHunger -= decreaseStep;
            this.currentHunger -= UnityEngine.Random.Range(0f, decreaseRandomMax);
            if (this.currentHunger < 0) this.currentHunger = 0;
            this.currentHunger = this.currentHunger % 100;

            // Thirst
            this.currentThirst -= decreaseStep; 
            this.currentThirst -= UnityEngine.Random.Range(0f, decreaseRandomMax);
            if (this.currentThirst < 0) this.currentThirst = 0;
            this.currentThirst = this.currentThirst % 100;

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
            currentHunger = currentHunger % 100;

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
            currentThirst = currentThirst % 100;

            // Update player stats screen
            playerScreen.text = UpdatePlayerScreen();
        }

        private void Update()
        {

        }
    }
}
