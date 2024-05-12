using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomFun
{
    public class UILayoutFun : MonoBehaviour
    {
        public List<UILayoutElement> Elements = new List<UILayoutElement>();

        private void Awake()
        {
            foreach (var item in Elements)
            {
                item.Element.SetActive(item.Show);
            }
        }
    }

    [Serializable]
    public class UILayoutElement
    {
        public bool Show;
        public GameObject Element;
    }

}

