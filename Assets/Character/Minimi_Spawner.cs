using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using Spine;

public class Minimi_Spawner : MonoBehaviour
{
    public List<string> minimiNames;      // ������ �絵 �̸���
    public string[] exceptAnimationNames; // ������ �ִϸ��̼�
    public GameObject minimiPrefab;
    public GameObject minimiSpawnPointParent;
    public GameObject minimiCenterPointParent;
    public GameObject minimiParent;

    SkeletonAnimation skAnim_Reading;
    public static List<string> animationNames;
    List<Vector3> spawnPoints;
    List<Vector3> centerPoints;

    void Start()
    {
        // �ִϸ��̼� �̸� ��� �ҷ�����
        skAnim_Reading = SystemManager.Instance.GetComponent<SkeletonAnimation>();
        animationNames = new List<string>();
        SkeletonData skeletonData = skAnim_Reading.Skeleton.Data;
        foreach (var animation in skeletonData.Animations)
        {
            if (!exceptAnimationNames.Any(except => animation.Name.ToLower().Contains(except.ToLower())))
                animationNames.Add(animation.Name);
        }

        // �������� �߰�
        spawnPoints = new List<Vector3>();
        foreach(Transform child in minimiSpawnPointParent.transform)
        {
            spawnPoints.Add(child.position);
        }

        // �߽����� �߰�
        centerPoints = new List<Vector3>();
        foreach (Transform child in minimiCenterPointParent.transform)
        {
            centerPoints.Add(child.position);
        }

        // �絵 ����
        foreach (string name in minimiNames)
        {
            Vector3 spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            GameObject minimi = Instantiate(minimiPrefab, spawnPoint, Quaternion.identity, minimiParent.transform);
            minimi.name = name;

            SkeletonAnimation skAnim = minimi.GetComponent<SkeletonAnimation>();
            skAnim.Skeleton.SetSkin(name);

            Minimi_Control control = minimi.GetComponent<Minimi_Control>();
            control.ChangeRandomAnim();
            Vector3 centerPoint = centerPoints[Random.Range(0, centerPoints.Count)];
            control.targetPos = GetRandomPosOnNavMesh(centerPoint, 0.5f);
            control.ChangeState<MoveTargetState>();
        }
    }

    public static Vector3 GetRandomPosOnNavMesh(Vector2 point, float range)
    {
        // NavMesh ���� ���� ��ġ�� hit�� ����
        NavMeshHit hit;
        int t = 0;
        while (t < 30)
        {
            Vector2 randomDirection = point + Random.insideUnitCircle * range;
            if (NavMesh.SamplePosition(randomDirection, out hit, range, NavMesh.AllAreas))
            {
                NavMeshHit edgeHit;
                if (NavMesh.FindClosestEdge(hit.position, out edgeHit, NavMesh.AllAreas))
                    if (edgeHit.distance > 0.1)
                        return hit.position;
            }
            t++;
        }
        return point;
    }
}
