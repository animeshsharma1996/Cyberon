using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
    private static bool IsValidPath(Node start, Node end)
    {
        if (end == null)
            return false;
        if (start == null)
            return false;
        //if (end.isObstructed)
        //    return false;
        if (end.isObstructed)
            return false;
        if (start == end)
            return false;
        return true;
    }

    // TODO: Can be refactored to always require input grid of nodes and remove the initiliser completely
    public static List<Node> FindPath(Node start, Node end, int length)
    {
        if (!IsValidPath(start, end))
            return null;

        List<Node> openSet = new List<Node>();
        List<Node> closedSet = new List<Node>();

        openSet.Add(start);

        while (openSet.Count > 0)
        {
            //Find shortest step distance in the direction of your goal within the open set
            int winner = 0;
            for (int i = 0; i < openSet.Count; i++)
                if (openSet[i].FCost < openSet[winner].FCost)
                    winner = i;
                else if (openSet[i].FCost == openSet[winner].FCost) //tie breaking for faster routing
                        if (openSet[i].hCost < openSet[winner].hCost)
                            winner = i;

            var current = openSet[winner];

            //Found the path, creates and returns the path
            if (openSet[winner] == end)
            {
                List<Node> Path = new List<Node>();
                var temp = current;
                Path.Add(temp);
                while (temp.previous != start)
                {
                    Path.Add(temp.previous);
                    temp = temp.previous;
                }
                if (length - (Path.Count) < 0)
                {
                    Path.RemoveRange(0, (Path.Count - 1) - length);
                }
                return Path;
            }

            openSet.Remove(current);
            closedSet.Add(current);

            var neighbours = current.neighbours;
            for (int i = 0; i < neighbours.Count; i++)
            {
                var n = neighbours[i];
                // if (n.isOccupied);
                if(n.isOccupied && n != end)
                {
                    closedSet.Add(n);
                    continue;
                }
                if (!closedSet.Contains(n))
                {
                    var tempG = current.gCost + 1;

                    bool newPath = false;
                    if (openSet.Contains(n)) 
                    {
                        if (tempG < n.gCost)
                        {
                            n.gCost = tempG;
                            newPath = true;
                        }
                    }
                    else
                    {
                        n.gCost = tempG;
                        newPath = true;
                        openSet.Add(n);
                    }
                    if (newPath)
                    {
                        n.hCost = Heuristic(n, end);
                        n.previous = current;
                    }
                }
            }

        }
        // End [2]
        return null;
    }

    public static List<Node> FindUndetectedPath(Node start, Node end, int length)
    {
        if (!IsValidPath(start, end))
            return null;

        List<Node> openSet = new List<Node>();
        List<Node> closedSet = new List<Node>();

        openSet.Add(start);

        while (openSet.Count > 0)
        {
            //Find shortest step distance in the direction of your goal within the open set
            int winner = 0;
            for (int i = 0; i < openSet.Count; i++)
                if (openSet[i].FCost < openSet[winner].FCost)
                    winner = i;
                else if (openSet[i].FCost == openSet[winner].FCost) //tie breaking for faster routing
                    if (openSet[i].hCost < openSet[winner].hCost)
                        winner = i;

            var current = openSet[winner];

            //Found the path, creates and returns the path
            if (openSet[winner] == end)
            {
                List<Node> Path = new List<Node>();
                var temp = current;
                Path.Add(temp);
                while (temp.previous != start)
                {
                    Path.Add(temp.previous);
                    temp = temp.previous;
                }
                if (length - (Path.Count) < 0)
                {
                    Path.RemoveRange(0, (Path.Count - 1) - length);
                }
                return Path;
            }

            openSet.Remove(current);
            closedSet.Add(current);

            var neighbours = current.neighbours;
            for (int i = 0; i < neighbours.Count; i++)
            {
                var n = neighbours[i];
                // if (n.isOccupied);
                if (n.isOccupied || n.isDetected)
                {
                    closedSet.Add(n);
                    continue;
                }
                else if (!closedSet.Contains(n))
                {
                    var tempG = current.gCost + 1;

                    bool newPath = false;
                    if (openSet.Contains(n))
                    {
                        if (tempG < n.gCost)
                        {
                            n.gCost = tempG;
                            newPath = true;
                        }
                    }
                    else
                    {
                        n.gCost = tempG;
                        newPath = true;
                        openSet.Add(n);
                    }
                    if (newPath)
                    {
                        n.hCost = Heuristic(n, end);
                        n.previous = current;
                    }
                }
            }

        }
        // End [2]
        return null;
    }

    private static int Heuristic(Node a, Node b)
    {
        //manhattan
        var dx = Mathf.Abs(a.gridX - b.gridX);
        var dy = Mathf.Abs(a.gridY - b.gridY);
        return 1 * (dx + dy);
    }



    private static HashSet<Node> visitedNodes;
    private static Queue<Node> nodesToCheck;

    public static List<Node> FindRange(Node startNode, int range)
    {
        List<Node> rangeNodes = new List<Node>();
        visitedNodes = new HashSet<Node>();
        nodesToCheck = new Queue<Node>();

        nodesToCheck.Enqueue(startNode);
        visitedNodes.Add(startNode);

        int level = range;
        while (level > 0)
        {
            int nodesInCurrentLevel = nodesToCheck.Count;

            while (nodesInCurrentLevel > 0)
            {
                // Add the visited node
                Node checkNode = nodesToCheck.Dequeue();
                nodesInCurrentLevel--;

                // Check for any neighbours
                foreach (Node neighbour in checkNode.neighbours)
                {
                    if (!visitedNodes.Contains(neighbour) && !neighbour.isOccupied)
                    {
                        nodesToCheck.Enqueue(neighbour);
                        visitedNodes.Add(neighbour);
                    }
                }
            }
            level--;
        }

        // After DFS finishes fill the List of nodes from the HashSet
        foreach (Node n in visitedNodes)
            rangeNodes.Add(n);

        return rangeNodes;
    }

    public static List<Node> FindPlayField(Node startNode)
    {
        List<Node> playNodes = new List<Node>();

        // Create visited 2D array
        visitedNodes = new HashSet<Node>();

        // Mark nodes that are part of the playfield
        DFSPlayMark(startNode);

        // After DFS finishes fill the List of nodes from the HashSet
        foreach (Node n in visitedNodes)
            playNodes.Add(n);

        return playNodes;
    }

    private static void DFSPlayMark(Node node)
    {
        if (!node.isOccupied) visitedNodes.Add(node);

        foreach (Node n in node.neighbours)
        {
            if (!n.isOccupied && !visitedNodes.Contains(n))
                DFSPlayMark(n);
        }
    }


    public static List<Node> FindDetectorRange(Node startNode, int range)
    {
        List<Node> rangeNodes = new List<Node>();
        visitedNodes = new HashSet<Node>();
        nodesToCheck = new Queue<Node>();

        nodesToCheck.Enqueue(startNode);
        visitedNodes.Add(startNode);

        int level = range;
        while (level > 0)
        {
            int nodesInCurrentLevel = nodesToCheck.Count;

            while (nodesInCurrentLevel > 0)
            {
                // Add the visited node
                Node checkNode = nodesToCheck.Dequeue();
                nodesInCurrentLevel--;

                // Check for any neighbours
                foreach (Node neighbour in checkNode.neighbours)
                {
                    if (!visitedNodes.Contains(neighbour) && !neighbour.isObstructed)
                    {
                        nodesToCheck.Enqueue(neighbour);
                        visitedNodes.Add(neighbour);
                    }
                }
            }
            level--;
        }

        // After DFS finishes fill the List of nodes from the HashSet
        foreach (Node n in visitedNodes)
            rangeNodes.Add(n);

        return rangeNodes;
    }
}

