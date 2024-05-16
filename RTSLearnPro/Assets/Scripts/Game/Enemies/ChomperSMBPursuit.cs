using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChomperSMBPursuit : SceneLinkedSMB<ChomperBehavior>
{
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

        if (m_MonoBehaviour.MoveTarget == null)
        {
            m_MonoBehaviour.StopPursuit();
        }
        else
        {
            Vector3 toTarget = m_MonoBehaviour.MoveTarget;
            Vector3 disV3 = toTarget - m_MonoBehaviour.transform.position;
            if (disV3.sqrMagnitude < 0.005f)
            {
                Debug.Log("StopTarget:" + toTarget);
                m_MonoBehaviour.StopPursuit();
            }
            else
            {

                m_MonoBehaviour.controller.SetTarget(toTarget);
            }


        }
    }
}
