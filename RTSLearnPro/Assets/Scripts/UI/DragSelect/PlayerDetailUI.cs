using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDetailUI : MonoBehaviour
{
    public Text NameText;
    public Text LifeText;

    public void SetContent(string name,float life)
    {
        NameText.text = name;
        LifeText.text = life.ToString();
    }
}
