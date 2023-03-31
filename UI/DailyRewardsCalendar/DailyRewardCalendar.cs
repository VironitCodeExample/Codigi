using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using System.Globalization;

public class DailyRewardCalendar : MonoBehaviour
{
    public List<DailyCalendarRewardElement> rewardElements;
    public List<Button> dayButtons;
    public RewardCalendar7thDayElementContainer _7thDayRewards;
    public TextMeshProUGUI bottomText;
    public DailyRewardCalendarBar bar;
    public SkeletonGraphic animation;
    public Animator contentAnimator;
    public GeneralRewardPopUp generalRewardsPopUp;
    public CountDownTimerText countDown;
    public I2.Loc.Localize timerLocalizer;
    public BasePopUpAnimator popupAnimator;
    public VaultRevealPanel vaultReveal;
    public BaloonPopUp baloonPopUp;
    public System.Action snakyAnimationFinish;
    BasePopUp basePopUp;
    int[] dayCounts = new int[] { 8, 15, 22, 30 };
    bool claimed = true;
    int claimableIndex = -1;

    bool isUnlocked
    {
        get
        {
            return DataManager.DataApi.UserData.user.locationLevel >= DataManager.rewards.requiredLevel;
        }
    }


    void Start()
    {
        SFXManager.Instance.Play("JR reward open");
        if (!DataManager.hasUserData)
        {
            return;
        }
        AnalyticsHandler.RewardsCalendarEnterTrigger();
        LoadData();
        basePopUp = GetComponent<BasePopUp>();
    }


    public void ElementClicked()
    {
        if (claimableIndex > -1)
        {
            MasterInputBlockController.Instance.DisableInput();
            int lastCountTotal = DataManager.DataApi.UserData.actionResults.calendarRewardsDaily.countTotal;

            SFXManager.Instance.Play("Rewards click on");
            if (claimableIndex < 6)
            {
                rewardElements[claimableIndex].particles.Play();
                rewardElements[claimableIndex].MarkAsClaimed();
            }
            else
            {
                _7thDayRewards.particles.Play();
                _7thDayRewards.MarkAsClaimed();
            }

            NetworkHandler.Instance.PostCalendarReward((success) =>
            {
                if (success)
                {
                    SetupCountDownTimer();
                    Vector3 pos;
                    if (claimableIndex < 6)
                    {
                        pos = rewardElements[claimableIndex].tick.gameObject.transform.position;
                    }
                    else
                    {
                        pos = _7thDayRewards.tick.gameObject.transform.position;
                    }

                    SFXManager.Instance.Play("Rewards check mark");
                    bar.AnimateTickFrom(pos, () =>
                    {
                        int currentCountTotal = DataManager.DataApi.UserData.actionResults.calendarRewardsDaily.countTotal;
                        bar.SetupBar(currentCountTotal);
                        int i = 0;
                        bool isWeekly = false;
                        int lastDay = dayCounts[dayCounts.Length - 1];
                        foreach (int day in dayCounts)
                        {
                            if (day == currentCountTotal || (day == lastDay && day == lastCountTotal + 1))
                            {
                                PlayWeeklyRewardAnimation(i);
                                isWeekly = true;
                                break;
                            }
                            i++;
                        }
                        if (!isWeekly)
                        {
                            List<ConsumableItem> list = GetClaimedConsumables(false);
                            List<Card> cards = DataManager.DataApi.UserData.actionResults.calendarRewardsDaily.cards;
                            PlayLastRewardAnimationPart(list, cards, () =>
                            {
                                if (snakyAnimationFinish != null)
                                {
                                    snakyAnimationFinish();
                                }
                            });
                        }
                    });
                }
            });
        }
        else
        {
            basePopUp.CloseButtonAction();
        }

    }

    List<ConsumableItem> GetClaimedConsumables(bool appendWeekly)
    {
        var actionResults = DataManager.DataApi.UserData.actionResults;
        List<ConsumableItem> list = Utils.ConvertBaseActionResultRewardToConsumables(actionResults.calendarRewardsDaily);
        if (appendWeekly)
        {
            var list2 = Utils.ConvertBaseActionResultRewardToConsumables(actionResults.calendarRewardsDailyTotal);
            list.AddRange(list2);
        }

        return list;
    }

    void PlayLastRewardAnimationPart(List<ConsumableItem> list, List<Card> cards, System.Action completion)
    {
        if (cards != null && cards.Count > 0)
        {
            MasterInputBlockController.Instance.EnableInput();
            PopUpPresenter.Instance.darkOverlay.AnimateOut();
            vaultReveal.gameObject.SetActive(true);
            vaultReveal.Present(cards, () =>
            {
                ClosePopUpAndDisplaySnakyAnimation(list, () =>
                 {
                     completion();
                 });
            });
        }
        else
        {
            ClosePopUpAndDisplaySnakyAnimation(list, () =>
             {
                 completion();
             });
        }
    }

    void ClosePopUpAndDisplaySnakyAnimation(List<ConsumableItem> list, System.Action completion)
    {
        popupAnimator.AnimateOut(() =>
        {
            MasterInputBlockController.Instance.EnableInput();
            PopUpPresenter.Instance.darkOverlay.AnimateOut();
            MenuOverlayMovableItemSpawnersController.Instance.Play(list, () =>
             {
                 completion();
             });
        });
    }


    public void InvisibleButtonClicked()
    {
        ElementClicked();
    }

