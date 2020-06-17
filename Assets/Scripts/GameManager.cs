using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int playerCount = 4;//遊戲人數
    public int taggerCount = 1;//獵人人數
    public float gameTime = 180f;//單局遊戲時間

    public int playerAvatarIndex = 0;//保存玩家選擇的角色

    public AvatarData[] datas = new AvatarData[0];

    private void Awake() {
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(Instance != this) {
            Destroy(gameObject);
        }
    }

    public void LoadLevel(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

}
