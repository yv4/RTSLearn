using DragSelect;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectStatusUI : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public Transform PlayerListContent;

    private void Awake()
    {
        this.gameObject.SetActive(false);    
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MultiPlayerSelectCallBack(PlayerDatas selData)
    {
        this.gameObject.SetActive(true);

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
            selPlayerUI.SelIndexText.text = index.ToString();
            index++;
        }
    }

}
