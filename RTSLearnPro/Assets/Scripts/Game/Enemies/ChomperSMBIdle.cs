using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChomperSMBIdle : SceneLinkedSMB<ChomperBehavior>
{
    public float minimumIdleGruntTime = 2.0f;
    public float maximumIdleGruntTime = 5.0f;

    protected float remainingToNextGrunt = 0.0f;

    public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnSLStateEnter(animator, stateInfo, layerIndex);

        if (minimumIdleGruntTime > maximumIdleGruntTime)
            minimumIdleGruntTime = maximumIdleGruntTime;

        remainingToNextGrunt = Random.Range(minimumIdleGruntTime, maximumIdleGruntTime);

        m_MonoBehaviour.controller.navmeshAgent.enabled = true;
    }

    public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex);

        remainingToNextGrunt -= Time.deltaTime;

        if(remainingToNextGrunt<0)
        {
            remainingToNextGrunt = Random.Range(minimumIdleGruntTime, maximumIdleGruntTime);
            m_MonoBehaviour.Grunt();
        }

        m_MonoBehaviour.FindTarget();
        if (m_MonoBehaviour.MoveTarget != Vector3.zero)
        {
            m_MonoBehaviour.StartPursuit();
        }
    }
}
