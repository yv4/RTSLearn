using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReleaseTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            //释放内存资源 在跳转场景后内存中的资源真正清除
            ResourceManager.Instance.ReleaseAssetsByLabel("TestPrefabs");
        }

        if(Input.GetKeyDown(KeyCode.B))
        {
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        }
    }
}
