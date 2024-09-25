using Spine.Unity;
using Spine;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StringListWrapper
{
    public List<string> list;
}

[System.Serializable]
public class Settings
{
    public int minimiNum;
    public HashSet<string> selectedSkins;

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("minimiNum", minimiNum);

        StringListWrapper wrapper = new StringListWrapper();
        wrapper.list = selectedSkins.ToList();
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString("selectedSkins", json);
    }

    public void LoadSettings()
    {
        minimiNum = 3;
        selectedSkins = new HashSet<string>();

        if (PlayerPrefs.HasKey("minimiNum"))
        {
            minimiNum = PlayerPrefs.GetInt("minimiNum");
        }
        if (PlayerPrefs.HasKey("selectedSkins"))
        {
            string json = PlayerPrefs.GetString("selectedSkins");
            StringListWrapper wrapper = JsonUtility.FromJson<StringListWrapper>(json);
            selectedSkins = wrapper.list.ToHashSet();
        }
        else
        {
            SkeletonData skeletonData = SystemManager.Instance.skAnim_Reading.Skeleton.Data;
            foreach(var skin in skeletonData.Skins)
            {
                selectedSkins.Add(skin.Name);
            }
        }
    }

    public void SetMinimiNum(int n)
    {
        minimiNum = n;
    }

    public void AddSkin(string skinName)
    {
        selectedSkins.Add(skinName);
    }
    public void RemoveSkin(string skinName)
    {
        selectedSkins.Remove(skinName);
    }
}

public class SystemManager : MonoBehaviour
{
    private static SystemManager instance;
    public static SystemManager Instance
    {
        get
        {
            return instance;
        }
    }

    public Settings settings;
    public GameObject settingWindowPrefab;
    public Transform canvas;

    public GameObject settingWindow;
    Minimi_Spawner minimi_spawner;

    public SkeletonAnimation skAnim_Reading;

    void Awake()
    {
        instance = this;
        minimi_spawner = FindAnyObjectByType<Minimi_Spawner>();
        skAnim_Reading = GetComponent<SkeletonAnimation>();
        
        settings = new Settings();
        settings.LoadSettings();
    }

    void Start()
    {
        settingWindow = Instantiate(settingWindowPrefab, canvas);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!settingWindow.activeSelf)
            {
                PlayerPrefs.DeleteAll();
                settingWindow.SetActive(true);
            }
        }
    }
}
