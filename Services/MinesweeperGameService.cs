using CST_350_Milestone.Models;

namespace CST_350_Milestone.Services
{
    /// <summary>
    /// MILESTONE 3: Minesweeper Game Service
    /// Encapsulates all game rules, mine placement, and cell reveal logic in a
    /// back-end business service (per the Milestone 3 requirement), so the
    /// controller stays a thin HTTP/session layer.
    ///
    /// The actual game rules here are ported from the Milestone 2 BoardModel
    /// implementation (mine placement with a safe first click, BFS flood
    /// reveal of empty cells, adjacent-mine counting, win/loss detection, and
    /// score calculation) and adapted to operate on the GameState/CellModel
    /// shape used by the Milestone 3 AJAX/service architecture.
    /// </summary>
    public class MinesweeperGameService : IMinesweeperGameService
    {
        public GameState InitializeGame(int boardSize, string difficulty)
        {
            var gameState = new GameState
            {
                BoardSize = boardSize,
                Difficulty = difficulty,
                MineCount = CalculateMineCount(boardSize, difficulty),
                FirstClick = true,
                StartTime = DateTime.UtcNow
            };

            for (int i = 0; i < boardSize * boardSize; i++)
            {
                gameState.Cells.Add(new CellModel(i, 0, "blue_button.png"));
            }

            return gameState;
        }

        public GameState RevealCell(int cellId, GameState gameState)
        {
            if (cellId < 0 || cellId >= gameState.Cells.Count)
                return gameState;

            // Game is already over - ignore further reveals
            if (gameState.IsGameWon || gameState.IsGameLost)
                return gameState;

            var cell = gameState.Cells[cellId];

            // Flagged or already-revealed cells cannot be revealed
            if (cell.CellState == 2 || cell.CellState == 1)
                return gameState;

            // Safe first click: place mines only after we know which cell was clicked
            if (gameState.FirstClick)
            {
                PlaceMines(gameState, cellId);
                gameState.StartTime = DateTime.UtcNow;
                gameState.FirstClick = false;
            }

            if (cell.IsMine)
            {
                // Capture elapsed time BEFORE flipping IsGameLost - GetElapsedSeconds()
                // short-circuits to FinalElapsedSeconds once the game is marked
                // over, so computing it after setting the flag always returned 0.
                gameState.FinalElapsedSeconds = (int)(DateTime.UtcNow - gameState.StartTime).TotalSeconds;
                gameState.IsGameLost = true;

                foreach (var mineCell in gameState.Cells.Where(c => c.IsMine))
                {
                    mineCell.CellState = 3;
                    mineCell.CellImage = "red_button.png";
                }

                return gameState;
            }

            FloodReveal(gameState, cellId);

            if (gameState.Cells.All(c => c.IsMine || c.CellState == 1))
            {
                // Same ordering fix as the loss case above.
                gameState.FinalElapsedSeconds = (int)(DateTime.UtcNow - gameState.StartTime).TotalSeconds;
                gameState.IsGameWon = true;
            }

            return gameState;
        }

        public GameState ToggleFlag(int cellId, GameState gameState)
        {
            if (cellId < 0 || cellId >= gameState.Cells.Count)
                return gameState;

            var cell = gameState.Cells[cellId];

            // Revealed cells cannot be flagged
            if (cell.CellState == 1)
                return gameState;

            if (cell.CellState == 2)
            {
                // Flagged -> remove flag
                cell.CellState = 0;
                cell.CellImage = "blue_button.png";
            }
            else
            {
                // Hidden -> flag
                cell.CellState = 2;
                cell.CellImage = "red_button.png";
            }

            return gameState;
        }

        public CellModel GetCell(int cellId, GameState gameState)
        {
            if (cellId < 0 || cellId >= gameState.Cells.Count)
                return null;

            return gameState.Cells[cellId];
        }

        public string CheckGameStatus(GameState gameState)
        {
            if (gameState.IsGameLost) return "Loss";
            if (gameState.IsGameWon) return "Win";
            return "InProgress";
        }

