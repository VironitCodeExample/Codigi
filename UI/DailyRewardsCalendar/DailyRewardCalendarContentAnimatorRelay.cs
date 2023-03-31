using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyRewardCalendarContentAnimatorRelay : MonoBehaviour
{
	public DailyRewardCalendar model;
	
	public void TrigerDay8SpineAnimationStart()
	{
		model.PlaySpineTrigered(0);
	}
	
	public void TrigerDay15SpineAnimationStart()
	{
		model.PlaySpineTrigered(1);
	}
	
	public void TrigerDay22SpineAnimationStart()
	{
		model.PlaySpineTrigered(2);
	}
	
	public void TrigerDay30SpineAnimationStart()
	{
		model.PlaySpineTrigered(3);
	}
	//----------------
	public void TrigerDay8AssetSwap()
	{
		model.SwapAssetTrigered(0);
	}
	
	public void TrigerDay15AssetSwap()
	{
		model.SwapAssetTrigered(1);
	}
	
	public void TrigerDay22AssetSwap()
	{
		model.SwapAssetTrigered(2);
	}
	
	public void TrigerDay30AssetSwap()
	{
		model.SwapAssetTrigered(3);
	}
	
	//----------------
	public void TrigerDay8AnimationComplete()
	{
		model.AnimationCompletionTrigered(0);
	}
	
	public void TrigerDay15AnimationComplete()
	{
		model.AnimationCompletionTrigered(1);
	}
	
	public void TrigerDay22AnimationComplete()
	{
		model.AnimationCompletionTrigered(2);
	}
	
	public void TrigerDay30AnimationComplete()
	{
		model.AnimationCompletionTrigered(3);
	}
	
	//----------------
	public void TrigerDay8PopUpPresent()
	{
		model.PopUpPresentationTrigered(0);
	}
	
	public void TrigerDay15PopUpPresent()
	{
		model.PopUpPresentationTrigered(1);
	}
	
	public void TrigerDay22PopUpPresent()
	{
		model.PopUpPresentationTrigered(2);
	}
	
	public void TrigerDay30PopUpPresent()
	{
		model.PopUpPresentationTrigered(3);
	}
}
