using Spine.Unity;
using Spine;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    public GameObject settingWindowPrefab;
    public Transform canvas;

    GameObject settingWindow;
    Minimi_Spawner minimi_spawner;
    SkeletonAnimation skAnim_Reading;
    int minimiNum;

    void Awake()
    {
        instance = this;
        minimi_spawner = FindAnyObjectByType<Minimi_Spawner>();
        skAnim_Reading = GetComponent<SkeletonAnimation>();
    }

    void Start()
    {
        SkeletonData skeletonData = skAnim_Reading.Skeleton.Data;
        List<Skin> skins = skeletonData.Skins.ToList();
        minimiNum = Mathf.Min(Random.Range(3, 6), skins.Count);

        List<Skin> shuffledSkins = skins.OrderBy(s => Random.value).ToList();
        List<Skin> selectedSkins = shuffledSkins.Take(minimiNum).ToList();
        foreach (Skin skin in selectedSkins)
        {
            minimi_spawner.minimiNames.Add(skin.Name);
        }

        settingWindow = Instantiate(settingWindowPrefab, canvas);
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(2))
        {
            if (settingWindow.activeSelf)
                settingWindow.SetActive(false);
            else
                settingWindow.SetActive(true);
        }
    }
}
