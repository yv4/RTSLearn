using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragSelectFun : MonoBehaviour
{
    private bool m_IsMouseDown = false;
    private LineRenderer m_Line;

    private Vector3 m_LeftUpPoint;
    private Vector3 m_RightUpPoint;
    private Vector3 m_LeftDownPoint;
    private Vector3 m_RightDownPoint;
    private Vector3 m_BeginWorldPos;
    private Vector3 m_FrontPos = Vector3.zero;

    private RaycastHit m_HitInfo;
    private float m_SoldierOffset = 2;
    private List<Soldier> m_Soldiers = new List<Soldier>();

    // Start is called before the first frame update
    void Start()
    {
        m_Line = this.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetSoldiers()
    {
        if(Input.GetMouseButtonDown(0))
        {
            m_LeftUpPoint = Input.mousePosition;
            m_IsMouseDown = true;

        }
        else if(Input.GetMouseButtonUp(0))
        {
            m_IsMouseDown = false;
            m_Line.positionCount = 0;
            m_FrontPos = Vector3.zero;
        }

        if(m_IsMouseDown)
        {
            m_LeftUpPoint.z = 5;
            m_RightDownPoint = Input.mousePosition;
            m_RightDownPoint.z = 5;

            m_RightUpPoint.x = m_RightDownPoint.x;
            m_RightUpPoint.y = m_LeftUpPoint.y;
            m_RightUpPoint.z = 5;

            m_LeftDownPoint.x = m_LeftUpPoint.x;
            m_LeftDownPoint.y = m_RightDownPoint.y;
            m_LeftDownPoint.z = 5;

            m_Line.positionCount = 4;
            m_Line.SetPosition(0, Camera.main.ScreenToWorldPoint(m_LeftUpPoint));
            m_Line.SetPosition(1, Camera.main.ScreenToWorldPoint(m_RightUpPoint));
            m_Line.SetPosition(2, Camera.main.ScreenToWorldPoint(m_RightDownPoint));
            m_Line.SetPosition(3, Camera.main.ScreenToWorldPoint(m_LeftDownPoint));
        }
    }
}
