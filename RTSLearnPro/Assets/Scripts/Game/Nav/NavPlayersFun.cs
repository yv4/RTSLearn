using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavPlayersFun : MonoBehaviour
{
    public GameObject[] Players;
    public GameObject EnemyObj;
    private List<Soldier> m_Solders;
    private RaycastHit m_HitInfo;
    private float m_SoliderOffset = 2;

    private Vector3 m_FrontPos = Vector3.zero;
    public void ControlSoliderMove(List<Soldier> soldierObjs)
    {

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out m_HitInfo, 1000, 1 << LayerMask.NameToLayer(GameConst.GroundLayerName)))
        {
            m_Solders = soldierObjs;

            if (CheckEnemy(m_HitInfo.point))
                return;

            Vector3 oldDir = (m_HitInfo.point - soldierObjs[0].transform.position).normalized;
            Vector3 newDir = soldierObjs[0].transform.forward;

            if (Vector3.Angle(oldDir, newDir) > 60)
            {
                soldierObjs.Sort((a, b) =>
                {
                    if (Vector3.Distance(a.transform.position, m_HitInfo.point) <=
                       Vector3.Distance(b.transform.position, m_HitInfo.point))
                    {
                        return -1;
                    }
                    else return 1;
                });
            }

            GetTargetPos(m_HitInfo.point);
        }
    }

    private bool CheckEnemy(Vector3 center)
    {
        
        Vector3 halfExtents = new Vector3(1.5f, 1, 1.5f);

        Collider[] colliders = Physics.OverlapBox(center,
           halfExtents,
           Quaternion.identity,
           1 << LayerMask.NameToLayer(GameConst.EnemyLayerName)
           );

        for (int i = 0; i < colliders.Length; i++)
        {
            Debug.Log("Enemy:" + colliders[i].transform.name);
            Vector3 targetPos = colliders[i].transform.position;
            EnemyObj.transform.position = targetPos;

            foreach (var soldier in m_Solders)
            {
                soldier.GetComponent<ChomperBehavior>().MoveTarget = targetPos;
                soldier.GetComponent<TargetDistributor>().Target = targetPos;
            }

            return true;
        }

      

        return false;
    }

    private void GetTargetPos(Vector3 targetPos)
    {
        Vector3 nowForward = Vector3.zero;
        Vector3 nowRight = Vector3.zero;

        if (m_FrontPos != Vector3.zero)
        {
            nowForward = (targetPos - m_FrontPos).normalized;
        }
        else
        {
            nowForward = (targetPos - m_Solders[0].transform.position).normalized;
        }

        nowRight = Quaternion.Euler(0, 90, 0) * nowForward;
        List<Vector3> targetsPos = new List<Vector3>();

        switch (m_Solders.Count)
        {
            case 1:
                targetsPos.Add(targetPos);
                break;
            case 2:
                targetsPos.Add(targetPos + nowRight * m_SoliderOffset / 2);
                targetsPos.Add(targetPos - nowRight * m_SoliderOffset / 2);
                break;
            case 3:
                targetsPos.Add(targetPos);
                targetsPos.Add(targetPos + nowRight * m_SoliderOffset);
                targetsPos.Add(targetPos - nowRight * m_SoliderOffset);
                break;
            case 4:
                targetsPos.Add(targetPos + nowForward * m_SoliderOffset / 2 - nowRight * m_SoliderOffset / 2);
                targetsPos.Add(targetPos + nowForward * m_SoliderOffset / 2 + nowRight * m_SoliderOffset / 2);
                targetsPos.Add(targetPos - nowForward * m_SoliderOffset / 2 - nowRight * m_SoliderOffset / 2);
                targetsPos.Add(targetPos - nowForward * m_SoliderOffset / 2 + nowRight * m_SoliderOffset / 2);
                break;
            case 5:
                targetsPos.Add(targetPos + nowForward * m_SoliderOffset / 2);
                targetsPos.Add(targetPos + nowForward * m_SoliderOffset / 2 - nowRight * m_SoliderOffset / 2);
                targetsPos.Add(targetPos + nowForward * m_SoliderOffset / 2 + nowRight * m_SoliderOffset / 2);
                targetsPos.Add(targetPos - nowForward * m_SoliderOffset / 2 - nowRight * m_SoliderOffset / 2);
                targetsPos.Add(targetPos - nowForward * m_SoliderOffset / 2 + nowRight * m_SoliderOffset / 2);
                break;
            case 6:
                targetsPos.Add(targetPos + nowForward * m_SoliderOffset / 2);
                targetsPos.Add(targetPos + nowForward * m_SoliderOffset / 2 - nowRight * m_SoliderOffset / 2);
                targetsPos.Add(targetPos + nowForward * m_SoliderOffset / 2 + nowRight * m_SoliderOffset / 2);
                targetsPos.Add(targetPos - nowForward * m_SoliderOffset / 2 - nowRight * m_SoliderOffset / 2);
                targetsPos.Add(targetPos - nowForward * m_SoliderOffset / 2 + nowRight * m_SoliderOffset / 2);
                targetsPos.Add(targetPos - nowForward * m_SoliderOffset / 2);
                break;

            default:
                break;
        }

        m_FrontPos = targetPos;

        int index = 0;
        foreach (var item in m_Solders)
        {
            Players[index].transform.localPosition = targetsPos[index];
            item.GetComponent<ChomperBehavior>().MoveTarget = targetsPos[index];
            index++;
        }
    }
}
