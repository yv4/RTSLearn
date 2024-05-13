using Data;
using DragSelect;
using G13Kit;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectStatusUI : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject UserStateContent;
    public GameObject UserInfoContent;
    public Transform PlayerListContent;

    public PlayerDetailUI m_PlayerDetailUI;

    private const String TableName = "PlayerData";

    public void CancelSelectCallBack()
    {
        this.gameObject.SetActive(false);

        foreach (Transform item in PlayerListContent)
        {
            GameObject.Destroy(item.gameObject);
        }
    }

    public void MultiPlayerSelectCallBack(SelPlayerDatas selData)
    {
        if(selData.CurrentSelPlayers.Count>0)
        {
          
            this.gameObject.SetActive(true);
            ResetLayout();


            foreach (Transform item in PlayerListContent)
            {
                GameObject.Destroy(item.gameObject);
            }

            foreach (var item in selData.CurrentSelPlayers)
            {
                int index = item.DataIndex;
                GameObject player = GameObject.Instantiate<GameObject>(PlayerPrefab);
                player.transform.SetParent(PlayerListContent.transform);
                player.SetActive(true);
                player.transform.localPosition = Vector3.zero;
                player.transform.localScale = Vector3.one;

                SelPlayerListUI selPlayerUI = player.GetComponent<SelPlayerListUI>();
                selPlayerUI.SelIndexText.text = index.ToString();
                selPlayerUI.Index = index;
            }
        }
        else
        {
            CancelSelectCallBack();
        }
    }

    public void ShowPlayerDetailCallBack(int index)
    {
        PlayerData player = DataManager.Instance.GetDataFromTableById<PlayerData>(TableName, index);
        m_PlayerDetailUI.SetContent(player.Name, player.HP);
    }


    private void ResetLayout()
    {
        UserInfoContent.SetActive(false);
        UserStateContent.SetActive(true);
    }


}
