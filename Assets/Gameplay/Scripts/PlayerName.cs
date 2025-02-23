using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerName : MonoBehaviour
{
    [SerializeField] bool staticName = false;
    [SerializeField] CharacterBase character;
    [SerializeField] Color playerColor;
    [SerializeField] Color enemyColor;
    Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        if (text == null || character == null) return;

        if (staticName) return;

        if(character.isPlayer)
		{
            text.text = Profile.Instance.PlayerName;
            text.color = playerColor;
		} else
		{
            text.text = AINamesGenerator.Utils.GetRandomName();
            text.color = enemyColor;
        }
    }

    void OnLevelStart(GameEvent Event_Type, Component Sender, object Param = null)
	{
        if (text == null) return;
        text.DOFade(0, 0.6f).SetDelay(2f);
    }

	private void OnEnable()
	{
        EventManager.Instance.AddListener(GameEvent.OnLevelStart, OnLevelStart);
	}

	private void OnDisable()
	{
        if (EventManager.Instance == null) return;
        EventManager.Instance.RemoveEvent(GameEvent.OnLevelStart, OnLevelStart);
	}
}
