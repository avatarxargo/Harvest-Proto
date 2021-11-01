using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResboardStats : MonoBehaviour
{
    public Image title;
    public Text score;
    private void OnValidate() {
        title = (Image) this.transform.Find("name").GetComponent<Image>();
        score = (Text) this.transform.Find("score").GetComponent<Text>();
    }
}