    public void PlayWeeklyRewardAnimation(int index)
    {
        switch (index)
        {
            case 0:
                contentAnimator.Play("DailyRewardsDayVideoSwapAnimation");
                break;
            case 1:
                contentAnimator.Play("DailyRewardsDay1VideoSwapAnimation");
                break;
            case 2:
                contentAnimator.Play("DailyRewardsDay2VideoSwapAnimation");
                break;
            case 3:
                contentAnimator.Play("DailyRewardsDay3VideoSwapAnimation");
                break;
            default:
                break;
        }

    }

    public void Present1Clicked()
    {
        PresentRewardsBubble(0);
    }

    public void Present2Clicked()
    {
        PresentRewardsBubble(1);
    }

    public void Present3Clicked()
    {
        PresentRewardsBubble(2);
    }

    public void Present4Clicked()
    {
        PresentRewardsBubble(3);
    }

    #region Animation Trigers

    public void PlaySpineTrigered(int index)
    {
        SFXManager.Instance.Play("Gift Box");
        PlaySpineAnimation(index);
    }

    public void SwapAssetTrigered(int index)
    {
        bar.SetDayCount(dayCounts[index]);
    }

    public void AnimationCompletionTrigered(int index)
    {

    }

    public void PopUpPresentationTrigered(int index)
    {
        SFXManager.Instance.Play("Rewards collect screen");
        MasterInputBlockController.Instance.EnableInput();
        generalRewardsPopUp.gameObject.SetActive(true);
        generalRewardsPopUp.gameObject.GetComponent<BasePopUp>().animator.AnimateIn();

        generalRewardsPopUp.PresentPopUpWithoutMovableItems(GetClaimedConsumables(true), () =>
        {
            PopUpPresenter.Instance.darkOverlay.AnimateOut();
            var actionResults = DataManager.DataApi.UserData.actionResults;
            List<Card> cards = actionResults.calendarRewardsDaily.cards;
            PlayLastRewardAnimationPart(GetClaimedConsumables(true), cards, () =>
            {
                if (snakyAnimationFinish != null)
                {
                    snakyAnimationFinish();
                }
            });
        });
    }

    #endregion
    void PlaySpineAnimation(int index)
    {

        string skin = null;

        switch (index)
        {
            case 0:
                skin = "GiftBox01";
                break;
            case 1:
                skin = "Gift Box02";
                break;
            case 2:
                skin = "GiftBox03";
                break;
            case 3:
                skin = "GiftBox04";
                break;
            default:
                break;
        }

        animation.Skeleton.SetSkin(skin);
        animation.AnimationState.SetAnimation(0, "animation", false);
        animation.AnimationState.Complete += delegate
        {

        };
    }

    void PresentRewardsBubble(int index)
    {
        List<ConsumableItem> rewards = Utils.ConvertCalendarWeeklyRewardToConsumables(DataManager.rewards.dailyTotal.rewards[index]);

        baloonPopUp.Setup(rewards, dayButtons[index].transform.position);
        if (!baloonPopUp.gameObject.activeSelf)
        {
            baloonPopUp.gameObject.SetActive(true);
            //StartCoroutine(PresentBaloon());
            baloonPopUp.GetComponent<BasePopUpAnimator>().AnimateIn();
        }

    }

    IEnumerator PresentBaloon()
    {
        yield return new WaitForSeconds(0.6f);
        baloonPopUp.GetComponent<BasePopUpAnimator>().AnimateIn();
    }

    void SetupCountDownTimer()
    {
        if (DataManager.DataApi.UserData.actionResults.calendarRewardsDaily == null || DataManager.DataApi.UserData.actionResults.calendarRewardsDaily.day == null)
        {
            timerLocalizer.SetTerm("Daily.Rewards.Expires_in.title");
        }
        else
        {
            timerLocalizer.SetTerm("Daily.Rewards.Available_in.title");
        }

        DateTime timeNext;
        try
        {
	        timeNext = DateTime.ParseExact(DateTime.UtcNow.AddDays(1).ToString("yyyyMMdd"), "yyyyMMdd", CultureInfo.InvariantCulture);
            countDown.SetupWith(timeNext);
        }
        catch (FormatException)
        {
            Console.WriteLine(" is not in an acceptable format.");
        }

    }

    void LoadData()
    {
        if (DataManager.rewards != null && isUnlocked)
        {

            for (int i = 0; i < 6; i++)
            {
                var reward = DataManager.rewards.daily.rewards[i];
                rewardElements[i].SetupAs(i, reward);
                if (reward.rewardStatus == CalendarRewardStatus.available)
                {
                    claimed = false;
                    claimableIndex = i;
                }
            }

            CalendarReward _7thDayReward = DataManager.rewards.daily.rewards[6];
            _7thDayRewards.SetupWith(7, _7thDayReward);
            if (_7thDayReward.rewardStatus == CalendarRewardStatus.available)
            {
                claimed = false;
                claimableIndex = 6;
            }

            if (!claimed)
            {
                bottomText.text = Utils.Translate("PopUp.Rewards_Calendar.Tap_To_Collect.title");
            }
            else
            {
                bottomText.text = Utils.Translate("PopUp.Rewards_Calendar.Tap_To_Close.title");
            }
            SetupCountDownTimer();
            bar.SetDayCount(DataManager.rewards.dailyTotal.currentCountTotal);
        }
    }

}
