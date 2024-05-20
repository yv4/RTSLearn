using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TargetDistributor : MonoBehaviour
{
    public Vector3 Target;
    public GameObject AvoidObj;
    public NavMeshAgent Agent;

    private RaycastHit m_HitInfo;
    private int m_ArcCount = 36;
    private Vector3[] m_WorldDirection;
    private Vector3 m_CurrentMoveTarget;
    private float m_ArcDegree;

    public Vector3 CurrentMoveTarget { get => m_CurrentMoveTarget; set => m_CurrentMoveTarget = value; }

    private void Awake()
    {
        InitCircleDir();
    }

    private void Update()
    {
        CheckOtherPlayer();

        //if (Target == Vector3.zero)
        //    return;

        //Vector3 distance = Target - this.transform.position;
        //if (distance.sqrMagnitude < 3.2f)
        //{
        //    Agent.enabled = false;
        //}
        //else
        //{
        //    Agent.enabled = true;
        //    Agent.SetDestination(m_CurrentMoveTarget);
        //}
    }

    private void InitCircleDir()
    {
        Agent = this.GetComponent<NavMeshAgent>();
        CurrentMoveTarget = Target;
        m_WorldDirection = new Vector3[m_ArcCount];
        float arcDegree = 360.0f / m_ArcCount;
        m_ArcDegree = arcDegree;
        Quaternion rotation = Quaternion.Euler(0, -arcDegree, 0);
        Vector3 currentDirection = Vector3.forward;
        for (int i = 0; i < m_ArcCount; i++)
        {
            m_WorldDirection[i] = currentDirection;
            currentDirection = rotation * currentDirection;
        }
    }

    public void CheckOtherPlayer()
    {
        Vector3 curDir = (Target - this.transform.position).normalized;
        Ray ray = new Ray(this.transform.position, curDir);
        if (Physics.Raycast(ray,
                                                                     out m_HitInfo,
                                                                          2f,
                                                                 1 << LayerMask.NameToLayer("Player")
                                                                          ))
        {
            Debug.DrawRay(this.transform.position, curDir * 10f, Color.blue);
            //Agent.enabled = false;
            Vector3 avoidDir = CheckNoObstacleDire(curDir).normalized;

            m_CurrentMoveTarget = this.transform.position + avoidDir * 3;

        }
        else
        {
            m_CurrentMoveTarget = Target;
        }

        AvoidObj.transform.position = m_CurrentMoveTarget;
    }

    private Vector3 CheckNoObstacleDire(Vector3 dir)
    {
        Vector3 rayCastPosition = transform.position + Vector3.up * 0.4f;
        float angle = Vector3.SignedAngle(dir, Vector3.forward, Vector3.up);
        if (angle < 0)
            angle = 360 + angle;

        int wantedIndex = Mathf.RoundToInt(angle / m_ArcDegree);
        if (wantedIndex >= m_WorldDirection.Length)
            wantedIndex -= m_WorldDirection.Length;

        int choosenIndex = wantedIndex;

        int offset = 1;
        int halfCount = m_ArcCount / 2;
        while (offset <= halfCount)
        {
            int leftIndex = wantedIndex - offset;
            int rightIndex = wantedIndex + offset;

            if (leftIndex < 0) leftIndex += m_ArcCount;
            if (rightIndex >= m_ArcCount) rightIndex -= m_ArcCount;

            if (!Physics.Raycast(rayCastPosition, GetDirection(leftIndex), 3))
            {
                choosenIndex = leftIndex;
                break;
            }

            if (!Physics.Raycast(rayCastPosition, GetDirection(rightIndex), 3))
            {
                choosenIndex = rightIndex;
                break;
            }

            offset += 1;
        }

        Vector3 selDir = GetDirection(choosenIndex);
        Debug.DrawLine(rayCastPosition, selDir * 20, Color.green);
        return selDir;
    }

    public Vector3 GetDirection(int index)
    {
        return m_WorldDirection[index];
    }
}

