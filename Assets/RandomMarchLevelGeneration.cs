using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using System.Linq;

public class RandomMarchLevelGeneration : MonoBehaviour {
    [Header("Map Dimensions")]
    [SerializeField] private int width = 10;   // Width of the generated level map.
    [SerializeField] private int height = 10;  // Height of the generated level map.
    private LevelGrid levelGrid;

    [Header("Random Walker Parameters")]
    [SerializeField] private int walkSteps = 20;  // Number of steps each random walker takes.
    [SerializeField] private int maxStepLength = 1;  // Maximum length of a single step.
    [SerializeField, Range(0, 1)] private float backtrackProbability = 0.2f;  // Chance of backtracking.
    [SerializeField] private int numberOfWalkers = 1;  // Number of walkers for map generation.

    [SerializeField] private GameObject wallPrefab, edgePrefab, floorPrefab, islandPrefab;   // Prefab for map tiles.

    [Header("Island Parameters")]
    [SerializeField, Range(0, 1)] private float clusterProbability = 0.5f;  // Probability of island clustering.
    [SerializeField] private PhysicsMaterial2D physicsMaterial;  // Physics material for colliders.

    [Header("Debug")]
    [SerializeField] private LineRenderer edgeLineRenderer;  // Line renderer for debugging edges.

    [Header("Path Smoothing")]
    [SerializeField] private float pathNoise = 0.1f;  // Amount of randomness for path smoothing.

    [Header("Seed")]
    [SerializeField] private int seed;  // Seed for controlling random number generation.

    public SpriteShapeController spriteShape;

    // Lists to cache tiles by type
    // private List<Vector3> edgeTiles;
    // private List<Vector3> islandTiles;
    // private List<Vector3> floorTiles;

    private GameObject mapPool;

