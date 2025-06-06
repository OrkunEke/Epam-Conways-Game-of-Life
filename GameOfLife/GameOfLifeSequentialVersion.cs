namespace GameOfLife;

/// <summary>
/// Sequential version of Conway's Game of Life.
/// Evolves the game step-by-step based on the standard rules.
/// </summary>
public sealed class GameOfLifeSequentialVersion
{
    // --- Fields ---
    private readonly int rows;          // Number of rows in the grid
    private readonly int columns;       // Number of columns in the grid
    private readonly bool[,] initialGrid;        // Copy of the initial state (for restart)
    private bool[,] grid;               // Current state of the game grid

    /// <summary>
    /// Initializes a new instance of the <see cref="GameOfLifeSequentialVersion"/> class.
    /// Creates a random game grid with the given size (alive/dead cells chosen randomly).
    /// </summary>
    public GameOfLifeSequentialVersion(int rows, int columns)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(rows);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(columns);

        this.rows = rows;
        this.columns = columns;

        this.grid = new bool[rows, columns];
        var random = new Random();

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                this.grid[i, j] = random.Next(2) == 1;
            }
        }

        // Deep copy for restart functionality
        this.initialGrid = new bool[rows, columns];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                this.initialGrid[i, j] = this.grid[i, j];
            }
        }
    }

    public GameOfLifeSequentialVersion(bool[,] grid)
    {
        ArgumentNullException.ThrowIfNull(grid);

        if (grid.GetLength(0) == 0 || grid.GetLength(1) == 0)
        {
            throw new ArgumentException("Grid must have at least one row and one column.", nameof(grid));
        }

        this.grid = grid;
        this.rows = grid.GetLength(0);
        this.columns = grid.GetLength(1);

        // Deep copy for restart functionality
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
    /// Gets the current generation number.
    /// </summary>
    public int Generation { get; private set; }

    /// <summary>
    /// Resets the game to the initial state and sets generation count to zero.
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
    /// Advances the game to the next generation using Conway's rules.
    /// </summary>
    public void NextGeneration()
    {
        var newGrid = new bool[this.rows, this.columns];

        for (int i = 0; i < this.rows; i++)
        {
            for (int j = 0; j < this.columns; j++)
            {
                var neighbors = this.CountAliveNeighbors(i, j);
                if (this.grid[i, j])
                {
                    // Any live cell with 2 or 3 neighbors survives, otherwise it dies.
                    if (neighbors == 2 || neighbors == 3)
                    {
                        newGrid[i, j] = true;
                    }
                    else
                    {
                        newGrid[i, j] = false;
                    }
                }
                else
                {
                    // Any dead cell with exactly 3 live neighbors becomes alive.
                    if (neighbors == 3)
                    {
                        newGrid[i, j] = true;
                    }
                    else
                    {
                        newGrid[i, j] = false;
                    }
                }
            }
        }

        this.grid = newGrid;
        this.Generation += 1;
    }

    /// <summary>
    /// Counts the number of alive neighbors for a given cell.
    /// Considers up to 8 adjacent cells (skips out-of-bounds and self).
    /// </summary>
    private int CountAliveNeighbors(int row, int column)
    {
        int aliveCount = 0;

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
