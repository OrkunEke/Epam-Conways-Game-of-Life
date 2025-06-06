namespace GameOfLife;

/// <summary>
/// Provides extension methods to simulate and visualize the Game of Life.
/// </summary>
public static class GameOfLifeExtension
{
    /// <summary>
    /// Simulates the Game of Life for a given number of generations using the sequential version.
    /// Prints each generation to the provided writer, using given characters for alive and dead cells.
    /// </summary>
    public static void Simulate(this GameOfLifeSequentialVersion? game, int generations, TextWriter? writer, char aliveCell, char deadCell)
    {
        ArgumentNullException.ThrowIfNull(game);
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(generations);

        // Print initial state
        // Printing the gen 0 first because its ready to print
        writer.WriteLine($"Generation: {game.Generation}");
        var grid = game.CurrentGeneration;
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                writer.Write(grid[i, j] ? aliveCell : deadCell);
            }

            writer.WriteLine();
        }

        writer.WriteLine();

        // After gen 0 it evolves print each new generation
        for (int gen = 1; gen < generations; gen++)
        {
            game.NextGeneration();
            writer.WriteLine($"Generation: {game.Generation}");
            grid = game.CurrentGeneration;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    writer.Write(grid[i, j] ? aliveCell : deadCell); // Write the aliveCell character if the current cell is alive (true); otherwise write the deadCell character. Given by the user
                }

                writer.WriteLine();
            }

            writer.WriteLine();
        }
    }

    /// <summary>
    /// Asynchronously simulates the Game of Life for a given number of generations using the parallel version.
    /// Prints each generation to the writer, using given characters for alive and dead cells.
    /// </summary>
    public static async Task SimulateAsync(this GameOfLifeParallelVersion? game, int generations, TextWriter? writer, char aliveCell, char deadCell)
    {
        ArgumentNullException.ThrowIfNull(game);
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(generations);

        // Print initial state
        await writer.WriteLineAsync($"Generation: {game.Generation}");
        var grid = game.CurrentGeneration;
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                await writer.WriteAsync(grid[i, j] ? aliveCell : deadCell);
            }

            await writer.WriteLineAsync(string.Empty);
        }

        await writer.WriteLineAsync(string.Empty);

        // Evolve and print each new generation
        for (int gen = 1; gen < generations; gen++)
        {
            game.NextGeneration();
            await writer.WriteLineAsync($"Generation: {game.Generation}");
            grid = game.CurrentGeneration;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    await writer.WriteAsync(grid[i, j] ? aliveCell : deadCell);
                }

                await writer.WriteLineAsync(string.Empty);
            }

            await writer.WriteLineAsync(string.Empty);
        }
    }
}
