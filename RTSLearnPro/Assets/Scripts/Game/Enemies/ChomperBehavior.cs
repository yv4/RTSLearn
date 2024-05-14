using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChomperBehavior : MonoBehaviour
{
    public static readonly int hashInPursuit = Animator.StringToHash("InPursuit");
    public static readonly int hashAttack = Animator.StringToHash("Attack");
    public static readonly int hashHit = Animator.StringToHash("Hit");
    public static readonly int hashVerticalDot = Animator.StringToHash("VerticalHitDot");
    public static readonly int hashHorizontalDot = Animator.StringToHash("HorizontalHitDot");
    public static readonly int hashThrown = Animator.StringToHash("Thrown");
    public static readonly int hashGrounded = Animator.StringToHash("Grounded");
    public static readonly int hashVerticalVelocity = Animator.StringToHash("VerticalVelocity");
    public static readonly int hashSpotted = Animator.StringToHash("Spotted");
    public static readonly int hashNearBase = Animator.StringToHash("NearBase");
    public static readonly int hashIdleState = Animator.StringToHash("ChomperIdle");

    //public EnemyController controller { get { return m_Controller; } }
    //public PlayerController target { get { return m_Target; } }

    public Vector3 originalPosition { get; protected set; }
    [System.NonSerialized]
    public float attackDistance = 3;

    //public TargetScanner playerScanner;
    public float timeToStopPursuit;

    protected float m_TimerSinceLostTarget = 0.0f;

    //protected PlayerController m_Target = null;
    //protected EnemyController m_Controller;
    //protected TargetDistributor.TargetFollower m_FollowerInstance = null;

    //public TargetDistributor.TargetFollower followerData { get { return m_FollowerInstance; } }

    protected void OnEnable()
    {
        //m_Controller = GetComponentInChildren<EnemyController>();
        originalPosition = transform.position;

        //m_Controller.animator.Play(hashIdleState, 0, Random.value);
        //SceneLinkedSMB<ChomperBehavior>.Initialise(m_Controller.animator, this);
    }

    private void PlayStep(int frontFoot)
    {
        
    }

    public void Grunt()
    {
        
    }

    public void Spotted()
    {
        
    }

    protected void OnDisable()
    {
        //if (m_FollowerInstance != null)
        //    m_FollowerInstance.distributor.UnregisterFollower(m_FollowerInstance);
    }

    private void FixedUpdate()
    {
        //m_Controller.animator.SetBool(hashGrounded, controller.grounded);

        Vector3 toBase = originalPosition - transform.position;
        toBase.y = 0;

        //m_Controller.animator.SetBool(hashNearBase, toBase.sqrMagnitude < 0.1 * 0.1f);
    }

    public void FindTarget()
    {
        //PlayerController target = playerScanner.Detect(transform, m_Target == null);

        //if (m_Target == null)
        //{
        //    if(target!=null)
        //    {
        //        m_Controller.animator.SetTrigger(hashSpotted);
        //        m_Target = target;
        //        TargetDistributor distributor = target.GetComponentInChildren<TargetDistributor>();
        //        if (distributor != null)
        //            m_FollowerInstance = distributor.RegisterNewFollower();
        //    }
        //}
        //else
        //{
        //    if(target==null)
        //    {
        //        m_TimerSinceLostTarget += Time.deltaTime;

        //        if(m_TimerSinceLostTarget>=timeToStopPursuit)
        //        {
        //            Vector3 toTarget = m_Target.transform.position - transform.position;

        //            if(toTarget.sqrMagnitude>playerScanner.detectionRadius*playerScanner.detectionRadius)
        //            {
        //                if (m_FollowerInstance != null)
        //                    m_FollowerInstance.distributor.UnregisterFollower(m_FollowerInstance);

        //                m_Target = null;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (target != m_Target)
        //        {
        //            if (m_FollowerInstance != null)
        //                m_FollowerInstance.distributor.UnregisterFollower(m_FollowerInstance);

        //            m_Target = target;

        //            TargetDistributor distributor = target.GetComponentInChildren<TargetDistributor>();
        //            if (distributor != null)
        //                m_FollowerInstance = distributor.RegisterNewFollower();
        //        }

        //        m_TimerSinceLostTarget = 0.0f;
        //    }
        //}
    }

    public void StartPursuit()
    {
        //if(m_FollowerInstance!=null)
        //{
        //    m_FollowerInstance.requireSlot = true;
        //    RequestTargetPosition();
        //}

        //m_Controller.animator.SetBool(hashInPursuit, true);
    }

    public void StopPursuit()
    {
        //if(m_FollowerInstance!=null)
        //{
        //    m_FollowerInstance.requireSlot = false;
        //}

        //m_Controller.animator.SetBool(hashInPursuit, false);
    }

    public void RequestTargetPosition()
    {
        //Vector3 fromTarget = transform.position - m_Target.transform.position;
        //fromTarget.y = 0;

        //m_FollowerInstance.requiredPoint = m_Target.transform.position + fromTarget.normalized * attackDistance * 0.9f;
    }
  
    public void WalkBackToBase()
    {
        //if (m_FollowerInstance != null)
        //    m_FollowerInstance.distributor.UnregisterFollower(m_FollowerInstance);

        //m_Target = null;
        //StopPursuit();
        //m_Controller.SetTarget(originalPosition);
        //m_Controller.SetFollowNavmeshAgent(true);
    }

    public void TriggerAttack()
    {
        //m_Controller.animator.SetTrigger(hashAttack);
    }

    public void AttackBegin()
    {

    }

    public void AttackEnd()
    {

    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        //playerScanner.EditorGizmo(transform);
    }
#endif
}
