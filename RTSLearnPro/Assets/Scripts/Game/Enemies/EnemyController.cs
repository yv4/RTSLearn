using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    public bool interpolateTurning = false;
    public bool applyAnimationRotation = false;

    public Animator animator { get { if(m_Animator==null) m_Animator = GetComponent<Animator>();return m_Animator; } }
    public Vector3 externalForce { get { return m_ExternalForce; } }
    public NavMeshAgent navmeshAgent { get { return m_NavMeshAgent; } }
    public bool followNavmeshAgent { get { return m_FollowNavmeshAgent; } }
    public bool grounded { get { return m_Grounded; } }

    protected NavMeshAgent m_NavMeshAgent;
    protected bool m_FollowNavmeshAgent;
    protected Animator m_Animator;
    protected bool m_UnderExternalForce;
    protected bool m_ExternalForceAddGravity = true;
    protected Vector3 m_ExternalForce;
    protected bool m_Grounded;

    protected Rigidbody m_Rigibody;

    private const float k_GroundedRayDistance = 0.8f;

    private void OnEnable()
    {
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();
        m_Animator.updateMode = AnimatorUpdateMode.AnimatePhysics;

        m_NavMeshAgent.updatePosition = false;

        //m_Rigibody = GetComponentInChildren<Rigidbody>();
        //if (m_Rigibody == null)
        //    m_Rigibody = gameObject.AddComponent<Rigidbody>();

        //m_Rigibody.isKinematic = true;
        //m_Rigibody.useGravity = false;
        //m_Rigibody.interpolation = RigidbodyInterpolation.Interpolate;

        m_FollowNavmeshAgent = true;
    }

    private void FixedUpdate()
    {
        animator.speed = 1;

        CheckGrounded();

        if (m_UnderExternalForce)
            ForceMovement();
    }

    private void CheckGrounded()
    {
        RaycastHit hit;

        Ray ray = new Ray(transform.position + Vector3.up * k_GroundedRayDistance * 0.5f, -Vector3.up);
        m_Grounded = Physics.Raycast(ray, out hit, k_GroundedRayDistance, Physics.AllLayers,QueryTriggerInteraction.Ignore);
    }

    private void ForceMovement()
    {
        Debug.Log("ForceMove");
        if (m_ExternalForceAddGravity)
            return;

        if(m_FollowNavmeshAgent)
        {
            m_NavMeshAgent.speed = (m_Animator.deltaPosition / Time.deltaTime).magnitude;
            transform.position = m_NavMeshAgent.nextPosition;
        }
        else
        {
            RaycastHit hit;
            if (!m_Rigibody.SweepTest(m_Animator.deltaPosition.normalized, out hit,
                       m_Animator.deltaPosition.sqrMagnitude))
            {
                m_Rigibody.MovePosition(m_Rigibody.position + m_Animator.deltaPosition);
            }
        }

        m_NavMeshAgent.Warp(m_Rigibody.position);
    }

    private void OnAnimatorMove()
    {
        if (m_UnderExternalForce)
            return;

        if(m_FollowNavmeshAgent)
        {
            m_NavMeshAgent.speed = (m_Animator.deltaPosition / Time.deltaTime).magnitude;
            transform.position = m_NavMeshAgent.nextPosition;
        }
        else
        {
            RaycastHit hit;
            if (!m_Rigibody.SweepTest(m_Animator.deltaPosition.normalized, out hit,
                    m_Animator.deltaPosition.sqrMagnitude))
            {
                m_Rigibody.MovePosition(m_Rigibody.position + m_Animator.deltaPosition);
            }
        }

        if(applyAnimationRotation)
        {
            transform.forward = m_Animator.deltaRotation * transform.forward;
        }
    }
    
    public void SetFollowNavmeshAgent(bool follow)
    {
        if(!follow&&m_NavMeshAgent.enabled)
        {
            m_NavMeshAgent.ResetPath();
        }
        else if(follow&&!m_NavMeshAgent.enabled)
        {
            m_NavMeshAgent.Warp(transform.position);
        }

        m_FollowNavmeshAgent = follow;
        m_NavMeshAgent.enabled = follow;
    }

    public void AddForce(Vector3 force,bool useGravity=true)
    {
        if (m_NavMeshAgent.enabled)
            m_NavMeshAgent.ResetPath();

        m_ExternalForce = force;
        m_NavMeshAgent.enabled = false;
        m_UnderExternalForce = true;
        m_ExternalForceAddGravity = useGravity;
    }

    public void ClearForce()
    {
        m_UnderExternalForce = false;
        m_NavMeshAgent.enabled = true;
    }

    public void SetForward(Vector3 forward)
    {
        Quaternion targetRotation = Quaternion.LookRotation(forward);

        if(interpolateTurning)
        {
            targetRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, m_NavMeshAgent.angularSpeed * Time.deltaTime);
        }

        transform.rotation = targetRotation;
    }

    public bool SetTarget(Vector3 position)
    {
        return m_NavMeshAgent.SetDestination(position);
       
    }
}
