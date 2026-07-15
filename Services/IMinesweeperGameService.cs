using CST_350_Milestone.Models;

namespace CST_350_Milestone.Services
{
    /// <summary>
    /// MILESTONE 3: Minesweeper Game Service Interface
    /// Defines contract for game business logic (separate from controller)
    /// Follows CST-350 business service pattern
    /// </summary>
    public interface IMinesweeperGameService
    {
        /// <summary>
        /// Initialize a new game board. Mines are placed lazily on the first
        /// RevealCell call so that the player's first click is always safe
        /// (ported from the Milestone 2 BoardModel implementation).
        /// </summary>
        /// <param name="boardSize">8, 10, or 12 for board dimensions</param>
        /// <param name="difficulty">Easy, Medium, or Hard</param>
        /// <returns>Game state object with initialized (unmined) cells</returns>
        GameState InitializeGame(int boardSize, string difficulty);

        /// <summary>
        /// Reveal a cell and handle game logic (mine placement on first click,
        /// flood-fill reveal of empty neighbors, win/loss detection).
        /// </summary>
        /// <param name="cellId">Cell index (0-based)</param>
        /// <param name="gameState">Current game state</param>
        /// <returns>Updated game state with revealed cell(s)</returns>
        GameState RevealCell(int cellId, GameState gameState);

        /// <summary>
        /// Toggle flag on/off for a cell. Flagged cells cannot be revealed by
        /// a left click until the flag is removed. Revealed cells cannot be flagged.
        /// </summary>
        /// <param name="cellId">Cell index (0-based)</param>
        /// <param name="gameState">Current game state</param>
        /// <returns>Updated game state with flagged cell</returns>
        GameState ToggleFlag(int cellId, GameState gameState);

        /// <summary>
        /// Get a specific cell from the board
        /// </summary>
        /// <param name="cellId">Cell index (0-based)</param>
        /// <param name="gameState">Current game state</param>
        /// <returns>CellModel for requested cell</returns>
        CellModel GetCell(int cellId, GameState gameState);

        /// <summary>
        /// Check current game status (in progress, won, lost)
        /// </summary>
        /// <param name="gameState">Current game state</param>
        /// <returns>Game status string: "InProgress", "Win", or "Loss"</returns>
        string CheckGameStatus(GameState gameState);

        /// <summary>
        /// Get count of flagged cells
        /// </summary>
        /// <param name="gameState">Current game state</param>
        /// <returns>Number of flagged cells</returns>
        int GetFlagCount(GameState gameState);

        /// <summary>
        /// Get total mine count for the board
        /// </summary>
        /// <param name="gameState">Current game state</param>
        /// <returns>Total number of mines on board</returns>
        int GetMineCount(GameState gameState);

        /// <summary>
        /// Number of seconds elapsed since the game started (or since it ended,
        /// once the game is over). Mirrors BoardModel.GetElapsedSeconds from
        /// Milestone 2.
        /// </summary>
        int GetElapsedSeconds(GameState gameState);

        /// <summary>
        /// Compute the final score for a completed game based on total mines,
        /// difficulty, elapsed time, and flags used (see FlagPenalty). Ported
        /// from BoardModel.CalculateScore.
        /// </summary>
        int CalculateScore(GameState gameState);

        /// <summary>
        /// Points deducted from the final score for every flag still placed
        /// on the board when a game is won. Exposed so the UI can show the
        /// same number used in CalculateScore, instead of hardcoding it.
        /// </summary>
        int FlagPenalty { get; }

        /// <summary>
        /// Count of cells currently revealed (CellState == 1). Used on the
        /// Loss screen to show how far the player got.
        /// </summary>
        int GetRevealedCellCount(GameState gameState);
    }

    /// <summary>
    /// Game state container - holds board state and game info
    /// Serializable for session storage
    /// </summary>
    public class GameState
    {
        public List<CellModel> Cells { get; set; } = new List<CellModel>();
        public int BoardSize { get; set; }
        public string Difficulty { get; set; }
        public int MineCount { get; set; }
        public bool IsGameWon { get; set; }
        public bool IsGameLost { get; set; }
        public DateTime StartTime { get; set; }

        // MILESTONE 2 (ported from BoardModel): mines aren't placed until the
        // player's first reveal, so the first click can never be a mine.
        public bool FirstClick { get; set; } = true;

        // Elapsed seconds captured the moment the game ends (win or loss), so
        // the score/summary screens show a frozen time instead of a moving one.
        public int FinalElapsedSeconds { get; set; }
    }
}
