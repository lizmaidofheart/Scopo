using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BrainState
{
    public string name;
    protected CryptidBrain brain;

    public BrainState(string name, CryptidBrain brain)
    {
        this.name = name;
        this.brain = brain;
    }

    public virtual void Enter() { }

    public virtual void UpdateLogic() { }

    public virtual void Exit() { }

}
