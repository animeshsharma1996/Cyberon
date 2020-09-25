using System.Collections.Generic;
using UnityEngine;

public class ObjectivesQueue
{
    Queue<Objective> queue = new Queue<Objective>();
    public int currentObjectivIndex = 0;

    public ObjectivesQueue()
    {
        List<Objective> objectives = new List<Objective>(GameObject.FindObjectsOfType<Objective>());
        
        if (objectives.Count > 0)
        {
            objectives.Sort(new ObjectiveComparer());
            foreach (var objective in objectives)
            {
                objective.gameObject.SetActive(false);
                queue.Enqueue(objective);
            }

            AssignObjective();
        }
    }

    public bool OnUpdate()
    {
        queue.Peek().OnUpdate();
        if (queue.Peek().isComplete())
        {
            //Add Score
            int score = GameStateManager.Instance.score;
            score = queue.Peek().AddScore(score);
            GameStateManager.Instance.SetScore(score);

            var objective = queue.Dequeue();
            GameObject.Destroy(objective.gameObject);

            currentObjectivIndex++;

            if (queue.Count == 0)
            {
                return true;
            }

            AssignObjective();
        }
        return false;
    }

    public void ForwardToObjective(int index)
    {
        if (index > queue.Count)
        {
            Debug.LogWarning("Forwarding to objective " + index + " is not possible"
                + " due to objective queue size = " + queue.Count);
            return;
        }

        while (currentObjectivIndex != index)
        {
            var objective = queue.Dequeue();
            GameObject.Destroy(objective.gameObject);

            currentObjectivIndex++;
        }

        AssignObjective();
    }

    public void AssignObjective()
    {
        Objective objective = queue.Peek();
        objective.gameObject.SetActive(true);

        UIManager.Instance.SetObjectivesTitle(objective.Title);
        UIManager.Instance.SetObjectivesDescription(objective.Description);
        UIManager.Instance.SetObjectivesHint(objective.Hint);
        UIManager.Instance.ObjectivesReappear();
    }


    private class ObjectiveComparer : IComparer<Objective>
    {
        public int Compare (Objective a, Objective b)
        {
            if (a.order < b.order) return -1;
            if (a.order > b.order) return 1;
            return 0;
        }
    }


}
