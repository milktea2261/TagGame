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
    public AvatarData[] datas = new AvatarData[0];

    public Transform[] runnerSpawnPoints;
    public Transform taggerSpawnPoint;

    private void Awake() {
        playerControl = FindObjectOfType<PlayerCtrl>();
    }

    //生成玩家，出生位置
    private void Start() {
        //生成角色
        players = new List<AvatarCtrl>();
        for(int i = 0; i < GameManager.Instance.playerCount; i++) {
            AvatarCtrl ctrl;
            if(i == 0) {
                //生成玩家的角色
                ctrl = playerControl;
                //CreateAvatar(ctrl, player's index);
            }
            else {
                //生成AI的角色
                ctrl = Instantiate<AvatarCtrl>(aiPrefab);
                ctrl.name = string.Format("AI[{0}]", (i - 1).ToString("00"));

                int randomIndex = Random.Range(0, characters.Length);
                CreateAvatar(ctrl, randomIndex);
            }
            ctrl.gameObject.SetActive(false);
            players.Add(ctrl);
        }

        //打亂排序
        for(int i = 0; i < players.Count; i++) {
            int randomIndex = Random.Range(0, players.Count);
            var temp = players[i];
            players[i] = players[randomIndex];
            players[randomIndex] = temp;
        }

        //設定身分，初始位置
        for(int i = 0; i < players.Count; i++) {
            if(i < GameManager.Instance.taggerCount) {
                players[i].SetAvatar(AvatarTag.Tagger);
                players[i].name += " Tagger";
                players[i].transform.position = taggerSpawnPoint.position;
                players[i].transform.rotation = taggerSpawnPoint.rotation;
                //players[i].moveSpeed.AddModifier(new AttributeModifier(1.5f, AttributeModifierType.Flat, "System"));
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
        uiManager.UpdatePlayerStamina(playerControl.Stamina / playerControl.maxStamina.FinalValue);

        timer -= Time.deltaTime;
        if(timer <= 0 || UnFrezzePlayer == 0) {
            GameOver();
        }
    }

    //生成玩家等初始設定
    void CreateAvatar(AvatarCtrl ctrl, int index) {
        Animator character = Instantiate<Animator>(characters[index], ctrl.transform);
        ctrl.Init(character, datas[index]);
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
