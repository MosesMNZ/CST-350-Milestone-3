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
        /// Initialize a new game board with mines placed
        /// </summary>
        /// <param name="boardSize">8, 10, or 12 for board dimensions</param>
        /// <param name="difficulty">Easy, Medium, or Hard</param>
        /// <returns>Game state object with initialized cells</returns>
        GameState InitializeGame(int boardSize, string difficulty);

        /// <summary>
        /// Reveal a cell and handle game logic
        /// </summary>
        /// <param name="cellId">Cell index (0-based)</param>
        /// <param name="gameState">Current game state</param>
        /// <returns>Updated game state with revealed cell</returns>
        GameState RevealCell(int cellId, GameState gameState);

        /// <summary>
        /// Toggle flag on/off for a cell
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
    }
}
