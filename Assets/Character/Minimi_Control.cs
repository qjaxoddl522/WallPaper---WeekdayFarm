using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Minimi_Control : MonoBehaviour
{
    public float updateInterval;          // 목표지점 변경 주기
    public float updateDistance;          // 목표지점 변경 범위
    public string[] exceptAnimationNames; // 제외할 애니메이션

    NavMeshAgent agent;
    float lastUpdateTime;           // 마지막으로 목표지점 갱신한 시간
    Vector3 initialScale;

    SkeletonAnimation skAnim;
    List<string> animationNames;

    Vector2 mousePos;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        lastUpdateTime = updateInterval;
        initialScale = transform.localScale;

        // 애니메이션 이름 목록 불러오기
        skAnim = GetComponent<SkeletonAnimation>();
        animationNames = new List<string>();
        Spine.SkeletonData skeletonData = skAnim.Skeleton.Data;
        foreach (var animation in skeletonData.Animations)
        {
            if (!exceptAnimationNames.Any(except => animation.Name.ToLower().Contains(except.ToLower())))
                animationNames.Add(animation.Name);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            string randomAnimation = animationNames[Random.Range(0, animationNames.Count)];
            skAnim.AnimationState.SetAnimation(0, randomAnimation, true);
            //mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //agent.SetDestination(mousePos);
        }

        lastUpdateTime += Time.deltaTime;
        if (lastUpdateTime >= updateInterval)
        {
            Vector3 randomPos = GetRandomPosOnNavMesh(updateDistance);
            agent.SetDestination(randomPos);
            lastUpdateTime = 0;
        }

        // 이동 방향따라 반전
        if (agent.velocity.x > 0)
            transform.localScale = new Vector3(-initialScale.x, initialScale.y, initialScale.z);
        else if (agent.velocity.x < 0)
            transform.localScale = new Vector3(initialScale.x, initialScale.y, initialScale.z);
    }

    Vector3 GetRandomPosOnNavMesh(float distance)
    {
        Vector3 randomDirection = Random.insideUnitCircle * distance;
        randomDirection += transform.position;

        // NavMesh 위의 랜덤 위치를 hit에 저장
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, distance, NavMesh.AllAreas))
            return hit.position;
        else
            return transform.position;
    }
}
