using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardStats : MonoBehaviour
{
    public Text title;
    public Text score;
    private void OnValidate() {
        title = (Text) this.transform.Find("name").GetComponent<Text>();
        score = (Text) this.transform.Find("score").GetComponent<Text>();
    }
}