    void Start() {
        Random.InitState(seed);
        StartCoroutine(GenerateMap());
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            StartCoroutine(GenerateMap());
        }
    }

    IEnumerator GenerateMap() {
        // 1. Destroy previous map entities, if any.
        DestroyExistingMap();
        yield return new WaitForSecondsRealtime(0f); // Wait for 1 second (or any duration you want)
        CreateMap();

        //int shapeCount = GetComponentInChildren<SpriteShapeController>().spline.GetPointCount();
        //int iterationCount = 0;
        // Try to make a new map 15 times.
        // while(shapeCount < 100 && iterationCount < 25) {
        //     Debug.Log("Map too small, generating new map as points were: " + shapeCount);
            
        //     DestroyExistingMap();
        //     yield return new WaitForSecondsRealtime(0f); // Wait for 1 second (or any duration you want)
            
        //     CreateMap();

        //     shapeCount = GetComponentInChildren<SpriteShapeController>().spline.GetPointCount();
        //     iterationCount++;
        // }
    }

    void DestroyExistingMap() {
        //destroy all children in this gameobject
        if(mapPool != null)Destroy(mapPool);
        mapPool = new GameObject("MapPool");
        mapPool.transform.parent = this.transform;
    }

    void CreateMap() {
        levelGrid = new LevelGrid(width, height);

        for (int i = 0; i < numberOfWalkers; i++) {
            RandomMarchWalker walker = new RandomMarchWalker(walkSteps, maxStepLength, backtrackProbability, width, height);
            walker.Walk(levelGrid);
        }
        DrawDebugGrid();
    }

    public void DrawDebugGrid(){
        //loop through the grid and instantiate prefabs, as child of mapPool
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if(levelGrid.IsWallTile(x, y)){
                    SpriteRenderer s = Instantiate(wallPrefab, new Vector3(x, y, 0), Quaternion.identity, mapPool.transform).GetComponent<SpriteRenderer>();
                    s.color = Color.black;
                }
                else if(levelGrid.IsIslandTile(x, y)){
                    SpriteRenderer s = Instantiate(wallPrefab, new Vector3(x, y, 0), Quaternion.identity, mapPool.transform).GetComponent<SpriteRenderer>();
                    s.color = Color.green;
                }
                else if(levelGrid.IsFloorTile(x, y)){
                    SpriteRenderer s = Instantiate(wallPrefab, new Vector3(x, y, 0), Quaternion.identity, mapPool.transform).GetComponent<SpriteRenderer>();
                    s.color = Color.white;
                }
                else if(levelGrid.IsEdgeTile(x, y)){
                    SpriteRenderer s = Instantiate(wallPrefab, new Vector3(x, y, 0), Quaternion.identity, mapPool.transform).GetComponent<SpriteRenderer>();
                    s.color = Color.grey;
                }
            }
        }
    }

    // void DrawGrid() {
    //     spriteShape.transform.parent = mapPool.transform;
    //     Spline spline = spriteShape.spline;

    //     if(spline.GetPointCount() > 0) {
    //         spline.Clear();
    //     }

    //     List<Vector3> edgePoints = new List<Vector3>();

    //     // Lists to cache tiles by type
    //     edgeTiles = new List<Vector3>();
    //     islandTiles = new List<Vector3>();
    //     floorTiles = new List<Vector3>();

    //     // Iterate through the grid and instantiate prefabs and populate edgePoints
    //     for (int x = 0; x < width; x++) {
    //         for (int y = 0; y < height; y++) {
    //             if (grid[x, y] == Tile.Floor) {
    //                 //tile = Instantiate(floorPrefab, new Vector3(x, y, 0), Quaternion.identity, this.transform);
    //                 floorTiles.Add(new Vector3(x, y, 0)); // Cache floor tile
    //             } else {
    //                 //tile = Instantiate(wallPrefab, new Vector3(x, y, 0), Quaternion.identity, this.transform);
    //                 if (IsEdgeTile(grid[x, y])) {
    //                     edgeTiles.Add(new Vector3(x, y, 0)); // Cache edge tile
    //                     edgePoints.Add(new Vector3(x, y, 0));
    //                 } else if (grid[x, y] == Tile.Island) {
    //                     islandTiles.Add(new Vector3(x, y, 0)); // Cache island tile
    //                     GameObject island = Instantiate(wallPrefab, new Vector3(x, y, 0), Quaternion.identity, mapPool.transform);
    //                     island.AddComponent<BoxCollider2D>();
    //                     island.GetComponent<BoxCollider2D>().sharedMaterial = physicsMaterial;
    //                 }
    //             }
    //         }
    //     }

    //     // Determine the order of edge points using a naive approach
    //     List<Vector3> orderedEdgePoints = new List<Vector3>();

    //     Vector3? start = edgePoints[0];  // This assumes at least one edge point exists. Consider adding a safety check.
    //     orderedEdgePoints.Add(start.Value);
    //     edgePoints.Remove(start.Value);

    //     Vector3 current = start.Value;
    //     Vector3[] directions = { 
    //         new Vector3(0, 1, 0),   // North
    //         new Vector3(1, 0, 0),   // East
    //         new Vector3(0, -1, 0),  // South
    //         new Vector3(-1, 0, 0),  // West
    //         new Vector3(1, 1, 0),   // North-East
    //         new Vector3(1, -1, 0),  // South-East
    //         new Vector3(-1, -1, 0), // South-West
    //         new Vector3(-1, 1, 0)   // North-West
    //     };

    //     while (edgePoints.Count > 0) {
    //         Vector3? next = null;

    //         foreach (Vector3 dir in directions) {
    //             Vector3 neighbor = current + dir;

    //             if (edgePoints.Contains(neighbor)) {
    //                 int surroundingWallTiles = CountAdjacentWallTiles(neighbor, grid);

    //                 if (surroundingWallTiles > 0) {
    //                     next = neighbor;
    //                     break;
    //                 }
    //             }
    //         }

    //         if (next == null) break;  // Exit if no suitable edge point is found

    //         orderedEdgePoints.Add(next.Value);
    //         edgePoints.Remove(next.Value);
    //         current = next.Value;
    //     }

    //     // Insert ordered points into spline
    //     foreach (Vector3 point in orderedEdgePoints) {
    //         //debug the points
    //         //Debug.Log(point);
    //         //add some randomness influence to the points
    //         //Vector3 randomPoint = point + new Vector3(Random.Range(-pathNoise, pathNoise), Random.Range(-pathNoise, pathNoise));
    //         //remove a point randomly by not adding it.
    //         spline.InsertPointAt(spline.GetPointCount(), point);
    //     }

    //     for (int i = 0; i < spline.GetPointCount(); i++) {
    //         Smoothen(spriteShape, i);
    //     }

    //     // Let Unity calculate the smooth edges
    //     spriteShape.RefreshSpriteShape();
    // }

    // private void Smoothen(SpriteShapeController sc, int pointIndex) {

    //     Vector3 position = sc.spline.GetPosition(pointIndex);

    //     Vector3 positionNext = sc.spline.GetPosition(SplineUtility.NextIndex(pointIndex, sc.spline.GetPointCount()));

    //     Vector3 positionPrev = sc.spline.GetPosition(SplineUtility.PreviousIndex(pointIndex, sc.spline.GetPointCount()));

    //     Vector3 forward = gameObject.transform.forward;

    //     float scale = Mathf.Min((positionNext - position).magnitude, (positionPrev - position).magnitude) * 0.33f;

    //     Vector3 leftTangent = (positionPrev - position).normalized * scale;

    //     Vector3 rightTangent = (positionNext - position).normalized * scale;

    //     sc.spline.SetTangentMode(pointIndex, ShapeTangentMode.Continuous);

    //     SplineUtility.CalculateTangents(position, positionPrev, positionNext, forward, scale, out rightTangent, out leftTangent);

    //     sc.spline.SetLeftTangent(pointIndex, leftTangent);

    //     sc.spline.SetRightTangent(pointIndex, rightTangent);

    // }
}
