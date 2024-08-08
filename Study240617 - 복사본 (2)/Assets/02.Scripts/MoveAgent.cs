using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


//�ش� ��Ʈ����Ʈ�� ���ؼ� �ʼ� ������Ʈ ����
[RequireComponent(typeof(MoveAgent))]
public class MoveAgent : MonoBehaviour  
{   //����Ʈ�� ���� �迭 ���̷μ� 
    //�����Ͱ� �߰�/���� �ʿ� ���� ���� �� �ε����� �ٲ�
    public List<Transform> wayPoints;
    public int nextIdx; //���� ��ȯ���� �ε���

    NavMeshAgent agent; 
    Transform enemyTr;
    Animator animator;
    EnemyFire enemyFire;
    EnemyAI enemyAI;
    

    //������Ƽ ����
    //readonly�� �б� �����̶� �� ���� �Ұ�
    readonly float patrolSpeed = 1.5f;
    readonly float traceSpeed = 4f;
    float damping = 1f; //ȸ�� �ӵ� ���� ���

    bool _patrolling; //���� ���� ����Ǵ� ���� 

    public bool patrolling //��������� ǥ��Ǵ� ������Ƽ
    {
        get 
        {
            return _patrolling;
        }
        set 
        {
            //value�� �ܺο��� ���޵Ǵ� ��
            _patrolling = value;
            if (_patrolling)
            {
                agent.speed = patrolSpeed;         
                MoveWayPoint();
            }
        }

    }
    Vector3 _traceTarget;
    internal bool isStopped;

    public Vector3 traceTarget
    {
        
        get { return _traceTarget; }
        set 
        {
            _traceTarget = value;
            agent.speed = traceSpeed;
            damping = 7f;
            //��� ���� �޼ҵ� ȣ��
            TraceTarget(_traceTarget);

        }
    }
    public float speed 
    {
        get { return agent.velocity.magnitude; }
    }

    void Start()
        
    {
        enemyAI = GetComponent<EnemyAI>();  
        //IsFire�� �������� ������
        enemyFire = GetComponent<EnemyFire>();

        enemyTr = GetComponent<Transform>();

        agent = GetComponent<NavMeshAgent>();
        //�������� ����������� �ӵ� ���̴� �ɼ� ��Ȱ��ȭ
        agent.autoBraking=false;

        agent.updateRotation = false;
        agent.speed = patrolSpeed;
        animator = GetComponent<Animator>();
      
        //���̾��Ű �信�� WayPointGroup �̸��� ������Ʈ �˻�
        //FInd�� ���� �ȱ� ������ ���� ���°� ��õ���� �ʴ´�
        var group = GameObject.Find("WayPointGroup");

       

        if (group != null) 
        {
            //WayPointGroup������ �ִ� ��� Transform������Ʈ ����
            //����� ������Ʈ�� List wayPoints�� �ڵ� �߰�
            //�ٸ� �̶��� ������ ���� ~~~s InChildern �޼ҵ��
            //�θ� ������Ʈ�� 0��°�� ���� ���� �ڽ��� ��
            group.GetComponentsInChildren<Transform>(wayPoints);
            //���� 0��° index��Ҹ� �������μ� �θ� ������Ʈ ����
            wayPoints.RemoveAt(0);
            //ù��° ���� ��ġ�� �������� ����
            nextIdx = Random.Range(0, wayPoints.Count);
        }
        //���� ����Ʈ�� �����̴� �޼ҵ� ȣ��
       // MoveWayPoint();
       this.patrolling = true;
    }
    void MoveWayPoint() 
    {
        //������Ʈ�� �ִܰ�� ��� ���̶�� �̵� ���� 
        if (agent.isPathStale)
            return;
        //��ΰ���� ��������...
        //���� �������� ����Ʈ���� ������ ��ġ�� ����
        agent.destination = wayPoints[nextIdx].position;
        
        //������Ʈ Ȱ��ȭ 
        agent.isStopped = false;
    }

    void TraceTarget(Vector3 pos)
    {
        if (agent.isPathStale)  
            return;

        agent.destination = pos;
        agent.isStopped = false;
    }
    public void Stop()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        _patrolling = false;
    }

    void Update()
    {
        if (!agent.isStopped)
        {
            //NavMeshAgent�� ������ ������ ���ʹϾ� ������ ��ȯ
            Quaternion rot = Quaternion.LookRotation(agent.desiredVelocity);
            //������ ����� ���� ������ ���� Enemy�� rotation �� ����
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot,
                Time.deltaTime * damping);
        }
        //�������°� �ƴ϶��(��������) �������� ������� ���� x
        if (!_patrolling) 
         return;
        //�����̴� ���̳� �������� ���� ������ ����
        //���� �������� �����ϱ� ���� �ܰ�
        if (agent.velocity.sqrMagnitude >= 0.2f * 0.2f &&
            agent.remainingDistance <= 0.5f)
        {
            //nextIdx++;
            //nextIdx= nextIdx % wayPoints.Count;
            //�ε����� ��ȯ�ϰ� ���� �����̼��� ���� ����.
            nextIdx = Random.Range(0, wayPoints.Count);

            MoveWayPoint();
        }
    }
    public void Stopped()
    {
        //���߰�.
        isStopped= true;
        //NavmeshAgent���� �ȿ����̰�.
        agent.isStopped= true;
        //�ӵ��� 0����
        agent.velocity = Vector3.zero;
        //agent.angularSpeed = 0f;
        //���� ����
        _patrolling = false;
        //�ִϸ��̼� ����
        animator.enabled = false;
        enemyAI.state = EnemyAI.State.PATROL;
        
    }
}
