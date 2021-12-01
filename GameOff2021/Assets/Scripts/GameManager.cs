using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [SerializeField] float sceneChangeDelay = 0;
    public static GameManager Instance;
    public Level[] Levels;
    public Level NextUnlockedLevel => getNextUnlockedLevel();
    public Level CurrentLevel => Levels[CurrentLevelIndex];
    public int CurrentLevelIndex
    {
        get { return currentLevelIndex; }
        set
        {
            if (value >= 0 || value < Levels.Length) currentLevelIndex = value;
        }
    }

    public UnityEvent OnSceneChanged;

    int currentLevelIndex;
    Dictionary<Level, bool> levelUnlockedStatus;

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        levelUnlockedStatus = initialFillUnlockedLevels(Levels);
    }

    public void UnlockLevel(Level level)
    {
        if (!levelUnlockedStatus.ContainsKey(level)) return;
        levelUnlockedStatus[level] = true;
    }

    public void UnlockNextLevel()
    {
        CurrentLevelIndex++;
        UnlockLevel(Levels[CurrentLevelIndex]);
    }

    public bool IsLevelUnlocked(Level level)
    {
        if (!levelUnlockedStatus.ContainsKey(level)) return false;
        return levelUnlockedStatus[level];
    }

    public bool IsLevelUnlocked(int index)
    {
        // Bounds checking
        if (index < 0 || index >= Levels.Length) return false;
        return IsLevelUnlocked(Levels[index]);
    }

    public void MoveToGameScene()
    {
        AsyncOperation sceneLoading = SceneManager.LoadSceneAsync("GameScene");
        sceneLoading.allowSceneActivation = false;
        StartCoroutine(coWaitForLoading(sceneLoading));
    }

    public void MoveToCreditsScene()
    {
        AsyncOperation sceneLoading = SceneManager.LoadSceneAsync("Credits");
        sceneLoading.allowSceneActivation = false;
        StartCoroutine(coWaitForLoading(sceneLoading));
    }

    public void MoveToMainMenuScene()
    {
        AsyncOperation sceneLoading = SceneManager.LoadSceneAsync("MainMenu");
        sceneLoading.allowSceneActivation = false;
        StartCoroutine(coWaitForLoading(sceneLoading));
    }

    IEnumerator coWaitForLoading(AsyncOperation loadingOperation)
    {
        Timer timer = new Timer(this, sceneChangeDelay);
        timer.Start();

        while (!(loadingOperation.progress >= 0.9f) || !timer.IsCompleted)
        {
            yield return null;
        }

        loadingOperation.allowSceneActivation = true;
        OnSceneChanged?.Invoke();
    }

    // Fill the dictionary, only unlocking the first level
    Dictionary<Level, bool> initialFillUnlockedLevels(Level[] levels)
    {
        Dictionary<Level, bool> levelMappings = new Dictionary<Level, bool>();

        for (int i = 0; i < levels.Length; i++)
        {
            levelMappings.Add(levels[i], i == 0);
        }

        return levelMappings;
    }

    // Finds the last level that is unlocked
    Level getNextUnlockedLevel()
    {
        Level nextLevel = null;

        foreach (Level level in Levels)
        {
            if (!IsLevelUnlocked(level)) break;

            nextLevel = level;
        }

        return nextLevel;
    }
}
