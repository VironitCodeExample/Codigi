using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardCalendarBar : MonoBehaviour
{
    public List<Image> images;
	public List<Sprite> openedPresents;
	public List<GameObject> buttons;
	public RectTransform bar;
	public GameObject nonMoovableTick;
	public GameObject moovableTick;

    public float minWidth;
	public float maxWidth;
	
	public int count;
    
	public AnimationCurve movableTickAnimCurveOffsetX;
	public AnimationCurve movableTickAnimCurveOffsetY;
	public AnimationCurve movableTickAnimCurveScale;
	public float duration;

	private bool isAnimating;
	private float elapsed;
	private Vector3 startPos;
	System.Action _tickAnimCompletion;
	
	void Update()
	{
		if (isAnimating && elapsed < duration)
		{
			elapsed += Time.deltaTime;
			if (elapsed >= duration)
			{
				_tickAnimCompletion();
				elapsed = 0;
				isAnimating = false;
				var timePr = elapsed / duration;
				PositionTick(timePr);
				moovableTick.SetActive(false);
				return;
			}else
			{
				var timePr = elapsed / duration;
				PositionTick(timePr);
			}
		}
	}
	
	void PositionTick(float timePr)
	{		
		var x = Mathf.Lerp(startPos.x,nonMoovableTick.transform.position.x,timePr) + movableTickAnimCurveOffsetX.Evaluate(timePr);
		var y = Mathf.Lerp(startPos.y,nonMoovableTick.transform.position.y,timePr) + movableTickAnimCurveOffsetY.Evaluate(timePr);
		var s = movableTickAnimCurveScale.Evaluate(timePr);
			
		moovableTick.transform.position = new Vector3(x,y,0);
		moovableTick.transform.localScale = new Vector3(s,s,s);
	}
    
	public void AnimateTickFrom(Vector3 pos, System.Action tickAnimCompletion)
	{
		_tickAnimCompletion = tickAnimCompletion;
		isAnimating = true;
		startPos = pos;
	}
    
    // Start is called before the first frame update
	public void SetDayCount(int count)
	{
        this.count = Mathf.Clamp(count, 0, 30);
        SetupPresents(this.count);
        SetupBar(this.count);
    }

    // Update is called once per frame
    public void SetupBar(int count)
    {
        float pr = ((float)count / 30f);
        float value = Mathf.Lerp(minWidth, maxWidth, pr);
        bar.sizeDelta = new Vector2(value, bar.sizeDelta.y);
    }

    public void SetupPresents(int count)
    {
        if (count >= 8)
        {
            images[0].sprite = openedPresents[0];
			buttons[0].SetActive(false);
		}

        if (count >= 15)
        {
            images[1].sprite = openedPresents[1];
			buttons[1].SetActive(false);
		}

        if (count >= 22)
        {
            images[2].sprite = openedPresents[2];
			buttons[2].SetActive(false);
		}

        if (count >= 30)
        {
            images[3].sprite = openedPresents[3];
			buttons[3].SetActive(false);
		}
    }
    void OnValidate()
    {
        SetupBar(this.count);
    }

}
