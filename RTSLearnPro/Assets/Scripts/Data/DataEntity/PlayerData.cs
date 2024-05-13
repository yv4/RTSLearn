using G13Kit;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class PlayerData : ProxyJObject
    {
        public int ID { get; private set;}
        public string Name { get; private set; }
        public float HP { get; private set; }

        public string Pos { get; private set; }

        public PlayerData(JObject obj):base(obj)
        {
            try
            {
                ID = GetValue<int>("ID");
                Name = GetValue<string>("Name");
                HP = GetValue<float>("HP");
                Pos = GetValue<string>("Pos");
                ReleaseOrigin();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        protected override T GetValue<T>(string key)
        {
            return base.GetValue<T>(key);
        }

        public Vector3 GetPos()
        {
              string[] posVal = Pos.Split(';');
                return new Vector3(float.Parse(posVal[0]), float.Parse(posVal[1]), float.Parse(posVal[2]));
    }
    }
}

