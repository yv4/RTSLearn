using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : MonoBehaviour
{
    public GameObject SelObj;
    public int DataIndex;

    private void Awake()
    {
        ShowSelect(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowSelect(bool sel)
    {
        SelObj.gameObject.SetActive(sel);
    }
}
