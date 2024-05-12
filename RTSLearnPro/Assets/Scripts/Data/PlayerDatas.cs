using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDatas : MonoBehaviour
{
    public List<Soldier> CurrentSelPlayers = new List<Soldier>();

    public void ResetState()
    {
        foreach (var item in CurrentSelPlayers)
        {
            item.ShowSelect(false);
        }
    }

    public void ClearSelData()
    {

        ResetState();
        CurrentSelPlayers.Clear();
    }
}
