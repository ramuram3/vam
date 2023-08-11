using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("# Game Control")]
    public float playTime;
    public float[] maxPlayTimes;
    public bool isLive;
    public bool isClear;
    public int stage;
    [Header("# Player Info")]
    public int playerId;
    public int health;
    public int maxHealth=100;
    public int playerLevel;
    public int kill;
    public float exp;
    public int[] maxExp;
    [Header("# Game Object")]
    public PoolManager poolManager;
    public Player player;
    public LevelUp uiLevelUp;
    public GameObject gameOver;
    public GameObject stageClear;
    public GameObject[] unLockCharacters;
    public DataManager dataManager;
    public Transform uiJoy;


    void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
        dataManager.Load();
        dataManager.CheckHasKey();
        UnLockCharacter();
    }
    void Start()
    {
        Time.timeScale = 0;
        health = maxHealth;


    }
    void Update()
    {
        playTime += Time.deltaTime;

        if(playTime >= maxPlayTimes[stage])
        {
            StageClear();
        }
    }

    public void GetExp(float _exp)
    {
        exp += _exp;
        if(exp >= maxExp[Mathf.Min(playerLevel, maxExp.Length-1)])
        {
            while(true)
            {
                exp = exp - maxExp[Mathf.Min(playerLevel, maxExp.Length - 1)];
                playerLevel++;
                uiLevelUp.Show();
                if (exp < maxExp[Mathf.Min(playerLevel, maxExp.Length - 1)])
                    break;
            }
        }
    }

    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0;
        uiJoy.localScale = Vector3.zero;
    }

    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;
        uiJoy.localScale = Vector3.one;
    }

    public void GameOver()
    {
        AudioManager.instance.PlayBgm(false);
        gameOver.SetActive(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose);
    }

    public void LoadScene()
    {
        //Main Menu Button
        SceneManager.LoadScene(0);
    }

    public void GameQuit()
    {
        Application.Quit();
    }

    public void StageClear()
    {
        AudioManager.instance.PlayBgm(false);
        if (isClear==false)
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Win);
            stageClear.SetActive(true);
            if (dataManager.gameData.clearStage < stage)
            {
                dataManager.gameData.clearStage = stage;
            }
            dataManager.CheckHasKey();
            dataManager.Save();
            Stop();
        }
        isClear = true;
    }

    public void NextStage()
    {
        AudioManager.instance.PlayBgm(true);
        //Next Stage Button
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
        playTime = 0;
        playerLevel = 0;
        exp = 0;
        kill = 0;
        health = maxHealth;
        poolManager.BroadcastMessage("Dead", SendMessageOptions.DontRequireReceiver);
        //uiLevelUp.BroadcastMessage("ResetLevel", SendMessageOptions.DontRequireReceiver);
        //uiLevelUp.Select(0);
        stageClear.SetActive(false);
        stage++;
        isClear = false;
        Resume();
    }

    public void UnLockCharacter()
    {
        for(int i=0;i<unLockCharacters.Length;i++)
        {
            if (dataManager.gameData.hasKey[i])
            {
                unLockCharacters[i].gameObject.SetActive(false);
            }
        }
    }
    public void SetPlayer(int i)
    {
        playerId = i;
        player.animator.runtimeAnimatorController = player.animatorControllers[i];
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
        AudioManager.instance.PlayBgm(true);
    }

}
