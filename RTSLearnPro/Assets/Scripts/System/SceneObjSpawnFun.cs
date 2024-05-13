using Data;
using G13Kit;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BaseSystemFun
{
    public class SceneObjSpawnFun : MonoBehaviour
    {
        public Transform PlayerParent;

        private void Awake()
        {
            SpawnPlayer();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void SpawnPlayer()
        {
            var table = DataManager.Instance.GetTableByName("PlayerData");
            foreach (var item in table)
            {
                PlayerData data = item.As<PlayerData>();
    
                GameObject prefab = Resources.Load<GameObject>("Chomper");
                GameObject obj = GameObject.Instantiate(prefab);
                obj.transform.SetParent(PlayerParent);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = data.GetPos();
                obj.GetComponent<Soldier>().DataIndex = data.ID;
            }
        }
    }
}

