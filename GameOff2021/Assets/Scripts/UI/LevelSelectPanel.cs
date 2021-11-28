using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectPanel : MonoBehaviour
{
    [SerializeField] Animator anim;
    bool closing = false;

    public void ClosePanel()
    {
        anim.SetFloat("AnimationDirection", -anim.GetFloat("AnimationDirection"));
        anim.Play("LevelSelectAnimation");
        closing = true;
    }

    public void Disable()
    {
        if (gameObject.activeSelf && closing) gameObject.SetActive(false);
        closing = false;
    }
}
