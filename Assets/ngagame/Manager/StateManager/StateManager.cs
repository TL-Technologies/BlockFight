using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{

	private Enum _stateKey;
	public Enum CurrentStateKey => _stateKey;
	public State CurrentState => currentState;

	protected Dictionary<System.Enum, State> states = new Dictionary<Enum, State>();
	protected State currentState = null;
	protected State lastState = null;
	protected object owner;

	public void Configure(object go)
	{
		owner = go;
	}

	public void AddState(System.Enum key, State state)
	{
		states.Add(key, state);
	}

	public void ChangeState(System.Enum key)
	{
		if (states.ContainsKey(key))
		{
			if (currentState != null)
			{
				lastState = currentState;
				lastState.Exit();
			}
			_stateKey = key;
			currentState = states[key];
			currentState.Enter(owner);
			Debug.LogWarningFormat("Change state to: {0}", CurrentStateKey);
		}
		else
		{
			throw new Exception(string.Format("State {0} selectionMode exist in state stack", key));
		}
	}

	public void UpdateExecute()
	{
		if (currentState != null)
		{
			currentState.Execute();
		}
	}
}
