using System.Collections.Generic;
using UnityEngine;

// The LevelGrid class manages the grid layout for a level, including tile types and grid manipulation.
public class LevelGrid {
    private Tile[,] grid; // 2D array representing the grid layout
    private int width; // Width of the grid
    private int height; // Height of the grid

    // Enumeration defining different types of tiles in the grid
    public enum Tile {
        Wall,
        Floor,
        EdgeTile,
        Island
    }

    // Constructor to initialize the grid with specified dimensions
    public LevelGrid(int width, int height) {
        this.width = width;
        this.height = height;
        InitializeGrid();
    }

    // Initializes the grid, setting all tiles to walls initially
    private void InitializeGrid() {
        grid = new Tile[width, height];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                grid[x, y] = Tile.Wall;
            }
        }
    }

    // Helper method to check if a given coordinate is within the bounds of the grid
    private bool WithinBounds(int x, int y) {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    // Retrieves the type of tile at a given coordinate
    public Tile GetTile(int x, int y) {
        return WithinBounds(x, y) ? grid[x, y] : Tile.Wall;
    }

    // Retrieves the dimensions of the grid
    public Vector2 GetDimentions() {
        return new Vector2(width, height);
    }

    // Sets the type of tile at a given coordinate
    public void SetTile(int x, int y, Tile tileType) {
        if (WithinBounds(x, y)) {
            grid[x, y] = tileType;
        }
    }

    // Methods to check the type of a tile at a given coordinate
    public bool IsEdgeTile(int x, int y) {
        return GetTile(x, y) == Tile.EdgeTile;
    }
    public bool IsFloorTile(int x, int y) {
        return GetTile(x, y) == Tile.Floor;
    }
    public bool IsWallTile(int x, int y) {
        return GetTile(x, y) == Tile.Wall;
    }
    public bool IsIslandTile(int x, int y) {
        return GetTile(x, y) == Tile.Island;
    }

    // Detects edges in the grid and updates tiles accordingly
    public void DetectEdges() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                // Check adjacent tiles to determine if the current tile is an edge
                if (grid[x, y] == Tile.Floor) {
                    // Update adjacent wall tiles to edge tiles
                    if (y + 1 < height && grid[x, y + 1] == Tile.Wall) grid[x, y + 1] = Tile.EdgeTile;
                    if (y - 1 >= 0 && grid[x, y - 1] == Tile.Wall) grid[x, y - 1] = Tile.EdgeTile;
                    if (x + 1 < width && grid[x + 1, y] == Tile.Wall) grid[x + 1, y] = Tile.EdgeTile;
                    if (x - 1 >= 0 && grid[x - 1, y] == Tile.Wall) grid[x - 1, y] = Tile.EdgeTile;
                }
            }
        }
    }

    // Identifies and marks isolated areas (islands) in the grid
    public void IdentifyIslands() {
        bool[,] visited = new bool[width, height];
        
        // Perform a flood fill from the grid borders to identify islands
        for (int x = 0; x < width; x++) {
            FloodFillIsland(new Vector2Int(x, 0), ref visited);
            FloodFillIsland(new Vector2Int(x, height - 1), ref visited);
        }

        for (int y = 0; y < height; y++) {
            FloodFillIsland(new Vector2Int(0, y), ref visited);
            FloodFillIsland(new Vector2Int(width - 1, y), ref visited);
        }

        // Mark unvisited tiles as islands
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (!visited[x, y] && (grid[x, y] == Tile.Wall || IsEdgeTile(x, y))) {
                    grid[x, y] = Tile.Island;
                }
            }
        }
    }

    // Flood fill algorithm to identify islands
    void FloodFillIsland(Vector2Int start, ref bool[,] visited) {
        if (start.x < 0 || start.x >= width || start.y < 0 || start.y >= height) return;

        Queue<Vector2Int> toVisit = new Queue<Vector2Int>();
        toVisit.Enqueue(start);

        while (toVisit.Count > 0) {
            Vector2Int current = toVisit.Dequeue();

            // Continue if the current tile is outside the grid or already visited
            if (current.x < 0 || current.x >= width || current.y < 0 || current.y >= height) continue;
            if (visited[current.x, current.y] || grid[current.x, current.y] == Tile.Floor || grid[current.x, current.y] == Tile.Island) continue;

            // Mark the tile as visited and add its neighbors to the queue
            visited[current.x, current.y] = true;
            toVisit.Enqueue(new Vector2Int(current.x + 1, current.y));
            toVisit.Enqueue(new Vector2Int(current.x - 1, current.y));
            toVisit.Enqueue(new Vector2Int(current.x, current.y + 1));
            toVisit.Enqueue(new Vector2Int(current.x, current.y - 1));
        }
    }
}