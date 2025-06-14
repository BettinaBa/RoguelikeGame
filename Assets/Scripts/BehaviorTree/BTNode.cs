using System;
using System.Collections.Generic;

public abstract class BTNode
{
    public enum State { Success, Failure, Running }
    public abstract State Tick();
}

public class Sequence : BTNode
{
    private readonly List<BTNode> children;
    private int index = 0;

    public Sequence(IEnumerable<BTNode> nodes)
    {
        children = new List<BTNode>(nodes);
    }

    public override State Tick()
    {
        while (index < children.Count)
        {
            var state = children[index].Tick();
            if (state == State.Running)
                return State.Running;
            if (state == State.Failure)
            {
                index = 0;
                return State.Failure;
            }
            index++;
        }
        index = 0;
        return State.Success;
    }
}

public class Selector : BTNode
{
    private readonly List<BTNode> children;
    private int index = 0;

    public Selector(IEnumerable<BTNode> nodes)
    {
        children = new List<BTNode>(nodes);
    }

    public override State Tick()
    {
        while (index < children.Count)
        {
            var state = children[index].Tick();
            if (state == State.Success)
            {
                index = 0;
                return State.Success;
            }
            if (state == State.Running)
                return State.Running;
            index++;
        }
        index = 0;
        return State.Failure;
    }
}

public class ActionNode : BTNode
{
    private readonly Func<State> action;
    public ActionNode(Func<State> act) { action = act; }
    public override State Tick() => action();
}
