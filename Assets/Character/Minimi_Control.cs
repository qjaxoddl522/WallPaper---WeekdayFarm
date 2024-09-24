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

    public IdleState(Minimi_Control minimi)
    {
        this.minimi = minimi;
    }

    public override void EnterState()
    {
        minimi.remainUpdateTime = Random.Range(minimi.updateIntervalLeft, minimi.updateIntervalRight);
    }
    public override void ExitState() { }
    public override void UpdateState()
    {
        minimi.remainUpdateTime -= Time.deltaTime;
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

public class MoveFreeState : FSMBase
{
    readonly Minimi_Control minimi;

    public MoveFreeState(Minimi_Control minimi)
    {
        this.minimi = minimi;
    }

    public override void EnterState()
    {
        Vector3 randomPos = Minimi_Spawner.GetRandomPosOnNavMesh(minimi.transform.position, minimi.updateDistance);
        minimi.agent.SetDestination(randomPos);
    }
    public override void ExitState() { }
    public override void UpdateState() { }
}


public class Minimi_Control : MonoBehaviour
{
    public float updateIntervalLeft;      // 목표지점 변경 주기
    public float updateIntervalRight;     
    public float updateDistance;          // 목표지점 변경 범위
    public Vector3 targetPos;

    public NavMeshAgent agent;
    public Vector2 mousePos;
    public float remainUpdateTime;        // 목표지점 갱신까지 남은 시간

    SkeletonAnimation skAnim;
    MeshRenderer meshRenderer;
    Vector3 initialScale;

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
    public bool IsMoveFree
    {
        get
        {
            return state.GetType() == typeof(MoveFreeState);
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

        agent.autoRepath = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        initialScale = transform.localScale;

        if (state == null)
            state = new IdleState(this);
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            //ChangeRandomAnim();
            //mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //agent.SetDestination(mousePos);
        }

        if (IsMoveTarget || IsMoveFree)
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!IsIdle)
                    ChangeState<IdleState>();
            }
        }
        else if (remainUpdateTime <= 0)
        {
            if (!IsMoveFree)
                ChangeState<MoveFreeState>();
        }
        UpdateState();

        // 이동 방향따라 반전
        if (agent.velocity.x > 0)
            transform.localScale = new Vector3(-initialScale.x, initialScale.y, initialScale.z);
        else if (agent.velocity.x < 0)
            transform.localScale = new Vector3(initialScale.x, initialScale.y, initialScale.z);
        // 순서 조절
        meshRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }

    void UpdateState()
    {
        state.UpdateState();
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
