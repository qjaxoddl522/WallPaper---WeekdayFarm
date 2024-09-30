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
        minimi.remainUpdateTime = Random.Range(minimi.updateIntervalMin, minimi.updateIntervalMax);
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
        minimi.agent.speed = Random.Range(minimi.speedMin, minimi.speedMax);
        minimi.agent.SetDestination(minimi.targetPos);
    }
    public override void ExitState()
    {
        // Debug.Log("����");
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
    public override void ExitState()
    {
        if (minimi.spawner.isOuter)
            minimi.remainFreeMove -= 1;
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
    public float updateIntervalMin;      // ��ǥ���� ���� �ֱ�
    public float updateIntervalMax;     
    public float updateDistance;         // ��ǥ���� ���� ����
    public float speedMin;               // �̵��ӵ� ����
    public float speedMax;
    public int remainFreeMove;           // ������ ������ ���� ���� �̵� Ƚ��
    [Header("Not User Settings")]
    public Vector3 targetPos;

    public NavMeshAgent agent;
    public float remainUpdateTime;       // ��ǥ���� ���ű��� ���� �ð�

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
    public bool IsMoveFree
    {
        get
        {
            return state.GetType() == typeof(MoveFreeState);
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
                if (remainFreeMove <= 0 && spawner.isOuter)
                    ChangeState<MoveOuterState>();
                else
                    ChangeState<MoveFreeState>();
            }
        }
        else if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (IsMoveTarget || IsMoveFree)
            {
                ChangeState<IdleState>();
            }
            else if (IsMoveOuter)
            {
                spawner.RefreshMinimi(gameObject);
            }
        }
        state.UpdateState();

        // �̵� ������� ����
        float velocityThreshold = 0.01f; // �ӵ� �Ӱ谪
        if (agent.velocity.x > velocityThreshold)
            transform.localScale = new Vector3(-initialScale.x, initialScale.y, initialScale.z);
        else if (agent.velocity.x < -velocityThreshold)
            transform.localScale = new Vector3(initialScale.x, initialScale.y, initialScale.z);

        // ���� ����
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
