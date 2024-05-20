using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChomperSMBPursuit : SceneLinkedSMB<ChomperBehavior>
{
    public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnSLStateEnter(animator, stateInfo, layerIndex);

        m_MonoBehaviour.GetComponent<TargetDistributor>().Target = m_MonoBehaviour.MoveTarget;
    }
    public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex);
        m_MonoBehaviour.FindTarget();

        if (m_MonoBehaviour.controller.navmeshAgent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathPartial
            ||m_MonoBehaviour.controller.navmeshAgent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid)
        {
            m_MonoBehaviour.StopPursuit();
            return;
        }

        if (m_MonoBehaviour.MoveTarget == Vector3.zero)
        {
            m_MonoBehaviour.StopPursuit();
        }
        else
        {
            Vector3 toTarget = m_MonoBehaviour.MoveTarget;
          
            toTarget = m_MonoBehaviour.GetComponent<TargetDistributor>().CurrentMoveTarget;

            Debug.Log("MoveTarget:" + m_MonoBehaviour.MoveTarget);
            Vector3 disV3 = m_MonoBehaviour.MoveTarget - m_MonoBehaviour.transform.position;
            if (disV3.sqrMagnitude < 3.2f)
            {
                Debug.Log("Stop");
                m_MonoBehaviour.StopPursuit();
            }
            else
            {

                m_MonoBehaviour.controller.SetTarget(toTarget);
                // m_MonoBehaviour.StopPursuit(true);
            }


        }
    }
}
