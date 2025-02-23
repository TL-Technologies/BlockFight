using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State {

    public abstract void Enter(object data);
    public abstract void Exit();
    public abstract void Execute();
}
