using Data;
using G13Kit;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace System
{
    public class SceneObjSpawnFun : MonoBehaviour
    {
        private void Awake()
        {
            var table = DataManager.Instance.GetTableByName("PlayerData");
            var item = table.FirstOrDefault(item => (int)item["ID"] == 1);
            if (item != null)
            {
                PlayerData playerData = item.As<PlayerData>();
                Debug.Log("PlayerName:" + playerData.Name);
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

