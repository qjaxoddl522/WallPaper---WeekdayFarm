using Spine;
using Spine.Unity;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingController : MonoBehaviour
{
    [SerializeField] GameObject toggleItemPrefab;
    [SerializeField] Transform verticalGroup;
    [SerializeField] Slider minimiNumSlider;
    [SerializeField] TextMeshProUGUI minimiNumText;

    SystemManager sys;
    List<string> skinNames;
    List<Toggle> toggles;

    void Start()
    {
        sys = SystemManager.Instance;
        InitSettings();
    }

    void InitSettings()
    {
        minimiNumSlider.value = sys.settings.minimiNum;
        toggles = new List<Toggle>();

        SkeletonData skeletonData = sys.GetComponent<SkeletonAnimation>().Skeleton.Data;
        List<Skin> skins = skeletonData.Skins.ToList();
        skinNames = new List<string>();
        foreach (Skin skin in skins)
        {
            skinNames.Add(skin.Name);
        }
        GenerateItems();
    }

    void GenerateItems()
    {
        foreach (string skinName in skinNames)
        {
            GameObject newItem = Instantiate(toggleItemPrefab, verticalGroup);
            newItem.name = skinName;

            Toggle toggle = newItem.GetComponent<Toggle>();
            toggle.isOn = sys.settings.selectedSkins.Contains(skinName);
            toggle.onValueChanged.AddListener(newItem.GetComponent<ToggleItem>().ToggleMinimi);
            toggles.Add(toggle);

            TextMeshProUGUI label = newItem.GetComponentInChildren<TextMeshProUGUI>();
            label.text = skinName.Substring(5);
        }
    }

    public void OnSliderValueChanged()
    {
        sys.settings.minimiNum = (int)minimiNumSlider.value;
        minimiNumText.text = $"등장 사도 수: {(int)minimiNumSlider.value}";
    }

    public void OnClickClose()
    {
        sys.SettingWindowOff();
    }

    public void ToggleAllOn()
    {
        foreach(Toggle toggle in toggles)
            toggle.isOn = true;
    }

    public void ToggleAllOff()
    {
        foreach (Toggle toggle in toggles)
            toggle.isOn = false;
    }
}
