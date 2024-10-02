using Spine.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public abstract class FSMBase
{
    public abstract void EnterState();
    public abstract void ExitState();
    public abstract void UpdateState();
}

public class IdleState : FSMBase
{
    readonly Minimi_Control minimi;
    bool isGetTargetPos;

    public IdleState(Minimi_Control minimi)
    {
        this.minimi = minimi;
        this.isGetTargetPos = false;
    }

    public override void EnterState()
    {
        minimi.remainUpdateTime = Random.Range(minimi.updateIntervalMin, minimi.updateIntervalMax);
    }
    public override void ExitState()
    {
        if (minimi.spawner.isOuter)
            minimi.remainFreeMove -= 1;
    }
    public override void UpdateState()
    {
        minimi.remainUpdateTime -= Time.deltaTime;
        if (!isGetTargetPos)
        {
            var result = minimi.spawner.GetRandomPosOnNavMesh(minimi.transform.position, minimi.updateDistance);
            minimi.targetPos = result.pos;
            isGetTargetPos = result.isSuccess;
        }
    }
}

public class MoveTargetState : FSMBase
{
    readonly Minimi_Control minimi;

    public MoveTargetState(Minimi_Control minimi)
    {
        this.minimi = minimi;
    }

    public override void EnterState()
    {
        minimi.agent.SetDestination(minimi.targetPos);
    }
    public override void ExitState()
    {
        // Debug.Log("도착");
    }
    public override void UpdateState() { }
}

public class MoveOuterState : FSMBase
{
    readonly Minimi_Control minimi;

    public MoveOuterState(Minimi_Control minimi)
    {
        this.minimi = minimi;
    }

    public override void EnterState()
    {
        minimi.agent.SetDestination(minimi.spawner.GetRandomSpawnPoint());
    }
    public override void ExitState() { }
    public override void UpdateState() { }
}

public class Minimi_Control : MonoBehaviour
{
    [Header("User Settings")]
    public float updateIntervalMin;      // 목표지점 변경 주기
    public float updateIntervalMax;     
    public float updateDistance;         // 목표지점 변경 범위
    public float speedMin;               // 이동속도 범위
    public float speedMax;
    public int remainFreeMove;           // 나가기 전까지 남은 자유 이동 횟수
    [Header("Not User Settings")]
    public Vector3 targetPos;

    public NavMeshAgent agent;
    public float remainUpdateTime;       // 목표지점 갱신까지 남은 시간

    SkeletonAnimation skAnim;
    MeshRenderer meshRenderer;
    Vector3 initialScale;
    public Minimi_Spawner spawner;

    FSMBase state;
    public bool IsIdle
    {
        get
        {
            return state.GetType() == typeof(IdleState);
        }
    }
    public bool IsMoveTarget
    {
        get
        {
            return state.GetType() == typeof(MoveTargetState);
        }
    }
    public bool IsMoveOuter
    {
        get
        {
            return state.GetType() == typeof(MoveOuterState);
        }
    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        skAnim = GetComponent<SkeletonAnimation>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        transform.rotation = Quaternion.identity;

        agent.speed = Random.Range(speedMin, speedMax);
        agent.autoRepath = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        initialScale = transform.localScale;

        if (state == null)
            state = new IdleState(this);
    }

    void Update()
    {
        if (IsIdle)
        {
            if (remainUpdateTime <= 0)
            {
                if (spawner.isOuter && remainFreeMove <= 0)
                    ChangeState<MoveOuterState>();
                else
                    ChangeState<MoveTargetState>();
            }
        }
        else if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (IsMoveTarget)
            {
                ChangeState<IdleState>();
            }
            else if (IsMoveOuter)
            {
                spawner.RefreshMinimi(gameObject);
            }
        }
        state.UpdateState();

        // 이동 방향따라 반전
        float velocityThreshold = 0.01f; // 속도 임계값
        if (agent.velocity.x > velocityThreshold)
            transform.localScale = new Vector3(-initialScale.x, initialScale.y, initialScale.z);
        else if (agent.velocity.x < -velocityThreshold)
            transform.localScale = new Vector3(initialScale.x, initialScale.y, initialScale.z);

        // 순서 조절
        meshRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }

    public void ChangeState<T>() where T : FSMBase
    {
        if (state != null && state.GetType() != typeof(T))
            state.ExitState();

        T t = (T)Activator.CreateInstance(typeof(T), args: this);
        this.state = t;
        t.EnterState();
    }

    public void ChangeRandomAnim()
    {
        string randomAnimation = Minimi_Spawner.animationNames[Random.Range(0, Minimi_Spawner.animationNames.Count)];
        skAnim.AnimationState.SetAnimation(0, randomAnimation, true);
    }
}
