using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AUSJ
{
    public class TextUtils : MonoBehaviour
    {
        public static IEnumerator FadeInText(float timeSpeed, TextMeshPro screen, string text, bool clearScreen = true)
        {
            if (clearScreen)
            {
                screen.text = "";
            }

            for (int i = 0; i < text.Length; i++)
            {
                screen.text += text[i];
                yield return new WaitForSeconds(0.1f / timeSpeed);
            }
            yield return null;
        }

        public static IEnumerator FadeOutText(float timeSpeed, TextMeshProUGUI text)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
            while (text.color.a > 0.0f)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime * timeSpeed));
                yield return null;
            }
        }

        //private void FadeInText(float timeSpeed = -1.0f)
        //{
        //    if (timeSpeed <= 0.0f)
        //    {
        //        timeSpeed = timeMultiplier;
        //    }
        //    StartCoroutine(FadeInText(timeSpeed, textToUse));
        //}

        //private void FadeOutText(float timeSpeed = -1.0f)
        //{
        //    if (timeSpeed <= 0.0f)
        //    {
        //        timeSpeed = timeMultiplier;
        //    }
        //    StartCoroutine(FadeOutText(timeSpeed, textToUse));
        //}
    }
}
