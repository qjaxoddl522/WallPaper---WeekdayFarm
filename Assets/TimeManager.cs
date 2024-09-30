using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    [SerializeField] RectTransform hourParent;
    [SerializeField] TextMeshProUGUI hour;
    [SerializeField] TextMeshProUGUI ampm;
    [SerializeField] TextMeshProUGUI date;
    float timer = 1f;
    string prevHour;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 1f)
        {
            DateTime now = DateTime.Now;
            string currentHour = now.ToString("hh:mm");
            string currentAmpm = now.ToString("tt", new CultureInfo("en-US"));
            string currentDate = now.ToString("yyyy. MM. dd.");

            if (prevHour == null || prevHour != currentHour)
            {
                prevHour = currentHour;

                hour.text = currentHour;
                hour.transform.parent.GetComponent<TextMeshProUGUI>().text = currentHour;
                ampm.text = currentAmpm;
                ampm.transform.parent.GetComponent<TextMeshProUGUI>().text = currentAmpm;
                date.text = currentDate;
                date.transform.parent.GetComponent<TextMeshProUGUI>().text = currentDate;
                LayoutRebuilder.ForceRebuildLayoutImmediate(hourParent);
            }

            timer = 0f; 
        }
    }
}
