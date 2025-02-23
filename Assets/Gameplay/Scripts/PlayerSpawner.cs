using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerSpawner : MonoBehaviour
{
    const string PLAY_COUNT_PREFIX = "PLAY_COUNT_PREFIX_";
    [SerializeField] PlayerType type;
    public PlayerType Type => type;
    [SerializeField] string skin;
    public string Skin
    {
        get
		{
            if(type == PlayerType.Player)
			{
                return OutfitItem.Instance.Current;
            } else
			{
                int playCount = PlayerPrefs.GetInt(PLAY_COUNT_PREFIX + skin, 0);
                return playCount <= 0 ? skin : OutfitItem.Instance.RandomItem(OutfitItem.Instance.Current);
            }
        }
    }

	private void OnEnable()
	{
        EventManager.Instance.AddListener(GameEvent.OnLevelStart, OnLevelStart);
	}

	private void OnLevelStart(GameEvent Event_Type, Component Sender, object Param)
	{
        int playCount = PlayerPrefs.GetInt(PLAY_COUNT_PREFIX + skin, 0);
        PlayerPrefs.SetInt(PLAY_COUNT_PREFIX + skin, playCount + 1);
    }

	public enum PlayerType
	{
        Player,
        Enemy
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
    {
        Handles.color = type == PlayerType.Player ? Handles.zAxisColor : Handles.xAxisColor;
        Handles.CylinderHandleCap(
                0,
                transform.position + Vector3.up * 0.5f,
                transform.rotation * Quaternion.LookRotation(Vector3.up),
                1,
                EventType.Repaint
            );
       
        Handles.ConeHandleCap(
            0,
            transform.position + Vector3.up * 0.75f + transform.forward * 0.75f,
            transform.rotation * Quaternion.LookRotation(Vector3.forward),
            0.5f,
            EventType.Repaint
        );
    }
#endif
}
