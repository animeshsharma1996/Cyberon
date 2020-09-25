using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Assertions;

public class GridControl : SSystem<GridControl>
{
    [Header ("Stealth Phase Palette")]
    [SerializeField]
    private Color PathReach = Color.white;
    [SerializeField]
    private Color PathNextReach = Color.white;
    [SerializeField]
    private Color PathOutOfReach = Color.white;

    [SerializeField]
    private Color PlayMap = Color.white;
    [SerializeField]
    private Color PlayMapReach = Color.white;

    [Header ("Hunt Tile Palette")]
    [SerializeField]
    private Color HuntPathReach = Color.white;
    [SerializeField]
    private Color HuntPathNextReach = Color.white;
    [SerializeField]
    private Color HuntPathOutOfReach = Color.white;

    [SerializeField]
    private Color HuntPlayMap = Color.white;
    [SerializeField]
    private Color HuntPlayMapReach = Color.white;

    [Header("Detector Palette")]
    [SerializeField]
    private Color DetectorReach = Color.white;
    [SerializeField]
    private Color HuntDetectorReach = Color.white;
    [SerializeField]
    private Color NoDetectorColour = Color.white;


    [Header ("TilesBases")]
    [SerializeField]
    private TileBase PlayTile = null;
    [SerializeField]
    private TileBase PathTile = null;
    [SerializeField]
    private TileBase DetectorTile = null;

    // Color sets
    private Color pathReachColor = Color.white;
    private Color pathNextReachColor = Color.white;
    private Color pathOutOfReachColor = Color.white;
    private Color playMapColor = Color.white;
    private Color playMapReachColor = Color.white;
    private Color detectorReachColor = Color.white;

    // Grid Data
    private Tilemap levelMap = null;
    private Tilemap pathMap = null;
    private Tilemap detectorMap = null;

    private Node[,] gridOfNodes;
    private BoundsInt bounds;

    // Used in optimising path checks
    private Node lastSelectedNode = new Node(0,0);

    public void InitialiseMaps(LevelGridType gridMap)
    {
        // Set colours
        playMapColor = PlayMap;
        playMapReachColor = PlayMapReach;

        pathReachColor = PathReach;
        pathNextReachColor = PathNextReach;
        pathOutOfReachColor = PathOutOfReach;

        detectorReachColor = DetectorReach;

        // Obtain LevelMap 
        foreach (Transform child in gridMap.gameObject.transform)
        {
            if (child.gameObject.name == "LevelMap")
                levelMap = child.GetComponent<Tilemap>();
        }
        Assert.AreNotEqual(null, levelMap, "Grid Control did not find LevelMap in level Grid");

        // Create PathMap
        GameObject pathMapObj = new GameObject("PathMap");
        pathMapObj.layer = LayerMask.NameToLayer("TileMap");

        pathMapObj.transform.parent = gridMap.transform;
        pathMapObj.AddComponent<Tilemap>();
        pathMapObj.AddComponent<TilemapRenderer>();
        pathMap = pathMapObj.GetComponent<Tilemap>();
        pathMap.orientation = Tilemap.Orientation.XZ;
        Assert.AreNotEqual(null, pathMap, "Grid Control did not find PathMap in level Grid");

        // Create DetectorMap
        GameObject detectorMapObj = new GameObject("DetectorMap");
        //detectorMapObj.layer = LayerMask.NameToLayer("TileMap");
        detectorMapObj.transform.parent = gridMap.transform;
        detectorMapObj.AddComponent<Tilemap>();
        detectorMapObj.AddComponent<TilemapRenderer>();
        detectorMap = detectorMapObj.GetComponent<Tilemap>();
        detectorMap.orientation = Tilemap.Orientation.XZ;
        Assert.AreNotEqual(null, detectorMap, "Grid Control did not find DetectorMap in level Grid");

        // Set up map bounds
        pathMap.CompressBounds();
        levelMap.CompressBounds();
        detectorMap.CompressBounds();
        bounds = levelMap.cellBounds;

        // Obtain grid info
        CreateGridOfNodes();
    }

    public void RefreshPlayMap()
    {
        levelMap.RefreshAllTiles();
        foreach (Node n in gridOfNodes)
        {
            levelMap.SetTileFlags(n.Vec3Int, TileFlags.None);
            levelMap.SetColor(n.Vec3Int, playMapColor);
        }
    }

