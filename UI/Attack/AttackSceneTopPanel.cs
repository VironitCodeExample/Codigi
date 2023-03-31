using System;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using I2.Loc;

public class AttackSceneTopPanel : MonoBehaviour
{
    Animator animator;
    public ProfileImage targetImage;
    public TextMeshProUGUI targetName;

    public LocalizationParamsManager targetLocManager;

    public TextDropShadow revengeButtonText;
    public TextDropShadow friendsButtonText;

    public Button revengeButton;
    public Button friendsButton;
    public bool showing = false;


    public void ShowRaidPanel(UserDetails target, float delay)
    {
        string targetDispalyName = Utils.LocalizeUserName(target.displayName);
        string winnableCrypto = Utils.LocalizeNumberAmount(target.winnableCrypto);
        string line1 = "<#6ECDE4>" + Utils.Translate("General.Mogul") + "</color>";
        string line2 = $"{targetDispalyName} {Utils.Translate("General.treasure")}: <#EE9F1F>{winnableCrypto}</color>";
        string text = $"{line1}\n{line2}";

        targetName.SetText(text);
        targetImage.SetUri(target.profilePictureUri);
        friendsButton.gameObject.SetActive(false);
        revengeButton.gameObject.SetActive(false);

        if (delay == 0)
        {
            AnimateIn();
        }
        else
        {
            Invoke("AnimateIn", delay);
        }
    }

    public void ShowPanel(UserDetails target, UnityAction friendsAction, UnityAction revengeAction, float delay)
    {
        string targetDispalyName = Utils.LocalizeUserName(target.displayName);
        targetName.SetText(Utils.Translate("General.business").Replace("{[USER_NAME]}", targetDispalyName));
        targetImage.SetUri(target.profilePictureUri);
        friendsButton.onClick.AddListener(friendsAction);
        revengeButton.onClick.AddListener(revengeAction);

        if (delay == 0)
        {
            AnimateIn();
        }
        else
        {
            Invoke("AnimateIn", delay);
        }
    }

    void AnimateIn()
    {
        if (showing)
        {
            return;
        }
        animator = GetComponent<Animator>();
        PlayAnimateIn();
        showing = true;
    }

    public void HidePanel()
    {
        if (!showing)
        {
            return;
        }
        AnimateOut();
    }

    public void HidePanel(float delay)
    {
        if (!showing)
        {
            return;
        }
        Invoke("AnimateOut", delay);
    }

    private void AnimateOut()
    {
        animator.Play("AttackSceneTopPanelGoOut");
        animator.speed = 1f;
    }

    private void PlayAnimateIn()
    {
        animator.Play("AttackSceneTopPanelGoIn");
        animator.speed = 1f;
    }

    public void PanelGoOutAnimationFinished()
    {
        showing = false;
    }



}