        public int GetFlagCount(GameState gameState)
        {
            return gameState.Cells.Count(c => c.CellState == 2);
        }

        public int GetMineCount(GameState gameState)
        {
            return gameState.MineCount;
        }

        public int GetElapsedSeconds(GameState gameState)
        {
            if (gameState.FirstClick)
                return 0;

            if (gameState.IsGameWon || gameState.IsGameLost)
                return gameState.FinalElapsedSeconds;

            return (int)(DateTime.UtcNow - gameState.StartTime).TotalSeconds;
        }

        // Points deducted for every flag left placed on the board at the
        // moment of a win. Flags aren't required to win (mines are avoided
        // just by never clicking them), so this rewards players who can spot
        // and clear mines without leaning on flags to track them. Exposed via
        // the interface so the game-over UI can display the same number.
        public int FlagPenalty => 10;

        public int CalculateScore(GameState gameState)
        {
            double multiplier = gameState.Difficulty switch
            {
                "Easy" => 1.0,
                "Medium" => 1.5,
                "Hard" => 2.0,
                _ => 1.0
            };

            int flagsUsed = GetFlagCount(gameState);

            int rawScore = (int)(gameState.MineCount * 100 * multiplier)
                           - gameState.FinalElapsedSeconds * 5
                           - flagsUsed * FlagPenalty;

            return Math.Max(0, rawScore);
        }

        public int GetRevealedCellCount(GameState gameState)
        {
            return gameState.Cells.Count(c => c.CellState == 1);
        }

        // ----- private helpers (ported from Milestone 2 BoardModel) -----

        private static int CalculateMineCount(int size, string difficulty) => difficulty switch
        {
            "Easy" => (int)Math.Round(size * size * 0.12),
            "Medium" => (int)Math.Round(size * size * 0.17),
            "Hard" => (int)Math.Round(size * size * 0.23),
            _ => (int)Math.Round(size * size * 0.15)
        };

        private void PlaceMines(GameState gameState, int safeCell)
        {
            var rand = new Random();
            int placed = 0;
            var cells = gameState.Cells;

            while (placed < gameState.MineCount)
            {
                int idx = rand.Next(cells.Count);
                if (!cells[idx].IsMine && idx != safeCell)
                {
                    cells[idx].IsMine = true;
                    placed++;
                }
            }

            for (int i = 0; i < cells.Count; i++)
                if (!cells[i].IsMine)
                    cells[i].AdjacentMines = CountAdjacentMines(gameState, i);
        }

        private int CountAdjacentMines(GameState gameState, int index)
        {
            int count = 0;
            foreach (int n in GetNeighbors(gameState.BoardSize, index))
                if (gameState.Cells[n].IsMine) count++;
            return count;
        }

        private List<int> GetNeighbors(int size, int index)
        {
            var neighbors = new List<int>();
            int row = index / size, col = index % size;
            for (int dr = -1; dr <= 1; dr++)
                for (int dc = -1; dc <= 1; dc++)
                {
                    if (dr == 0 && dc == 0) continue;
                    int nr = row + dr, nc = col + dc;
                    if (nr >= 0 && nr < size && nc >= 0 && nc < size)
                        neighbors.Add(nr * size + nc);
                }
            return neighbors;
        }

        private void FloodReveal(GameState gameState, int index)
        {
            var queue = new Queue<int>();
            var visited = new HashSet<int>();
            queue.Enqueue(index);

            while (queue.Count > 0)
            {
                int idx = queue.Dequeue();
                if (!visited.Add(idx)) continue;

                var cell = gameState.Cells[idx];
                if (cell.CellState != 0 || cell.IsMine) continue;

                cell.CellState = 1;
                cell.CellImage = cell.AdjacentMines == 0 ? "green_button.png" : "orange_button.png";

                if (cell.AdjacentMines == 0)
                    foreach (int n in GetNeighbors(gameState.BoardSize, idx))
                        if (!visited.Contains(n)) queue.Enqueue(n);
            }
        }
    }
}
