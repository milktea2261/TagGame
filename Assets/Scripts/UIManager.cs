using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Text gameText = null;
    [SerializeField] Text avatarStateText = null;
    [SerializeField] Text runToogleText = null;

    [SerializeField] Text playerCountText = null;
    [SerializeField] Text taggerRateText = null;
    [SerializeField] Text gameTimeText = null;
    [SerializeField] Scrollbar staminaBar = null;

    [SerializeField] GameObject pausePanel = null;

    [SerializeField] GameObject gameOverPanel = null;
    [SerializeField] Text gameOverText = null;

    private void Update() {
        gameText.text = "UnFreeze: " + GameManager.Instance.UnFrezzePlayer.ToString("00") + "\t";
        gameText.text += "Left Time: " + GameManager.Instance.timer.ToString("000");
    }

    public void UpdatePlayerCount(int count) {
        playerCountText.text = "玩家人數：" + count.ToString("00");
    }
    public void UpdateTaggerRate(float rate, int taggerCount) {
        taggerRateText.text = "獵人比例：" + rate.ToString("0.00") + " 數量：" + taggerCount;
    }
    public void UpdateGameTime(float gameTime) {
        gameTimeText.text = "遊戲時長：" + gameTime.ToString("000") + " 秒";
    }

    public void UpdatePlayerState(string avatar, bool isFreeze) {
        if(avatar == "Tagger") {
            avatarStateText.text = "Tagger";
        }
        else {
            avatarStateText.text = "Runner";
            if(isFreeze) {
                avatarStateText.text += "[Freeze]";
            }
        }
    }

    public void UpdatePlayerStamina(float percent) {
        staminaBar.size = percent;
    }

    public void RunBtn(bool isRun) {
        runToogleText.text = isRun ? "跑" : "走";
    }

    public void TogglePauseMenu() {
        pausePanel.SetActive(!pausePanel.activeInHierarchy);
    }

    public void GameOver(bool isRunnerWin) {
        gameOverPanel.SetActive(true);
        
        if(isRunnerWin) {
            gameOverText.text = "Runner Win";
        }
        else {
            gameOverText.text = "Runner Lose";
        }
    }
}