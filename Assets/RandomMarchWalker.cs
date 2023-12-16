using UnityEngine;
using System.Collections.Generic;

// The RandomMarchWalker class is responsible for generating random paths on a grid.
// It simulates the movement of a 'walker' that randomly moves across a grid, creating paths (floors) and modifying the grid layout.
public class RandomMarchWalker {
    public Vector2Int currentPosition; // Current position of the walker on the grid
    private int walkSteps; // Number of steps the walker will take
    private int maxStepLength; // Maximum length of a single step
    private float backtrackProbability; // Probability of the walker to backtrack on its path
    private int width; // Width of the grid
    private int height; // Height of the grid

    // Constructor for initializing the walker with specific parameters.
    public RandomMarchWalker(int walkSteps, int maxStepLength, float backtrackProbability, int width, int height) {
        // Initialize the walker at the center of the grid
        this.currentPosition = new Vector2Int(width / 2, height / 2);
        this.walkSteps = walkSteps;
        this.maxStepLength = maxStepLength;
        this.backtrackProbability = backtrackProbability;
        this.width = width;
        this.height = height;
    }

    // Method to start the walking process on the given grid.
    public void Walk(LevelGrid grid) {
        for (int i = 0; i < walkSteps; i++) {
            Vector2Int direction = ChooseRandomDirection(); // Choose a random direction to move
            int stepLength = Random.Range(1, maxStepLength + 1); // Determine the length of the step

            for (int j = 0; j < stepLength; j++) {
                // Decide whether to backtrack or move forward
                if (Random.value < backtrackProbability) {
                    this.currentPosition -= direction;
                } else {
                    this.currentPosition += direction;
                }

                // Ensure the walker stays within the bounds of the grid
                currentPosition.x = Mathf.Clamp(currentPosition.x, 1, width - 2);
                currentPosition.y = Mathf.Clamp(currentPosition.y, 1, height - 2);

                // Update the grid based on the walker's new position
                grid.SetTile(currentPosition.x, currentPosition.y, LevelGrid.Tile.Floor);
                
                // Create a wider corridor based on the direction of the walker
                CreateWideCorridor(currentPosition, direction, grid);
            }
        }
        // After walking is done, detect edges and islands on the grid
        grid.DetectEdges();
        grid.IdentifyIslands();
    }

    // Method to create wider corridors based on the current direction of the walker
    private void CreateWideCorridor(Vector2Int position, Vector2Int direction, LevelGrid grid) {
        // Expand the corridor in the direction perpendicular to the walker's movement
        if (direction.x != 0) { // Horizontal movement
            if (position.y + 1 < height) grid.SetTile(position.x, position.y + 1, LevelGrid.Tile.Floor);
            if (position.y - 1 >= 0) grid.SetTile(position.x, position.y - 1, LevelGrid.Tile.Floor);
        } else if (direction.y != 0) { // Vertical movement
            if (position.x + 1 < width) grid.SetTile(position.x + 1, position.y, LevelGrid.Tile.Floor);
            if (position.x - 1 >= 0) grid.SetTile(position.x - 1, position.y, LevelGrid.Tile.Floor);
        }
    }

    // Method to choose a random direction for the walker to move, including diagonals
    private Vector2Int ChooseRandomDirection() {
        List<Vector2Int> validDirections = new List<Vector2Int> {
            // Possible movement directions
            new Vector2Int(0, 1),    // Up
            new Vector2Int(0, -1),   // Down
            new Vector2Int(-1, 0),   // Left
            new Vector2Int(1, 0),    // Right
            new Vector2Int(1, 1),    // Up-Right
            new Vector2Int(-1, 1),   // Up-Left
            new Vector2Int(1, -1),   // Down-Right
            new Vector2Int(-1, -1)   // Down-Left
        };

        // Remove directions that would take the walker out of the grid bounds
        if (currentPosition.x <= 0) {
            validDirections.RemoveAll(dir => dir.x == -1); // Remove left and diagonals going left
        }
        if (currentPosition.x >= width - 1) {
            validDirections.RemoveAll(dir => dir.x == 1); // Remove right and diagonals going right
        }
        if (currentPosition.y <= 0) {
            validDirections.RemoveAll(dir => dir.y == -1); // Remove down and diagonals going down
        }
        if (currentPosition.y >= height - 1) {
            validDirections.RemoveAll(dir => dir.y == 1); // Remove up and diagonals going up
        }

        // If no valid directions are left, return zero vector (no movement)
        if (validDirections.Count == 0) {
            return Vector2Int.zero;
        }

        // Return a random valid direction
        return validDirections[Random.Range(0, validDirections.Count)];
    }
}