    // Draws the path from the player character to the targetPos (targetPos should be raycasted mouse position)
    public void DrawPath(Vector3 characterPos, Vector3 targetPos, int range, int max_range)
    {
        Node endNode = GetMapNode(targetPos);

        if (endNode == lastSelectedNode) return;
        else lastSelectedNode = endNode;
        pathMap.ClearAllTiles();

        //Node startNode = GetMapNode(characterPos);

        List<Node> path = GetPathToFollow(characterPos, targetPos);
        //List<Node> path = AStar.FindPath(startNode, endNode, 1000);

        GenerateCursorMessage(path);

        if (path == null) return;

        // Draw new path
        for (int i = 0; i < path.Count; i++)
        {
            Node node = path[i];
            pathMap.SetTile(node.Vec3Int, PathTile);
            pathMap.SetTileFlags(node.Vec3Int, TileFlags.None);

            if (i >= path.Count - range)
                pathMap.SetColor(node.Vec3Int, pathReachColor);
            else if (i >= (path.Count - (range + max_range)))
                pathMap.SetColor(node.Vec3Int, pathNextReachColor);
            else pathMap.SetColor(node.Vec3Int, pathOutOfReachColor);
        }
    }

    private void CreateGridOfNodes()
    {
        int rows = bounds.size.x;
        int columns = bounds.size.y;
        gridOfNodes = new Node[rows, columns];
        
        
        // For every tile
        for (int i = 0; i < rows; i++) // columnd [0: 2x bounds.size.y] // bounds.size.y = 2*extends
        {
            for (int j = 0; j < columns; j++) // rows [0: bounds.size.x]
            {
                // Create a Node
                gridOfNodes[i, j] = new Node(i + bounds.xMin, j + bounds.yMin);

                //Check if it is occupied
                Vector3Int tilePos = new Vector3Int(i + bounds.xMin,j + bounds.yMin,0);

                //Check if it is occcupied
                if (levelMap.HasTile(tilePos))
                {
                    if (levelMap.GetTile(tilePos).name == "UnwalkableTile")
                    {
                        gridOfNodes[i, j].isOccupied = true;

                    }
                }
                else
                {
                    levelMap.SetTile(tilePos, PlayTile);
                    levelMap.SetTileFlags(tilePos, TileFlags.None);
                    levelMap.SetColor(tilePos, playMapColor);
                }
            }
        }

        for(int i = 0; i < rows; i++)
        {
            for(int j = 0; j < columns; j++)
            {
                gridOfNodes[i, j].AddNeighbours(gridOfNodes, i, j);
            }
        }

        Node playerStartNode = GetMapNode(GameStateManager.Instance.Player.gameObject.transform.position);
        List<Node> playField = AStar.FindPlayField(playerStartNode);

        // Mark all nodes as occupied
        foreach (Node node in gridOfNodes)
        {
            node.isOccupied = true;
            node.isObstructed = true;
            Vector3Int tilePos = node.Vec3Int;
            levelMap.SetTile(tilePos, null);
        }

        // Mark all play field nodes as not occupied
        foreach (Node node in playField)
        {
            node.isOccupied = false;
            Vector3Int tilePos = node.Vec3Int;
            levelMap.SetTile(tilePos, PlayTile);
            levelMap.SetTileFlags(tilePos, TileFlags.None);
            levelMap.SetColor(tilePos, playMapColor);

            node.isObstructed = false;
            detectorMap.SetTile(tilePos, DetectorTile);
            detectorMap.SetTileFlags(tilePos, TileFlags.None);
        }

    }

    // Returns a Node corresponding to the given position if such exists
    public Node GetMapNode(Vector3 anyPosition)
    { 
        int tileX = levelMap.WorldToCell(anyPosition).x;
        int tileY = levelMap.WorldToCell(anyPosition).y;
        Vector3 mapTilePos = new Vector3(tileX, tileY, 0);

        int x = (int)(mapTilePos.x - bounds.xMin);
        int y = (int)(mapTilePos.y - bounds.yMin);

        if (x < 0 || y < 0 || x > gridOfNodes.GetUpperBound(0) || y > gridOfNodes.GetUpperBound(1))
            return null; 

        return gridOfNodes[x, y];
    }

    public bool IsInRange(Vector3 obj, Vector3 target, float range)
    {
        return Vector3.Distance(obj, target) <= range;
    }

