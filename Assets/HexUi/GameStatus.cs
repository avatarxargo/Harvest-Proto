using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStatus : MonoBehaviour
{

    public Color playerColor;
    public Color playerAltColor;
    public float barWidth = 30;

    public PlayerData playerData;
    public ScoreboardStats[] playerScores;
    public ResboardStats[] playerResources;

    public RectTransform barL;
    public RectTransform barR;
    public RectTransform barB;
    public RectTransform barT;
    public Text activePlayerDisplay;
    public Image activePlayerDisplayFrame;

    private void OnValidate()
    {
        barL = (RectTransform) this.transform.Find("playerColor L").gameObject.GetComponent<RectTransform>();
        barR = (RectTransform) this.transform.Find("playerColor R").gameObject.GetComponent<RectTransform>();
        barB = (RectTransform) this.transform.Find("playerColor B").gameObject.GetComponent<RectTransform>();
        barT = (RectTransform) this.transform.Find("playerColor T").gameObject.GetComponent<RectTransform>();
        activePlayerDisplay = (Text) this.transform.Find("ap holder").transform.Find("active player").gameObject.GetComponent<Text>();
        activePlayerDisplayFrame = (Image) this.transform.Find("ap holder").GetComponent<Image>();
        playerScores = this.transform.Find("scoreboard").GetComponentsInChildren<ScoreboardStats>(true);
        playerResources = this.transform.Find("resboard").GetComponentsInChildren<ResboardStats>();
        for(int i = 0; i < 4; ++i) {
            Color col = playerData.playerInfo[i].playerColor;
            col.r *= 1.5f;
            col.g *= 1.5f;
            col.b *= 1.5f;
            col.a = 1;
            playerScores[i].title.text = playerData.playerInfo[i].title;
            playerScores[i].title.color = col;
            playerScores[i].score.color = col;
        }
        repaint();
    }

    public void setPlayerCount(int count) {
        playerData.playerCount = count;
        for(int p = 0; p < 4; ++p) {
            playerScores[p].gameObject.SetActive(p<playerData.playerCount);
        }
    }

    public void repaint()
    {
        setPlayerCount(playerData.playerCount);
        playerColor = playerData.playerInfo[playerData.activePlayer].playerColor;
        playerColor.a = 1;
        playerAltColor = playerColor;
        playerAltColor.r *= 0.5f;
        playerAltColor.g *= 0.5f;
        playerAltColor.b *= 0.5f;
        barL.sizeDelta = new Vector2(barWidth, 0);
        barR.sizeDelta = new Vector2(barWidth, 0);
        barT.sizeDelta = new Vector2(0, barWidth);
        barB.sizeDelta = new Vector2(0, barWidth);
        barL.GetComponent<Image>().color = playerColor;
        barR.GetComponent<Image>().color = playerColor;
        barT.GetComponent<Image>().color = playerColor;
        barB.GetComponent<Image>().color = playerColor;
        activePlayerDisplay.text = "Active Player:    " + playerData.playerInfo[playerData.activePlayer].title;
        activePlayerDisplayFrame.color = playerAltColor;
        for(int i = 0; i < 7; ++i) {
            playerResources[i].score.text = ""+playerData.playerInfo[playerData.activePlayer].scores[i];
        }
        for(int p = 0; p < 4; ++p) {
            int sum = 0;
            for(int i = 0; i < 9; ++i) {
                sum += playerData.playerInfo[p].scores[i];
            }
            playerScores[p].score.text = ""+sum;
        }
    }
    
}
