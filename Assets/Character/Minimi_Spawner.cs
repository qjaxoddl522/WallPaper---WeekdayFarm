using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using Spine;

public class Minimi_Spawner : MonoBehaviour
{
    public string[] exceptAnimationNames; // 제외할 애니메이션
    public GameObject minimiPrefab;
    public GameObject minimiSpawnPointParent;
    public GameObject minimiCenterPointParent;
    public GameObject minimiParent;
    public int freeMoveNumMin;
    public int freeMoveNumMax;

    SkeletonAnimation skAnim_Reading;
    public static List<string> animationNames;
    List<Vector3> spawnPoints;
    List<Vector3> centerPoints;

    Queue<string> shuffledSkinsOrder;
    public bool isOuter = true;

    void Start()
    {
        // 애니메이션 설정
        skAnim_Reading = SystemManager.Instance.GetComponent<SkeletonAnimation>();
        animationNames = new List<string>();
        SkeletonData skeletonData = skAnim_Reading.Skeleton.Data;
        foreach (var animation in skeletonData.Animations)
        {
            if (!exceptAnimationNames.Any(except => animation.Name.ToLower().Contains(except.ToLower())))
                animationNames.Add(animation.Name);
        }

        // 생성지점 추가
        spawnPoints = new List<Vector3>();
        foreach(Transform child in minimiSpawnPointParent.transform)
        {
            spawnPoints.Add(child.position);
        }

        // 중심지점 추가
        centerPoints = new List<Vector3>();
        foreach (Transform child in minimiCenterPointParent.transform)
        {
            centerPoints.Add(child.position);
        }

        // 사도 생성
        List<string> skins = SystemManager.Instance.settings.selectedSkins.ToList();
        shuffledSkinsOrder = new Queue<string>(skins.OrderBy(s => Random.value));

        int minimiNum = SystemManager.Instance.settings.minimiNum;
        if (minimiNum > skins.Count)
        {
            minimiNum = skins.Count;
            isOuter = false;
        }

        for (int i=0; i<minimiNum; i++)
        {
            GameObject minimi = Instantiate(minimiPrefab, minimiParent.transform);
            minimi.GetComponent<Minimi_Control>().spawner = this;
            RefreshMinimi(minimi);
        }
    }

    public void RefreshMinimi(GameObject minimi)
    {
        string name = shuffledSkinsOrder.Dequeue();
        InitMinimi(minimi, name);
        shuffledSkinsOrder.Enqueue(name);
    }

    void InitMinimi(GameObject minimi, string name)
    {
        minimi.transform.position = GetRandomSpawnPoint();
        minimi.name = name;

        SkeletonAnimation skAnim = minimi.GetComponent<SkeletonAnimation>();
        skAnim.Skeleton.SetSkin(name);

        Minimi_Control control = minimi.GetComponent<Minimi_Control>();
        control.ChangeRandomAnim();
        Vector3 centerPoint = centerPoints[Random.Range(0, centerPoints.Count)];
        control.targetPos = GetRandomPosOnNavMesh(centerPoint, 0.5f).pos;
        control.remainFreeMove = Random.Range(freeMoveNumMin, freeMoveNumMax);
        control.ChangeState<MoveTargetState>();
    }

    public Vector3 GetRandomSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Count)];
    }

    public (Vector3 pos, bool isSuccess) GetRandomPosOnNavMesh(Vector2 point, float range)
    {
        NavMeshHit hit = new NavMeshHit();
        Vector2 randomDirection;
        int t = 0;
        while (t < 10)
        {
            randomDirection = point + Random.insideUnitCircle * range;
            if (NavMesh.SamplePosition(randomDirection, out hit, range, NavMesh.AllAreas))
            {
                NavMeshHit edgeHit;
                if (NavMesh.FindClosestEdge(hit.position, out edgeHit, NavMesh.AllAreas))
                {
                    if (edgeHit.distance > 0.01)
                        return (hit.position, true);
                }
            }
            t++;
        }
        return (hit.position, false);
    }
}
