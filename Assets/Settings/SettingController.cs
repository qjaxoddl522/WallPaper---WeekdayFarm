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
    int minimiNum;

    List<string> skinNames;

    void Start()
    {
        SkeletonData skeletonData = SystemManager.Instance.GetComponent<SkeletonAnimation>().Skeleton.Data;
        List<Skin> skins = skeletonData.Skins.ToList();
        skinNames = new List<string>();
        foreach (Skin skin in skins)
        {
            skinNames.Add(skin.Name);
        }
        GenerateItems();
    }

    void Update()
    {
        minimiNum = (int)minimiNumSlider.value;
        minimiNumText.text = $"등장 사도 수: {minimiNum.ToString()}";
    }

    void GenerateItems()
    {
        foreach (string skinName in skinNames)
        {
            GameObject newItem = Instantiate(toggleItemPrefab, verticalGroup);
            newItem.name = skinName;
            TextMeshProUGUI label = newItem.GetComponentInChildren<TextMeshProUGUI>();
            label.text = skinName.Substring(5);
        }
    }
}
