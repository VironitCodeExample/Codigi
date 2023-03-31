using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class AttackSceneSpineAnimationController : MonoBehaviour
{
    public SkeletonGraphic coinsAnim;
    public SkeletonGraphic missleAnim;
    public SkeletonGraphic teslaAnim;
    public SkeletonGraphic davinciAnim;

    public Animator animator;

    public bool simulateDefendersGenius = true;
    public bool simulateMyGenius = true;
    public bool simulateDefendersShield = true;

    public float coinAnimStartDelayRegular;
    public float assetSwapDelay;
    public float teslaAnimDelay;
    public float teslaScale = 1;
    public float teslaScaleDelay = 2;
    public float teslaScaleNormal = 1;

    public float blockedMissileSoundDelay = 0f;

    System.Action _completion;
    System.Action _assetSwapTriger;

    public void PlayTest()
    {
        Play(simulateDefendersGenius, simulateDefendersShield, simulateMyGenius);
    }

    public void Play(ActionResults.AttackActionResultDetails data, System.Action assetSwapTriger, System.Action completion)
    {
        _completion = completion;
        _assetSwapTriger = assetSwapTriger;
        Play(data.wasDefendedWith.Equals("SIDEKICK"), data.wasDefendedWith.Equals("SHIELD"), data.sidekick.wasUsed);
    }

    void Play(bool blockedWithGenius, bool blockedByShield, bool usedMyGenius)
    {
        if (usedMyGenius)
        {
            SFXManager.Instance.Play("Arty Paintbrush Attack");
            animator.Play("DaVinciAttackAnimation", -1, 0f);
            davinciAnim.enabled = true;
            davinciAnim.AnimationState.SetAnimation(0, "Action", false);
            davinciAnim.AnimationState.Complete += delegate
            {
                PlayMainPartOfAnimation(blockedWithGenius, blockedByShield, usedMyGenius);
            };
        }
        else
        {
            PlayMainPartOfAnimation(blockedWithGenius, blockedByShield, usedMyGenius);
        }
    }

    public void TestAnim() => PlayMainPartOfAnimation(true, false, false);


    void PlayMainPartOfAnimation(bool blockedWithGenius, bool blockedByShield, bool usedMyGenius)
    {
        //davinciAnim.enabled = false;
        string animName;
        if (blockedWithGenius)
        {
            animName = "AttackNoMissile";
        }
        else if (blockedByShield)
        {
            animName = "Block";
            StartCoroutine(PlayBlockedByShieldSound(blockedMissileSoundDelay));
        }
        else
        {
            animName = "Attack";
        }


        missleAnim.enabled = true;
        missleAnim.AnimationState.SetAnimation(0, animName, false);
        missleAnim.AnimationState.Complete += delegate
        {
            if (blockedByShield)
            {
                TrigerCompletion();
            }
        };
        if (blockedWithGenius)
        {
            SFXManager.Instance.Play("AttackNoMissile");
            StartCoroutine(PlayDefenceWithGeniusAnimation());
        }
        else if (!blockedByShield && !blockedWithGenius)
        {
            SFXManager.Instance.Play("Missile Attack");
            Invoke("PlayCoinAnimation", coinAnimStartDelayRegular);
            Invoke("TrigerAssetSwap", assetSwapDelay);
        }
    }

    IEnumerator PlayBlockedByShieldSound(float delay)
    {
        yield return new WaitForSeconds(delay);
        SFXManager.Instance.Play("missile attack shield block relevel");
    }

    public void TrigerAssetSwap()
    {
        _assetSwapTriger();
    }

    public void TrigerCompletion()
    {
        _completion();
    }

    private IEnumerator PlayDefenceWithGeniusAnimation()
    {
        yield return new WaitForSeconds(teslaAnimDelay);
        SFXManager.Instance.Play("Sparky Coil");
        teslaAnim.enabled = true;
        teslaAnim.timeScale = teslaScale;
        teslaAnim.AnimationState.SetAnimation(0, "SpecialDefense", false);
        teslaAnim.AnimationState.Complete += delegate
        {
            teslaAnim.timeScale = 1;
            //coinsAnim.enabled = false;
            TrigerCompletion();
        };
        yield return new WaitForSeconds(teslaScaleDelay);
        teslaAnim.timeScale = teslaScaleNormal;
    }

    void PlayCoinAnimation()
    {
        coinsAnim.enabled = true;
        coinsAnim.AnimationState.SetAnimation(0, "Large", false);
        coinsAnim.AnimationState.Complete += delegate
        {
            //coinsAnim.enabled = false;
            TrigerCompletion();
        };
    }

}
