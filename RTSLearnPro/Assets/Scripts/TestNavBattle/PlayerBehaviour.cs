using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerBehaviour : MonoBehaviour
{
    public Transform Target;
    public NavMeshAgent Agent;
    public GameObject AvoidObj;

    private RaycastHit m_HitInfo;
    private bool m_StartNav = false;
    private int m_ArcCount = 36;
    private Vector3[] m_WorldDirection;
    private Vector3 m_CurrentMoveTarget;
    private float m_ArcDegree;

    // Start is called before the first frame update
    void Start()
    {
        InitCircleDir();
    }

    // Update is called once per frame
    void Update()
    {
        CheckOtherPlayer();

        Vector3 distance = Target.transform.position - this.transform.position;
        if(distance.sqrMagnitude<3.2f)
        {
            Agent.enabled = false;
        }
        else
        {
            Agent.enabled = true;
            Agent.SetDestination(m_CurrentMoveTarget);
        }
        
    }

    private void CheckOtherPlayer()
    {
     
        Vector3 curDir = (Target.transform.position - this.transform.position).normalized;
        Ray ray = new Ray(this.transform.position, curDir);
        if (Physics.Raycast(ray,
                                                                     out m_HitInfo,
                                                                          2f,
                                                                 1 << LayerMask.NameToLayer("Player")
                                                                          ))
        {
            Debug.DrawRay(this.transform.position, curDir*10f, Color.blue);
            //Agent.enabled = false;
            Vector3 avoidDir = CheckNoObstacleDire(curDir).normalized;

             m_CurrentMoveTarget = this.transform.position + avoidDir*3;
          
        }
        else
        {
            m_CurrentMoveTarget = Target.position;
        }

        //AvoidObj.transform.position = m_CurrentMoveTarget;
    }

    private void InitCircleDir()
    {
        Target = GameObject.FindGameObjectWithTag("Enemy").transform;
        m_CurrentMoveTarget = Target.transform.position;
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

            //RaycastHit hitInfoLeft;
            //Ray leftRay = new Ray(rayCastPosition, GetDirection(leftIndex));
            //if (Physics.Raycast(leftRay, out hitInfoLeft, 1000))
            //{
            //    Debug.Log("ColliderLeft:" + hitInfoLeft.transform.name);
            //    choosenIndex = leftIndex;
            //    break;
            //}

            //RaycastHit hitInfoRight;
            //Ray rightRay = new Ray(rayCastPosition, GetDirection(rightIndex));
            //if (Physics.Raycast(rightRay, out hitInfoRight, 1000))
            //{
            //    Debug.Log("ColliderRight:" + hitInfoLeft.transform.name);
            //    choosenIndex = rightIndex;
            //    break;
            //}

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
