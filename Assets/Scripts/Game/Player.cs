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
        
        private Hand hand;
        private TextMeshPro playerScreen;
        private GameObject playerWatch;
        private float currentThirst;
        private float currentHunger;
        //private float currentEnergy; // NOT USED YET

        public GameObject PlayerWatch { get => playerWatch; }
        public TextMeshPro PlayerScreen { get => playerScreen; }

        void Start()
        {
            // Get Playerscreen
            try
            {
                playerScreen = GameObject.FindGameObjectsWithTag("playerScreen")[0].GetComponent<TextMeshPro>();
            }
            catch (IndexOutOfRangeException)
            {
                throw new Exception("Player screen TAG not found");
            }

            try
            {
                hand = GameObject.Find("LeftHand").GetComponent<Hand>();
            }
            catch (Exception)
            {
                throw new Exception("Left Hand TAG cannot be found");
            }

            try
            {
                playerWatch = GameObject.FindGameObjectsWithTag("playerWatch")[0];
            }
            catch (Exception)
            {
                throw new Exception("Player watch TAG cannot be found");
            }

            // Disable player screen
            playerScreen.transform.gameObject.SetActive(false);

            // Change watch texture
            playerWatch.GetComponent<Renderer>().material = new Material(Shader.Find("Shader Graphs/Glowing red"));
            // playerWatch.GetComponent<Renderer>().material = new Material(Shader.Find("Shader Graphs/Glowing blue"));

            // Player instance
            // player = Valve.VR.InteractionSystem.Player.instance;

            // Set initial stats
            this.currentThirst = this.startThirst;
            this.currentHunger = this.startHunger;
            //this.currentEnergy = this.startEnergy;

            // Init player stats
            // UpdatePlayerScreen();

            // Decrease hunger and thirst every minute
            // InvokeRepeating("UpdatePlayerCondition", 10, 3);
        }

        public void UpdatePlayerScreen()
        {
            playerScreen.text = "# Statut du joueur" + "\n";
            playerScreen.text += "<color=\"blue\">Soif       [" + this.ComputeBarStat(this.currentThirst) + "] " + (int)this.currentThirst + "% </color>" + "\n";
            playerScreen.text += "<color=\"green\">Faim     [" + this.ComputeBarStat(this.currentHunger) + "] " + (int)this.currentHunger + "% </color>" + "\n";
            //playerScreen.text += "<color=\"red\">Energie [" + this.ComputeBarStat(this.currentEnergy) + "] " + (int)this.currentEnergy + "% </color>" + "\n";
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
        public void UpdatePlayerCondition()
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
            UpdatePlayerScreen();
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
            UpdatePlayerScreen();
        }

        /// <summary>
        /// Restore thirst by X percent in parameter
        /// </summary>
        /// <param name="percent">Percentage to restore</param>
        private void RestoreThirst(int percent)
        {
            // Restore thirst
            currentThirst += percent;

            // Cap current thirst
            currentThirst = currentThirst % 100;

            // Update player stats screen
            UpdatePlayerScreen();
        }

        private void Update()
        {

        }
    }
}
