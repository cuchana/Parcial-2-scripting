using System;
using System.Collections.Generic;
using System.Threading;

namespace Taller_POO
{
    public abstract class Node
    {
        protected List<Node> children = new List<Node>();

        public virtual void AddChild(Node child)
        {
            children.Add(child);
        }

        public abstract bool Execute();
    }

    public class Root : Node
    {
        private Node child;

        public void SetChild(Node node)
        {
            if (node is Root)
                throw new InvalidOperationException("Root no puede tener otro Root como hijo.");
            child = node;
        }

        public override bool Execute()
        {
            return child?.Execute() ?? false;
        }
    }

    public abstract class Composite : Node
    {
        protected Composite() : base() { }

        public override void AddChild(Node child)
        {
            if (child is Root)
                throw new InvalidOperationException("Composite no puede tener Root como hijo.");
            base.AddChild(child);
        }
    }

    public class Selector : Composite
    {
        public override bool Execute()
        {
            foreach (var child in children)
            {
                if (child.Execute()) return true;
            }
            return false;
        }
    }

    public class Sequence : Composite
    {
        public override bool Execute()
        {
            foreach (var child in children)
            {
                if (!child.Execute()) return false;
            }
            return true;
        }
    }

    public abstract class TaskNode : Node
    {
        protected TaskNode() : base() { }

        public override void AddChild(Node child)
        {
            throw new InvalidOperationException("Task no puede tener hijos.");
        }
    }

    public class CheckEvenNumberTask : TaskNode
    {
        private int number;

        public CheckEvenNumberTask(int num)
        {
            number = num;
        }

        public override bool Execute()
        {
            return number % 2 == 0;
        }
    }

    public class CheckDistanceTask : TaskNode
    {
        private float objectDistance;
        private float validDistance;

        public float ObjectDistance => objectDistance;
        public float ValidDistance => validDistance;

        public CheckDistanceTask(float objDist, float validDist)
        {
            objectDistance = objDist;
            validDistance = validDist;
        }

        public override bool Execute()
        {
            return objectDistance <= validDistance;
        }
    }

    public class MoveToTargetTask : TaskNode
    {
        private CheckDistanceTask checkDistanceTask;
        private float position;
        private float stepSize;

        public MoveToTargetTask(CheckDistanceTask checkDistance, float step = 1.0f)
        {
            checkDistanceTask = checkDistance;
            position = checkDistance.ObjectDistance;
            stepSize = step;
        }

        public override bool Execute()
        {
            float targetPosition = checkDistanceTask.ValidDistance;

            while (position < targetPosition)
            {
                position += stepSize;
                if (position > targetPosition) position = targetPosition;
            }

            return true;
        }
    }

    public class WaitTask : TaskNode
    {
        private int waitTime;

        public WaitTask(int time)
        {
            waitTime = time;
        }

        public override bool Execute()
        {
            Thread.Sleep(waitTime);
            return true;
        }
    }

    public class BehaviourTree
    {
        private Root root;

        public BehaviourTree(Root rootNode)
        {
            root = rootNode ?? throw new ArgumentNullException(nameof(rootNode));
        }

        public bool Execute()
        {
            return root.Execute();
        }
    }
}
