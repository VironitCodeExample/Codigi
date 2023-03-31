using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.Events;
using I2.Loc;

public class AttackSceneBotPanel : MonoBehaviour
{
    public Button collect;
    public Button brag;
    public Button exit;
    public TextMeshProUGUI text;
    public LocalizationParamsManager paramManager;
    public Localize localization;
    public float hideDelay;

    Animator animator;
    public bool showing = false;
    Action _proceedAnimationCompletion;
    Action _proceedAction;

    private bool _defended;
    private bool _isAttack = false;
    private bool _isFriend = false;

    private string _name;

    void Start()
    {
        animator = GetComponent<Animator>();
        exit.onClick.AddListener(Collect);
        collect.onClick.AddListener(Collect);
        brag.onClick.AddListener(Brag);
    }

    public void ShowPanelRaid(bool perfectRaid, ulong cryptoStolen, ulong cryptoStolenTotal, string targetName, Action proceedAction, Action proceedAnimationCompletion, float presentationDelay)
    {
        _proceedAnimationCompletion = proceedAnimationCompletion;
        _proceedAction = proceedAction;
        _defended = perfectRaid;
        _isAttack = false;
        _name = targetName;

        if (perfectRaid)
        {
            brag.gameObject.SetActive(true);
            exit.gameObject.SetActive(false);
            collect.gameObject.SetActive(true);
        }
        else
        {
            brag.gameObject.SetActive(false);
            exit.gameObject.SetActive(true);
            collect.gameObject.SetActive(false);
        }

        showing = true;

        SetTextRaid(cryptoStolen, _name);

        if (presentationDelay == 0)
        {
            SFXManager.Instance.Play("Hack UI Pop Up");
            AnimatePanelIn();
        }
        else
        {
            Invoke("PlayHackUIPopUpSFX", presentationDelay);
            Invoke("AnimatePanelIn", presentationDelay);
        }
    }

    private void PlayHackUIPopUpSFX()
    {
        SFXManager.Instance.Play("Hack UI Pop Up");
    }

    private void AnimatePanelIn()
    {
        animator.Play("AttackSceneBotPanelGoIn");
        animator.speed = 1f;
    }

    public void ShowPanelAttack(bool defended, string name, Action proceedAction, Action proceedAnimationCompletion)
    {
        _proceedAnimationCompletion = proceedAnimationCompletion;
        _proceedAction = proceedAction;
        _defended = defended;
        _isAttack = true;
        _name = name;

        Debug.Log("defended:" + defended);

        if (defended)
        {
            exit.gameObject.SetActive(true);
            brag.gameObject.SetActive(false);
            collect.gameObject.SetActive(false);
        }
        else
        {
            exit.gameObject.SetActive(false);
            brag.gameObject.SetActive(true);
            collect.gameObject.SetActive(true);
        }
        showing = true;
        var crypto = DataManager.DataApi.UserData.actionResults.attack.crypto;
        crypto += DataManager.DataApi.UserData.actionResults.attack.sidekick.wasUsed ? DataManager.DataApi.UserData.actionResults.attack.sidekick.crypto : 0;

        SetTextAttack(crypto, name, defended);
        SFXManager.Instance.Play("Attack UI Pop Up");
        AnimatePanelIn();
    }

    void Brag()
    {
        FacebookController.BragOnFacebookAboutWin(0, _isAttack, _isFriend);
    }

    public void Collect()
    {
        collect.enabled = false;
        if (_proceedAction != null)
        {
            _proceedAction.Invoke();
        }

        Debug.Log("betMultiplyer:" + DataManager.betMultiplyer);
        if (DataManager.betMultiplyer > 1)
        {
            SFXManager.Instance.Play("Hack and Attack Multiplier");
            animator.Play("AttackSceneBotPanelMultiplierCollectAnimation");
        }
        else
        {
            HidePanel();
        }
    }

    public void HidePanel()
    {
        animator.Play("AttackSceneBotPanelGoOut");
        animator.speed = 1f;
    }

    public void CollectAnimationFinished()
    {
        Invoke("HidePanel", hideDelay);
        ulong amount = _isAttack ? DataManager.DataApi.UserData.actionResults.attack.totalCrypto : DataManager.DataApi.UserData.actionResults.raid.totalCrypto;
        if (_isAttack)
        {
            SetTextAttack(amount, _name, _defended);
        }
        else
        {
            SetTextRaid(amount, _name);
        }

    }

    public void PanelGoOutAnimationFinished()
    {
        showing = false;
        if (_proceedAnimationCompletion != null)
        {
            _proceedAnimationCompletion.Invoke();
        }
    }

    void SetTextAttack(ulong cryptoStolen, string targetName, bool defended)
    {
        if (defended)
        {
            localization.SetTerm("Attack.Bottom_Panel.Defended.title");
        }
        else
        {
            targetName = Utils.LocalizeUserName(targetName);
            localization.SetTerm("Attack.Bottom_Panel.Success0.title");
        }
        targetName = String.Format("<color=\"white\">{0}<color=#75BBE7>", targetName);
        string cryptoWithColor = String.Format("<color=#ED9F21>{0}", Utils.LocalizeNumberAmount(cryptoStolen));
        paramManager.SetParameterValue("USER_NAME", targetName);
        paramManager.SetParameterValue("AMOUNT", cryptoWithColor);
    }

    void SetTextRaid(ulong cryptoStolen, string targetName)
    {
        targetName = String.Format("<color=\"white\">{0}</color>", targetName);
        paramManager.SetParameterValue("USER_NAME", targetName);
        paramManager.SetParameterValue("CRYPTO_COUNT", Utils.LocalizeNumberAmount(cryptoStolen));

        localization.SetTerm("Raid.Botttom_Panel.You_Stole.title");
    }

}
