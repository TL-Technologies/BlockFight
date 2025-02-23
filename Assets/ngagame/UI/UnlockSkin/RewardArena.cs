using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardArena : Scene
{
	[SerializeField] Image preview;

	string arenaInProgress;
	private void OnEnable()
	{
		arenaInProgress = ArenaItem.Instance.GetItemInProgress();
		preview.sprite = ArenaItem.Instance.GetPreview(arenaInProgress);
		ArenaItem.Instance.NewestItem = arenaInProgress;
		ArenaItem.Instance.CreateNewProgress();


	}

	public void OnClaimClick()
	{
		ArenaItem.Instance.Unlock(arenaInProgress.ToString(), false);
		Close();
	}

	public void OnSkipClick()
	{
		Close();
	}

	void Close()
	{
		SceneMaster.Instance.CloseScene(SceneID.RewardArena);
	}
}
