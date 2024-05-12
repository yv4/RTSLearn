using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace DragSelect
{

    public class DragSelectFun : MonoBehaviour
    {
        private bool m_IsMouseDown = false;
        private bool m_SingleSel = false;

        private Vector3 m_LeftUpPoint;
        private Vector3 m_RightUpPoint;
        private Vector3 m_LeftDownPoint;
        private Vector3 m_RightDownPoint;
        private Vector3 m_BeginWorldPos;
        private Vector3 m_FrontPos = Vector3.zero;

        private const string GroundLayerName = "Ground";
        private const string PlayerLayerName = "Player";

        private RaycastHit m_HitInfo;
        private float m_SoldierOffset = 2;
        private PlayerDatas m_SelData;
        private LineRenderer m_Line;

        public SelectPlayersEvent OnPlayersSelect;
        public CancelSelectPlayersEvent OnCancelPlayerSelect;

        private void Awake()
        {
            m_SelData = this.GetComponent<PlayerDatas>();    
        }

        // Start is called before the first frame update
        void Start()
        {
            m_Line = this.GetComponent<LineRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            SetSoldiers();
        }

        private void SetSoldiers()
        {
            if (Input.GetMouseButtonDown(0))
            {
            

                //if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),
                //                                                      out m_HitInfo,
                //                                                           1000,
                //                                                  1 << LayerMask.NameToLayer(PlayerLayerName)
                //                                                           ))
                //{
                //    m_LeftDownPoint = Vector3.zero;
                //    m_IsMouseDown = false;
                //    m_Line.positionCount = 0;
                //    m_SingleSel = true;
                //}
                //else
                //{
                //    m_LeftDownPoint = Vector3.zero;
                //    m_IsMouseDown = false;
                //    Debug.Log("UnSelect");
                //    m_SelData.ClearSelData();
                //    OnCancelPlayerSelect.Invoke();
                //    m_SingleSel = false;
                //}

                if (!m_SingleSel)
                {
                    m_LeftUpPoint = Input.mousePosition;
                    m_IsMouseDown = true;

                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),
                                                                      out m_HitInfo,
                                                                           1000,
                                                                  1 << LayerMask.NameToLayer(GroundLayerName)
                                                                           )
                        &&!EventSystem.current.IsPointerOverGameObject())
                    {

                        m_BeginWorldPos = m_HitInfo.point;
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0)&&!m_SingleSel)
            {
                m_IsMouseDown = false;
                m_Line.positionCount = 0;
                m_FrontPos = Vector3.zero;

                SelectLaunch();
            }

            if (m_IsMouseDown&&!m_SingleSel)
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

        private void SelectLaunch()
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),
                  out m_HitInfo,
                  1000,
                 1 << LayerMask.NameToLayer(GroundLayerName))
                 &&!EventSystem.current.IsPointerOverGameObject())
            {
                if (m_SelData.CurrentSelPlayers != null)
                    m_SelData.ClearSelData();

                Vector3 center = new Vector3((m_HitInfo.point.x + m_BeginWorldPos.x) / 2, 1, (m_HitInfo.point.z + m_BeginWorldPos.z) / 2);
                Vector3 halfExtents = new Vector3(Mathf.Abs(m_HitInfo.point.x - m_BeginWorldPos.x) / 2, 1, Mathf.Abs(m_HitInfo.point.x - m_BeginWorldPos.x) / 2);


                if(!EventSystem.current.IsPointerOverGameObject())
                {
                    Collider[] colliders = Physics.OverlapBox(center,
                   halfExtents,
                   Quaternion.identity,
                   1 << LayerMask.NameToLayer(PlayerLayerName)
                   );
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        Soldier obj = colliders[i].GetComponent<Soldier>();

                        if (obj != null)
                        {
                            obj.ShowSelect(true);
                            m_SelData.CurrentSelPlayers.Add(obj);
                        }
                    }

                    OnPlayersSelect.Invoke(m_SelData);
                }    
            }
        }

        #region Event

        [Serializable]
        public class SelectPlayersEvent : UnityEvent<PlayerDatas>
        { }

        [Serializable]
        public class CancelSelectPlayersEvent : UnityEvent
        { }

        #endregion
    }

}



