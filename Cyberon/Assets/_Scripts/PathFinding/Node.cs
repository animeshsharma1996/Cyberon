using System.Collections.Generic;
using UnityEngine;

public class Node 
{
    public int gridX;
    public int gridY;
    public int gridZ = 0;
    public bool isOccupied;


    public bool isDetected;
    public bool isObstructed;
    public int numDetectors;
    // For pathfinding
    public Node Parent;
    public int gCost;
    public int hCost;
    public List<Node> neighbours;
    public Node previous = null;

    public int FCost { get { return gCost + hCost; } }

    // Creation
    public Node(int x, int y)
    {
        gridX = x;
        gridY = y;
 
        neighbours = new List<Node>();
        isOccupied = false;
        isDetected = false;
    }

    public void AddNeighbours(Node[,] grid, int x, int y)
    {
        if (x < grid.GetUpperBound(0))
            neighbours.Add(grid[x + 1, y]);
        if (x > 0)
            neighbours.Add(grid[x - 1, y]);
        if (y < grid.GetUpperBound(1))
            neighbours.Add(grid[x, y + 1]);
        if (y > 0)
            neighbours.Add(grid[x, y - 1]);
    }


    // Interpretation:
    public Vector3Int Vec3Int { get => new Vector3Int(gridX, gridY, gridZ); }
    public Vector3 Vec3 { get => new Vector3(gridX, gridY, gridZ); }
    public Vector3 WorldPos { get => new Vector3(gridX + 0.5f, gridZ, gridY + 0.5f); }

    // For Debug
    public void Print()
    {
        Debug.LogWarning("(" + gridX + ", " + gridY + ", " + isOccupied + ", " + isObstructed +")");
    }

    public void PrintNeighbours()
    {
        foreach (var n in neighbours) n.Print();
    }
}
