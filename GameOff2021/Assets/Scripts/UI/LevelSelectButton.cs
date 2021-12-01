using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class LevelSelectButton : MonoBehaviour
{
    [SerializeField] int levelIndex;
    Button button;
    TMP_Text textField;

    static GameManager manager;

    private void Awake()
    {
        if (manager == null) manager = GameManager.Instance;
        button = GetComponent<Button>();
        textField = GetComponentInChildren<TMP_Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
        button.interactable = manager.IsLevelUnlocked(levelIndex);

        if (textField != null)
        {
            textField.text = (levelIndex + 1).ToString();
        }
    }

    public void OnClick()
    {
        GameManager.Instance.CurrentLevelIndex = levelIndex;
        GameManager.Instance.MoveToGameScene();
    }
}
