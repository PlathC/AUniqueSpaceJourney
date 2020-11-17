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
        public float startEnergy = 90f;
        public Hand hand;
        public float decreaseStep = 2f;
        public float decreaseRandomMax = 3f;
        public TextMeshPro playerScreen;
        private float currentThirst;
        private float currentHunger;
        private float currentEnergy;

        private Valve.VR.InteractionSystem.Player player = null;
        private Coroutine hintCoroutine = null;
        public SteamVR_Action_Boolean teleportAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("ToggleOnOffTool");

        void Start()
        {
            // GameObject textMeshPro = GameObject.FindGameObjectsWithTag("Player-personal-screen");
            // playerScreen = textMeshPro.GetComponent<TextMeshPro>();
            player = Valve.VR.InteractionSystem.Player.instance;

            this.currentThirst = this.startThirst;
            this.currentHunger = this.startHunger;
            this.currentEnergy = this.startEnergy;

            // Init player stats
            UpdatePlayerScreen();

            // Decrease hunger and thirst every minute
            // InvokeRepeating("UpdatePlayerCondition", 10, 3);

            // SteamVR_Action_Boolean toggleAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("ToggleOnOffTool");
            hand = GameObject.Find("LeftHand").GetComponent<Hand>();
            // Debug.Log(hand);
            // Debug.Log(toggleAction);
            // ControllerButtonHints.ShowButtonHint(hand, toggleAction);
            // ControllerButtonHints.ShowTextHint(hand, toggleAction, "Outil ON/OFF", true);

            // ControllerButtonHints.HideAllButtonHints(hand);
            // ControllerButtonHints.HideAllTextHints(hand);
            // ControllerButtonHints.ShowTextHint(hand, hand.grabGripAction, "Outil ON/OFF", true);
            Invoke("TestHand", 5.0f);
            //Invoke("TestHandCancel", 10.0f);
        }

        private void TestHand()
        {
            // SteamVR_Action_Boolean toggleAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("ToggleOnOffTool");
            SteamVR_Action_Boolean toggleAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Teleport");
            //ControllerButtonHints.ShowButtonHint(hand, hand.uiInteractAction);
            //ControllerButtonHints.ShowTextHint(hand, toggleAction, "Toggle On/Off Tool", true);
            ControllerButtonHints.ShowTextHint(hand, toggleAction, "Teleport", true);
        }

        private void TestHandCancel()
        {
            ControllerButtonHints.HideAllTextHints(hand);
        }

        private void UpdatePlayerScreen()
        {
            playerScreen.text = "# Statut du joueur" + "\n";
            playerScreen.text += "<color=\"blue\">Soif       [" + this.ComputeBarStat(this.currentThirst) + "] " + (int)this.currentThirst + "% </color>" + "\n";
            playerScreen.text += "<color=\"green\">Faim     [" + this.ComputeBarStat(this.currentHunger) + "] " + (int)this.currentHunger + "% </color>" + "\n";
            playerScreen.text += "<color=\"red\">Energie [" + this.ComputeBarStat(this.currentEnergy) + "] " + (int)this.currentEnergy + "% </color>" + "\n";
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
            this.currentHunger -= Random.Range(0f, decreaseRandomMax);
            if (this.currentHunger < 0) this.currentHunger = 0;
            this.currentHunger = this.currentHunger % 100;

            // Thirst
            this.currentThirst -= decreaseStep; 
            this.currentThirst -= Random.Range(0f, decreaseRandomMax);
            if (this.currentThirst < 0) this.currentThirst = 0;
            this.currentThirst = this.currentThirst % 100;

            // Update screen
            UpdatePlayerScreen();
        }

        private void Update()
        {
            // Set screen to new stats
        }
    }
}
