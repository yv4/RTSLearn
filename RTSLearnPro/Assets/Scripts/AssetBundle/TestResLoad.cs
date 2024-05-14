using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestResLoad : MonoBehaviour
{
    // Start is called before the first frame update

    private GameObject m_Obj;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            GameObject obj = ResourceManager.Instance.LoadAddressableAssetByNameAndLabel<GameObject>("Assets/Prefabs/TestCube.prefab", "TestPrefabs");
            m_Obj = GameObject.Instantiate(obj);
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            GameObject.Destroy(m_Obj);
            m_Obj = null;
            //释放内存资源 在跳转场景后内存中的资源真正清除
            ResourceManager.Instance.ReleaseAssetsByLabel("TestPrefabs");
        }

        if(Input.GetKeyDown(KeyCode.T))
        {
            SceneManager.LoadScene("ToScene",LoadSceneMode.Single);
        }
    }
}
