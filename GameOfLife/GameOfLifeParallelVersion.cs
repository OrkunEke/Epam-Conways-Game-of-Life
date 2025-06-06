namespace GameOfLife;

/// <summary>
/// Parallel version of Conway's Game of Life.
/// Uses parallel loops to efficiently simulate large grids.
/// </summary>
public sealed class GameOfLifeParallelVersion
{
    // --- Fields ---
    private readonly int rows;          // Number of rows in the grid
    private readonly int columns;       // Number of columns in the grid
    private readonly bool[,] initialGrid;        // Copy of the initial state (for restart)
    private bool[,] grid;               // Current state of the game grid

    // --- Constructors ---

    /// <summary>
    /// Initializes a new instance of the <see cref="GameOfLifeParallelVersion"/> class.
    /// Creates a random grid of the given size (alive/dead cells are randomly assigned).
    /// </summary>
    public GameOfLifeParallelVersion(int rows, int columns)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(rows);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(columns);

        this.rows = rows;
        this.columns = columns;

        // Create random initial grid
        this.grid = new bool[rows, columns];
        var random = new Random();

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                this.grid[i, j] = random.Next(2) == 1;  // return 0 or 1
            }
        }

        // Save a copy as initialGrid for Restart()
        this.initialGrid = new bool[rows, columns];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                this.initialGrid[i, j] = this.grid[i, j];
            }
        }
    }

    public GameOfLifeParallelVersion(bool[,] grid)
    {
        ArgumentNullException.ThrowIfNull(grid);

        if (grid.GetLength(0) == 0 || grid.GetLength(1) == 0)
        {
            throw new ArgumentException("Grid must have at least one row and one column.", nameof(grid));
        }

        this.rows = grid.GetLength(0);
        this.columns = grid.GetLength(1);
        this.grid = grid;

        // Deep copy to initialGrid
        this.initialGrid = new bool[this.rows, this.columns];
        for (int i = 0; i < this.rows; i++)
        {
            for (int j = 0; j < this.columns; j++)
            {
                this.initialGrid[i, j] = this.grid[i, j];
            }
        }
    }

    /// <summary>
    /// Gets a deep copy of the current game grid.
    /// </summary>
    public bool[,] CurrentGeneration
    {
        get
        {
            var copy = new bool[this.rows, this.columns];
            for (int i = 0; i < this.rows; i++)
            {
                for (int j = 0; j < this.columns; j++)
                {
                    copy[i, j] = this.grid[i, j];
                }
            }

            return copy;
        }
    }

    /// <summary>
    /// Gets the current generation number (increments after each NextGeneration call).
    /// </summary>
    public int Generation { get; private set; }

    /// <summary>
    /// Resets the game to the initial state (and resets the generation count).
    /// </summary>
    public void Restart()
    {
        this.Generation = 0;
        for (int i = 0; i < this.rows; i++)
        {
            for (int j = 0; j < this.columns; j++)
            {
                this.grid[i, j] = this.initialGrid[i, j];
            }
        }
    }

    /// <summary>
    /// Advances the game to the next generation using parallel processing.
    /// Applies Conway's rules in parallel for better performance on large grids.
    /// </summary>
    public void NextGeneration()
    {
        var newGrid = new bool[this.rows, this.columns];

        // Parallelize across rows for performance boost on large grids
        Parallel.For(0, this.rows, i =>
        {
            for (int j = 0; j < this.columns; j++)
            {
                int neighbors = this.CountAliveNeighbors(i, j);

                // Apply Conway's Game of Life rules
                if (this.grid[i, j])
                {
                    newGrid[i, j] = neighbors == 2 || neighbors == 3;
                }
                else
                {
                    newGrid[i, j] = neighbors == 3;
                }
            }
        });

        this.grid = newGrid;
        this.Generation++;
    }

    /// <summary>
    /// Counts the number of alive (true) neighbors around a given cell.
    /// Considers up to 8 adjacent cells (skips out-of-bounds and self).
    /// </summary>
    private int CountAliveNeighbors(int row, int column)
    {
        int aliveCount = 0;

        // Check all adjacent cells (including diagonals)
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int neighborRow = row + i;
                int neighborCol = column + j;

                // Skip the cell itself
                if (i == 0 && j == 0)
                {
                    continue;
                }

                // Count if within grid and alive
                if (neighborRow >= 0 && neighborRow < this.rows &&
                    neighborCol >= 0 && neighborCol < this.columns &&
                    this.grid[neighborRow, neighborCol])
                {
                    aliveCount++;
                }
            }
        }

        return aliveCount;
    }
}
