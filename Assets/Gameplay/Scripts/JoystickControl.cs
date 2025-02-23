using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

public class JoystickControl : OnScreenControl, IPointerUpHandler, IDragHandler
{
    [InputControl(layout = "Vector2")]
    [SerializeField] private string m_ControlPath;
	[InputControl(layout = "Vector2")]
	[SerializeField] private string m_ControlPathIOS;

	Joystick joystick;

	private void Awake()
	{
		joystick = GetComponent<Joystick>();
	}

	protected override string controlPathInternal {
		get
		{
#if UNITY_IPHONE
			return m_ControlPathIOS;
#endif
			return m_ControlPath;
		}
		set => m_ControlPath = value;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		SendValueToControl(Vector2.zero);
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (joystick == null) return;
		SendValueToControl(joystick.Direction);
	}
}
