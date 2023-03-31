using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System;

public class AttackSceneRevengeListItem : MonoBehaviour
{

    public delegate void RevengeUserListItemSelected(UserDetails target, bool isFirstCell);
    public delegate void RevengeHistoryListItemSelected(History target, bool isFirstCell);
    public static event RevengeUserListItemSelected OnUserRevengeListItemSelected;
    public static event RevengeHistoryListItemSelected OnHistoryRevengeListItemSelected;

    bool isOriginal;
    object user;

    public ProfileImage image;
    public Button button;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI buttonText;
    public TextMeshProUGUI rankText;
    public Sprite randomButtonSprite, revengeButtonSprite;

    // Start is called before the first frame update
    public void SetupWith(UserDetails target, bool isOriginal, bool isFriends, RevengeUserListItemSelected delegation)
    {
        this.isOriginal = isOriginal;
        this.user = target;
        OnUserRevengeListItemSelected = delegation;
        nameText.text = target.displayName;

        if (isOriginal)
        {
            buttonText.text = String.Format("<color=#3A4197>{0}", Utils.Translate("Attack.Revenge_Panel.List_Item.Current"));
            button.gameObject.GetComponent<Image>().sprite = randomButtonSprite;
        }
        else
        {
            buttonText.text = Utils.Translate("Attack.Revenge_Panel.List_Item.Revenge");
            button.gameObject.GetComponent<Image>().sprite = revengeButtonSprite;
        }

        rankText.text = Utils.LocalizeNumberAmount(target.rank);
        button.onClick.AddListener(OnButtonClick);
        image.SetUri(target.profilePictureUri);
    }

    public void SetupWith(History history, RevengeHistoryListItemSelected delegation)
    {
        this.user = (object)history;
        OnHistoryRevengeListItemSelected = delegation;
        nameText.text = history.displayName;

        if (isOriginal)
        {
            buttonText.text = String.Format("<color=#3A4197>{0}", Utils.Translate("Attack.Revenge_Panel.List_Item.Current"));
            button.gameObject.GetComponent<Image>().sprite = randomButtonSprite;
        }
        else
        {
            buttonText.text = Utils.Translate("Attack.Revenge_Panel.List_Item.Revenge");
            button.gameObject.GetComponent<Image>().sprite = revengeButtonSprite;
        }

        rankText.text = "";
        button.onClick.AddListener(OnButtonClick);
        image.SetUri(history.profilePictureUri);
    }

    public void SetupWith(Friend target, bool isOriginal, bool isFriends, RevengeUserListItemSelected delegation)
    {
        this.isOriginal = isOriginal;
        this.user = target.userDetails;

        OnUserRevengeListItemSelected = delegation;
        nameText.text = target.userDetails.displayName;

        button.gameObject.GetComponent<Image>().sprite = revengeButtonSprite;
        buttonText.text = Utils.Translate("Attack.Revenge_Panel.List_Item.Attack");

        rankText.text = Utils.LocalizeNumberAmount(target.userDetails.rank);
        button.onClick.AddListener(OnButtonClick);
        image.SetUri(target.userDetails.profilePictureUri);
    }

    void OnButtonClick()
    {
        if (user is UserDetails)
        {
            OnUserRevengeListItemSelected((UserDetails)user, isOriginal);
        }
        else if (user is History)
        {
            OnHistoryRevengeListItemSelected((History)user, isOriginal);
        }
    }
}
