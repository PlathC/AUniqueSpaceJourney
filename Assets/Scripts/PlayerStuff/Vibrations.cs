using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace AUSJ
{
    public class Vibrations
    {
        /// <summary>
        /// Trigger simple pulse
        /// </summary>
        /// <param name="hand">Controller to apply vibration on</param>
        /// <param name="amplitude">Amplitude between 0 and 1</param>
        /// <param name="duration">Duration in micro seconds</param>
        public static void SimplePulse(Hand hand, float amplitude = 0.4f, float duration = 300000f)
        {
            float seconds = (float)duration / 1000000f;
            hand.TriggerHapticPulse(seconds, 1f / seconds, amplitude);
        }

        /// <summary>
        /// Trigger multiple pulses in a given interval
        /// </summary>
        /// <param name="hand">Controller to apply vibration on</param>
        /// <param name="nbPulse">Pulse number</param>
        /// <param name="interval">Interval between pulses in seconds</param>
        /// <param name="increaseStep">Amplitude increase step for each pulse</param>
        /// <returns></returns>
        public static IEnumerator PulseAscend(Hand hand, int nbPulse, float interval, float increaseStep)
        {
            float microSecondsDuration = 2500f;
            float seconds = (float)microSecondsDuration / 1000000f;
            for (int i = 0; i < nbPulse; i++)
            {
                float amplitude = 0.3f + i * increaseStep;
                if (amplitude > 1)
                {
                    amplitude = 1f;
                }
                hand.TriggerHapticPulse(seconds, 1f / seconds, amplitude);
                yield return new WaitForSeconds(interval);
            }
        }
    }
}
