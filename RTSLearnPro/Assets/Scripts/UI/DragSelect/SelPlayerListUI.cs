using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelPlayerListUI : MonoBehaviour
{
    public int Index;
    public Text SelIndexText;

    public ShowPlayerDetailEvent OnShowPlayerDetail;

    public void ShowPlayerDetail()
    {
        OnShowPlayerDetail.Invoke(Index);
    }

    #region Event

    [Serializable]
    public class ShowPlayerDetailEvent : UnityEvent<int>
    { }

    #endregion
}
