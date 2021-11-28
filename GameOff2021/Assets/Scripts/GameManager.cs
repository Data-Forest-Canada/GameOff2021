using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    public Level[] Levels;
    public Level NextUnlockedLevel => getNextUnlockedLevel();
    public Level CurrentLevel => Levels[CurrentLevelIndex];
    public int CurrentLevelIndex
    {
        get { return currentLevelIndex; }
        set
        {
            if (value < 0 || value >= Levels.Length) currentLevelIndex = value;
        }
    }
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
        SceneManager.LoadScene("GameScene");
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
            if (!levelUnlockedStatus[level]) break;

            nextLevel = level;
        }

        return nextLevel;
    }
}
