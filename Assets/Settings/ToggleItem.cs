using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleItem : MonoBehaviour
{
    Toggle toggle;
    void Start()
    {
        toggle = GetComponent<Toggle>();
    }

    public void ToggleMinimi(bool isOn)
    {
        if (isOn)
            SystemManager.Instance.settings.AddSkin(name);
        else
            SystemManager.Instance.settings.RemoveSkin(name);
    }
}