    public void DisplayRange(Vector3 characterPos, int range, int maxRange)
    {
        Node startNode = GetMapNode(characterPos);
        if (startNode == null) return;


        List<Node> rangeNodesNext = AStar.FindRange(startNode, range + maxRange);
        List<Node> rangeNodes = AStar.FindRange(startNode, range);

        foreach (Node node in rangeNodesNext)
        {
            levelMap.SetTileFlags(node.Vec3Int, TileFlags.None); // might not be needed
            levelMap.SetColor(node.Vec3Int, pathNextReachColor); // TODO: SWITCH TO MAP REACH COLOUR
        }

        foreach (Node node in rangeNodes)
        {
          //  levelMap.SetTileFlags(node.Vec3Int, TileFlags.None); // might not be needed
            levelMap.SetColor(node.Vec3Int, playMapReachColor);
        }

    }

    public List<Node> SetAndGetDetectorMapLayer(Vector3 pos, int range)
    {
        Node startNode = GetMapNode(pos);
        if (startNode == null) return null;

        Debug.Log("---------------------------");
        List<Node> rangeNodes = AStar.FindDetectorRange(startNode, range);
        foreach(Node node in rangeNodes)
        {
            detectorMap.SetTileFlags(node.Vec3Int, TileFlags.None);
            detectorMap.SetColor(node.Vec3Int, detectorReachColor);
            node.numDetectors++;
            node.isDetected = true;
        }

        return rangeNodes;
    }

    public void RemoveDetectorMapLayer(List<Node> layer)
    {
        foreach (Node node in layer)
        {
            node.numDetectors--;
            if (node.numDetectors == 0)
            {
                node.isDetected = false;
                detectorMap.SetColor(node.Vec3Int, NoDetectorColour);
            }
        }
    }

    private void GenerateCursorMessage(List <Node> path)
    {

        if (path == null)
        {
            UIManager.Instance.cursorFollowText.SetText("No route");
            UIManager.Instance.cursorFollowText.SetColor(Color.red);

            return;
        }

        //// Check if it is in range of an actor
        bool detected = false;
        int tileIndex = 0;
        while (!detected && tileIndex < path.Count)
        {
            if (path[tileIndex].isDetected)
            {
                UIManager.Instance.cursorFollowText.SetText("Detected");
                UIManager.Instance.cursorFollowText.SetColor(Color.red);
                detected = true;
                foreach (var shooter in GameStateManager.Instance.AllShooters)
                {
                    Detector detector = shooter.GetComponent<Detector>();
                    if (detector.DoesItDetect(path[tileIndex]))
                    {
                        UIManager.Instance.cursorFollowText.SetText("Death");
                    }
                }
                break;
            }
            tileIndex++;
        }

        if (detected) return;

        UIManager.Instance.cursorFollowText.SetText("Move to");
        UIManager.Instance.cursorFollowText.SetColor(Color.green);

    }

    // Uses target position to find end node.
    public List<Node> GetPathToFollow(Vector3 characterPos, Vector3 targetPos)
    {
        // CreateGridOfNodes();
        Node startNode = GetMapNode(characterPos);
        Node endNode = GetMapNode(targetPos);
        List<Node> path = null;

        if (GameStateManager.Instance.Phase == GamePhase.Stealth)
            path = AStar.FindUndetectedPath(startNode, endNode, 1000);

        if (path == null)
        {
            path = AStar.FindPath(startNode, endNode, 1000);
        }
        return path;
    }

    public void ChangeToHuntPalette()
    {
        // Set hunt colours
        playMapColor = HuntPlayMap;
        playMapReachColor = HuntPlayMapReach;

        pathReachColor = HuntPathReach;
        pathNextReachColor = HuntPathNextReach;
        pathOutOfReachColor = HuntPathOutOfReach;

        detectorReachColor = HuntDetectorReach;

        RefreshPlayMap();
    }


    public void OccupyTile(Vector3 pos)
    {
        Node node = GetMapNode(pos);
        node.isOccupied = true;
        levelMap.SetTile(node.Vec3Int, null);
    }

    public void ObstructTile(Vector3 pos)
    {
        Node node = GetMapNode(pos);
        node.isObstructed = true;
    }

    public void UnObstructTile(Vector3 pos)
    {
        Node node = GetMapNode(pos);
        node.isObstructed = false;
    }

    public void FreeTile(Vector3 pos)
    {
        Node node = GetMapNode(pos);
        node.isOccupied = false;
        levelMap.SetTile(node.Vec3Int, PlayTile);
        levelMap.SetTileFlags(node.Vec3Int, TileFlags.None);
        levelMap.SetColor(node.Vec3Int, playMapColor);
    }

    public void EnablePlayMap()
    {
        levelMap.enabled = true;
    }

    public void DisablePlayMap()
    {
        levelMap.enabled = false;
    }

    public void ClearPath()
    {
        pathMap.ClearAllTiles();
    }


}
