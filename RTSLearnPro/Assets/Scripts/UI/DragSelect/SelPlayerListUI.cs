using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelPlayerListUI : MonoBehaviour
{
    public Text SelIndexText;

    public ShowPlayerDetailEvent OnShowPlayerDetail;

    public int DataIndex;


    public void ShowPlayerDetail()
    {
        OnShowPlayerDetail.Invoke();
    }

    #region Event

    [Serializable]
    public class ShowPlayerDetailEvent : UnityEvent
    { }

    #endregion
}
