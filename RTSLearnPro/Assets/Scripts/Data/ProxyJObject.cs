using System;
using Newtonsoft.Json.Linq;

namespace G13Kit
{
    public class ProxyJObject : IData
    {
        private JObject m_JObject;

        public ProxyJObject(JObject obj)
        {
            m_JObject = obj;
        }

        protected JToken this[string propertyName]
        {
            get => m_JObject[propertyName];
            set => m_JObject[propertyName] = value;
        }

        protected virtual T GetValue<T>(string key)
        {
            var value = m_JObject[key];

            if (!value.IsNullOrEmpty())
            {
                var obj = value.ToObject<T>();
                return obj;
            }

            Type type = typeof(T);
            object wantedValue = null;

            if (type == typeof(int))
            {
                wantedValue = int.MaxValue;
            }
            else if (type == typeof(string))
            {
                wantedValue = string.Empty;
            }

            return (T)wantedValue;
        }

        protected void ReleaseOrigin()
        {
            m_JObject = null;
        }
    }
}