using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    public UIManager uiManager;

    public int FreezeCount {
        get {
            int counter = 0;
            foreach(AvatarCtrl player in players) {
                if(player.IsFreeze)
                    counter++;
            }
            return counter;
        }
    }
    public int UnFrezzePlayer { get { return GameManager.Instance.playerCount - GameManager.Instance.taggerCount - FreezeCount; } }

    public float timer = 0;
    private bool isGameOver = false;
    private bool isGameStart = false;

    AvatarCtrl playerControl = null;
    public AvatarCtrl aiPrefab = null;
    public List<AvatarCtrl> players = new List<AvatarCtrl>();
    public Animator[] characters = new Animator[0];

    public Transform[] runnerSpawnPoints;
    public Transform taggerSpawnPoint;

    private void Awake() {
        playerControl = FindObjectOfType<PlayerCtrl>();
    }

    //生成玩家，出生位置
    private void Start() {
        //生成玩家
        players = new List<AvatarCtrl>();
        for(int i = 0; i < GameManager.Instance.playerCount; i++) {
            AvatarCtrl avatar;
            if(i == 0) {
                avatar = playerControl;//玩家
            }
            else {
                Animator model = characters[0];
                string avatarName = string.Format("AI[{0}]", (i - 1).ToString("00"));
                avatar = CreateAvatar(model, avatarName);
            }
            avatar.gameObject.SetActive(false);
            players.Add(avatar);
        }

        //打亂排序
        for(int i = 0; i < players.Count; i++) {
            int randIndex = Random.Range(0, players.Count);
            var temp = players[i];
            players[i] = players[randIndex];
            players[randIndex] = temp;
        }

        //設定身分，初始位置
        for(int i = 0; i < players.Count; i++) {
            if(i < GameManager.Instance.taggerCount) {
                players[i].SetAvatar(AvatarTag.Tagger);
                players[i].name += " Tagger";
                players[i].transform.position = taggerSpawnPoint.position;
                players[i].transform.rotation = taggerSpawnPoint.rotation;
            }
            else {
                players[i].SetAvatar(AvatarTag.Runner);
                int randIndex = Random.Range(0, runnerSpawnPoints.Length);
                players[i].transform.position = runnerSpawnPoints[randIndex].position;
                players[i].transform.rotation = runnerSpawnPoints[randIndex].rotation;
            }
        }

        GameStart();
    }

    private void Update() {
        if(isGameOver || !isGameStart) {
            return;
        }

        if(Input.GetKeyDown(KeyCode.Escape)) {
            if(Time.timeScale == 0) {
                Resume();
            }
            else {
                Pause();
            }
        }

        uiManager.UpdatePlayerState(playerControl.tag, playerControl.IsFreeze);
        uiManager.UpdatePlayerStamina(playerControl.Stamina / playerControl.maxStamina);

        timer -= Time.deltaTime;
        if(timer <= 0 || UnFrezzePlayer == 0) {
            GameOver();
        }
    }

    //生成玩家等初始設定
    AvatarCtrl CreateAvatar(Animator avatarModel, string avatarName) {
        AvatarCtrl avatar = Instantiate<AvatarCtrl>(aiPrefab);
        avatar.name = avatarName;

        Animator character = Instantiate<Animator>(avatarModel, avatar.transform);
        avatar.animator = character;

        return avatar;
    }

    //開始遊戲
    public void GameStart() {
        timer = GameManager.Instance.gameTime;
        foreach(AvatarCtrl avatar in players) {
            avatar.UnFreeze();
            avatar.gameObject.SetActive(true);
        }

        isGameStart = true;
        isGameOver = false;
        Time.timeScale = 1;
    }

    [ContextMenu("GameOver")]
    public void GameOver() {

        bool isRunnerWin = timer <= 0;
        if(isRunnerWin) {
            Debug.Log("Game Over. Ruuner win.");
        }
        if(isRunnerWin) {
            Debug.Log("Game Over. Tagger win.");
        }

        uiManager.GameOver(isRunnerWin);
        isGameStart = false;
        isGameOver = true;
        Time.timeScale = 0;
    }

    //返回主選單
    public void MainMenu() {
        Debug.LogError("TODO: Go To Main Menu");
    }

    public void Pause() {
        Time.timeScale = 0;
        uiManager.TogglePauseMenu();
    }
    public void Resume() {
        Time.timeScale = 1;
        uiManager.TogglePauseMenu();
    }

}
