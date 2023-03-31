using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AttackSceneRevengePanel : MonoBehaviour
{

	public delegate void RevengeUserListItemSelected(UserDetails target, bool isFirstCell);
	public delegate void RevengeHistoryListItemSelected(History target, bool isFirstCell);
	public static event RevengeUserListItemSelected OnUserRevengeListItemSelected;
	public static event RevengeHistoryListItemSelected OnHistoryRevengeListItemSelected;

    public GameObject prefab;

    public GameObject content;
    public GameObject emptyStatePanel;
	public GameObject scrollView;
    
	public BasePopUpAnimator animator;
	
	public DarkOverlay darkOverlay;

	public void SetupForRevengeWith(UserDetails originalTarget, List<AttackHistory> targets, 
		RevengeHistoryListItemSelected delegation, RevengeUserListItemSelected userDelegation)
    {
        emptyStatePanel.SetActive(false);
        scrollView.SetActive(true);
        OnHistoryRevengeListItemSelected = delegation;
        OnUserRevengeListItemSelected = userDelegation;
        SetupWith(originalTarget, targets);
    }

    public void SetupForFriendsWith(List<Friend> friends, RevengeUserListItemSelected delegation)
    {
        if (friends.Count > 0)
        {
            emptyStatePanel.SetActive(false);
            scrollView.SetActive(true);
            OnUserRevengeListItemSelected = delegation;
            SetupWith(friends);
        }
        else
        {
            emptyStatePanel.SetActive(true);
            scrollView.SetActive(false);
        }
    }

    void SetupWith(UserDetails originalTarget, List<AttackHistory> targets)
    {
        DestroyCells();
      
        int i = 0;

        if (originalTarget != null)
        {
            GameObject cell = Instantiate(prefab);
            AttackSceneRevengeListItem model = cell.GetComponent<AttackSceneRevengeListItem>();
            cell.transform.SetParent(content.transform);
            cell.transform.localScale = Vector3.one;
            model.SetupWith(originalTarget, true, false, UserListItemSelected);
        }

        foreach (AttackHistory history in targets)
        {
            GameObject cell = Instantiate(prefab);
            AttackSceneRevengeListItem model = cell.GetComponent<AttackSceneRevengeListItem>();
            cell.transform.SetParent(content.transform);
            cell.transform.localScale = Vector3.one;
            model.SetupWith(history, HistoryListItemSelected);
            i++;
        }
    }

    void SetupWith(List<Friend> targets)
    {
        if (targets.Count == 0)
        {

        }
        else
        {
            DestroyCells();

            int i = 0;
            foreach (Friend target in targets)
            {
                GameObject cell = Instantiate(prefab);
                AttackSceneRevengeListItem model = cell.GetComponent<AttackSceneRevengeListItem>();
                cell.transform.SetParent(content.transform);
                cell.transform.localScale = Vector3.one;
                model.SetupWith(target, false, true, UserListItemSelected);
                i++;
            }
        }
    }


    void DestroyCells()
    {
        if (content.transform.childCount > 0)
        {
            //TODO: we need to think about this.
            foreach (Transform child in content.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            //return; 
        }
    }

	void HistoryListItemSelected(History target, bool isFirstCell)
	{
		HidePanel();
		OnHistoryRevengeListItemSelected(target, isFirstCell);
	}

	void UserListItemSelected(UserDetails target, bool isFirstCell)
	{
		HidePanel();
		OnUserRevengeListItemSelected(target, isFirstCell);
	}

    public void ShowPanel()
	{
		darkOverlay.AnimateIn();
	    animator.AnimateIn();
    }

    public void HidePanel()
	{
		darkOverlay.AnimateOut();
	    animator.AnimateOut();
    }
}
