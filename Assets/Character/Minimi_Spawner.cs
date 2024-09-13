using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Minimi_Spawner : MonoBehaviour
{
    public string[] exceptAnimationNames; // ������ �ִϸ��̼�

    SkeletonAnimation skAnim;
    List<string> animationNames;

    void Start()
    {
        // �ִϸ��̼� �̸� ��� �ҷ�����
        skAnim = GetComponent<SkeletonAnimation>();
        animationNames = new List<string>();
        Spine.SkeletonData skeletonData = skAnim.Skeleton.Data;
        foreach (var animation in skeletonData.Animations)
        {
            if (!exceptAnimationNames.Any(except => animation.Name.ToLower().Contains(except.ToLower())))
                animationNames.Add(animation.Name);
        }
    }
}
