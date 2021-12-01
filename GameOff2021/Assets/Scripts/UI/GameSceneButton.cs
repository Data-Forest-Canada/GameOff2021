using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneButton : MonoBehaviour
{
    public void OnClick()
    {
        GameManager.Instance.MoveToGameScene();
    }
}
