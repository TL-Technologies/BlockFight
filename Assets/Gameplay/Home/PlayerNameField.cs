using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameField : MonoBehaviour
{
    InputField inputField;

    public void UpdatePlayerName(string v)
    {
        inputField = GetComponent<InputField>();
        if (inputField == null) return;
        Profile.Instance.PlayerName = v;
    }

	private void OnEnable()
	{
        inputField = GetComponent<InputField>();
        if (inputField == null) return;
        inputField.text = Profile.Instance.PlayerName;
    }
}
