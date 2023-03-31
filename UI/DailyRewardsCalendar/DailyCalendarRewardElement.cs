using TMPro;
using UnityEngine.UI;
using UnityEngine;


public enum DailyCalendarRewardType
{
    crypto,
    spin,
    chest,
    exp,
    food
}

public class DailyCalendarRewardElement : MonoBehaviour
{
    public Sprite greenBubble;
    public Sprite blueBubble;

    public GameObject artPrefab;
    public GameObject artContainer;

    public Image tick;
    public Image dayBubble;
    public Image hideOverlay;
    public Image glowImage;

    public I2.Loc.LocalizationParamsManager dayLabelLocParamsManager;
    public Animator animator;

    public ParticleSystem particles;

    void Start()
    {
        particles.Stop();
    }

    public void SetupAs(int day, CalendarReward reward)
    {
        bool isClaimed = reward.rewardStatus == CalendarRewardStatus.claimed ? true : false;
        tick.gameObject.SetActive(isClaimed);
        hideOverlay.gameObject.SetActive(isClaimed);
        glowImage.enabled = (reward.rewardStatus == CalendarRewardStatus.available) ? true : false;
        dayBubble.sprite = isClaimed ? blueBubble : greenBubble;
	    dayLabelLocParamsManager.SetParameterValue("DAY", (day + 1).ToString());

        int index = 0;
        foreach (ConsumableItem item in reward.consumables)
        {
            int lastIndex = reward.consumables.Count - 1;
	        bool isSingle = reward.consumables.Count == 1;
	        bool isRight = lastIndex == index;
	        CreateSingleReward(item, isSingle, isRight);
            index++;
        }


        if (reward.rewardStatus == CalendarRewardStatus.available)
        {
            animator.Play("CalendarSingleElementGlowAnimation");
        }
    }

	public void CreateSingleReward(ConsumableItem item, bool singleRewardInDay, bool rightAligment)
    {
        if (item.amount == 0)
        {
            return;
        }

        var go = Instantiate(artPrefab);
        GeneralRewardElement model = go.GetComponent<GeneralRewardElement>();
        model.Setup(item);

        go.transform.SetParent(artContainer.transform);
	    go.transform.localScale = Vector3.one;
	    if (!singleRewardInDay)
	    {
		    model.text.horizontalAlignment = rightAligment ? HorizontalAlignmentOptions.Right : HorizontalAlignmentOptions.Left;
	    }
    }

    public void MarkAsClaimed()
    {
        bool isClaimed = true;
        tick.gameObject.SetActive(isClaimed);
        hideOverlay.gameObject.SetActive(isClaimed);
        dayBubble.sprite = isClaimed ? blueBubble : greenBubble;
        animator.Play("CalendarSingleElementClickAnimation");
    }

}
