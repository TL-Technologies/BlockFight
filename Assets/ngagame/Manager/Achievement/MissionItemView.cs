using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionItemView : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text descText;
    [SerializeField] Text progressText;
    [SerializeField] Image iconImage;
    [SerializeField] Button claimButton;
    [SerializeField] CollectAnimation collectAnimation;
    [SerializeField] GameObject completedObject;

    Mission mission;
    const int REWARD_GEM = 50;

    public void Init(Mission _mission)
	{
        mission = _mission;
        Refresh();
    }

    void Refresh()
	{
        nameText.text = mission.achievement.name;
        descText.text = string.Format(mission.achievement.description, mission.require);
        progressText.text = $"{mission.current}/{mission.require}";
        iconImage.sprite = Resources.Load<Sprite>(mission.achievement.id);
        claimButton.interactable = mission.completed;
        completedObject.gameObject.SetActive(mission.claimed);
        claimButton.gameObject.SetActive(!mission.claimed);
    }

    public void OnClaimClick()
	{
        collectAnimation.Collect(delegate
        {
            Profile.Instance.Gem += REWARD_GEM;
        }, null);
        claimButton.interactable = false;
        if(mission != null && Achievement.Instance != null)
		{
            Achievement.Instance.Claim(mission.achievement.id);
            EventManager.Instance.PostNotification(GameEvent.OnMissionNotice, this, mission.achievement.id);
            Refresh();
        }
	}
}
