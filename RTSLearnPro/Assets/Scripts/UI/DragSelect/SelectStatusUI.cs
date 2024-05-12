using DragSelect;
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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CancelSelectCallBack()
    {
        this.gameObject.SetActive(false);

        foreach (Transform item in PlayerListContent)
        {
            GameObject.Destroy(item.gameObject);
        }
    }

    public void MultiPlayerSelectCallBack(PlayerDatas selData)
    {
        if(selData.CurrentSelPlayers.Count>0)
        {
          
            this.gameObject.SetActive(true);
            ResetLayout();

            int index = 1;

            foreach (Transform item in PlayerListContent)
            {
                GameObject.Destroy(item.gameObject);
            }

            foreach (var item in selData.CurrentSelPlayers)
            {
                GameObject player = GameObject.Instantiate<GameObject>(PlayerPrefab);
                player.transform.SetParent(PlayerListContent.transform);
                player.SetActive(true);
                player.transform.localPosition = Vector3.zero;
                player.transform.localScale = Vector3.one;

                SelPlayerListUI selPlayerUI = player.GetComponent<SelPlayerListUI>();
                selPlayerUI.DataIndex = index-1;
                selPlayerUI.SelIndexText.text = index.ToString();
                index++;
            }
        }
        else
        {
            CancelSelectCallBack();
        }
    }

    private void ResetLayout()
    {
        UserInfoContent.SetActive(false);
        UserStateContent.SetActive(true);
    }

    public void ShowPlayerDetail()
    {

    }

}
