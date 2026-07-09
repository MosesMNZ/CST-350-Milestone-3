using Microsoft.AspNetCore.Mvc;
using CST_350_Milestone.Models;
using CST_350_Milestone.Services;

namespace CST_350_Milestone.Controllers
{
    /// <summary>
    /// MILESTONE 3: Games Controller - Thin controller following MVC pattern
    /// Delegates game logic to IMinesweeperGameService (business logic layer)
    /// Handles HTTP requests, session management, and view rendering only
    /// </summary>
    public class GamesController : Controller
    {
        // Game service injected via dependency injection
        private readonly IMinesweeperGameService _gameService;

        /// <summary>
        /// Constructor with dependency injection
        /// GameService is registered in Program.cs
        /// </summary>
        public GamesController(IMinesweeperGameService gameService)
        {
            _gameService = gameService;
        }
        [HttpGet]
        public IActionResult StartGame()
        {
            // Check authentication
            var userAuthenticated = HttpContext.Session.GetString("UserAuthenticated");
            if (string.IsNullOrEmpty(userAuthenticated) || userAuthenticated != "true")
                return RedirectToAction("Login", "Account");

            return View(new GameSettingsModel());
        }

        [HttpPost]
        public IActionResult StartGame(GameSettingsModel model)
        {
            // Check authentication
            var userAuthenticated = HttpContext.Session.GetString("UserAuthenticated");
            if (string.IsNullOrEmpty(userAuthenticated) || userAuthenticated != "true")
                return RedirectToAction("Login", "Account");

            // Validate form data
            if (!ModelState.IsValid)
                return View(model);

            // TODO MILESTONE 2: Create Board object and place mines
            // TODO MILESTONE 2: Store board in session

            return RedirectToAction("MineSweeperBoard", new { boardSize = model.BoardSize, difficulty = model.Difficulty });
        }

        public IActionResult MineSweeperBoard(string boardSize, string difficulty)
        {
            // Check authentication
            var userAuthenticated = HttpContext.Session.GetString("UserAuthenticated");
            if (string.IsNullOrEmpty(userAuthenticated) || userAuthenticated != "true")
                return RedirectToAction("Login", "Account");

            try
            {
                // Parse board size
                int boardDimension = int.Parse(boardSize);
                
                // TODO MILESTONE 3: Call game service to initialize new game
                // var gameState = _gameService.InitializeGame(boardDimension, difficulty);
                // Store gameState in session as JSON string
                // For now, create placeholder cells

                // Placeholder: Create cells list (to be replaced with game service)
                var cells = new List<CellModel>();
                int totalCells = boardDimension * boardDimension;
                for (int i = 0; i < totalCells; i++)
                {
                    cells.Add(new CellModel(i, 0, "blue_button.png"));
                }

                // TODO MILESTONE 3: When game service is implemented:
                // var flagCount = _gameService.GetFlagCount(gameState);
                // var mineCount = _gameService.GetMineCount(gameState);

                // Initialize ViewBag for stats display (used by _GameStatsPartial.cshtml)
                ViewBag.BoardSize = boardDimension;
                ViewBag.Difficulty = difficulty;
                ViewBag.FlagCount = 0;
                ViewBag.MineCount = 0; // TODO MILESTONE 3: Get from game service
                ViewBag.TimeElapsed = "0:00";

                // Store game start timestamp in session for GetTimestamp endpoint
                // GetTimestamp action will calculate elapsed time from this
                HttpContext.Session.SetString("GameStartTime", DateTime.Now.ToString("O"));

                // Store board size in session for AJAX endpoints
                HttpContext.Session.SetString("BoardSize", boardDimension.ToString());

                // Store cell states in session for toggle functionality
                // Serialize cells list as JSON for session storage
                var cellStates = new Dictionary<int, int>();
                foreach (var cell in cells)
                {
                    cellStates[cell.Id] = cell.CellState; // 0=hidden, 2=flagged, etc.
                }
                var jsonCellStates = System.Text.Json.JsonSerializer.Serialize(cellStates);
                HttpContext.Session.SetString("CellStates", jsonCellStates);

                return View(cells);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MineSweeperBoard error: {ex.Message}");
                return RedirectToAction("StartGame");
            }
        }

        public IActionResult Win()
        {
            // Check authentication
            var userAuthenticated = HttpContext.Session.GetString("UserAuthenticated");
            if (string.IsNullOrEmpty(userAuthenticated) || userAuthenticated != "true")
                return RedirectToAction("Login", "Account");

            // TODO MILESTONE 2: Retrieve game stats from session (time, cells revealed, score)
            // TODO MILESTONE 2: Pass stats to view via ViewBag

            return View();
        }

        public IActionResult Loss()
        {
            // Check authentication
            var userAuthenticated = HttpContext.Session.GetString("UserAuthenticated");
            if (string.IsNullOrEmpty(userAuthenticated) || userAuthenticated != "true")
                return RedirectToAction("Login", "Account");

            // TODO MILESTONE 2: Retrieve game stats from session (time, cells revealed)
            // TODO MILESTONE 2: Pass stats to view via ViewBag

            return View();
        }

        // MILESTONE 3: Reveal cell on left-click
        // Called via AJAX POST, returns updated cell partial view
        [HttpPost]
        public IActionResult RevealCell(int cellId)
        {
            // Check authentication
            var userAuthenticated = HttpContext.Session.GetString("UserAuthenticated");
            if (string.IsNullOrEmpty(userAuthenticated) || userAuthenticated != "true")
                return Unauthorized();

            try
            {
                // Validate cellId is in valid range
                int boardSize = int.TryParse(HttpContext.Session.GetString("BoardSize"), out var size) ? size : 8;
                int maxCellId = (boardSize * boardSize) - 1;
                
                if (cellId < 0 || cellId > maxCellId)
                {
                    return BadRequest(new { error = "Invalid cell ID." });
                }

                // TODO MILESTONE 3: Implement service integration
                // Steps:
                // 1. Retrieve game state from session
                //    var jsonGameState = HttpContext.Session.GetString("GameState");
                //    var gameState = JsonConvert.DeserializeObject<GameState>(jsonGameState);
                //
                // 2. Call game service to reveal cell
                //    var updatedGameState = _gameService.RevealCell(cellId, gameState);
                //
                // 3. Check game status after reveal
                //    string status = _gameService.CheckGameStatus(updatedGameState);
                //    if (status == "Loss") return RedirectToAction("Loss");
                //    if (status == "Win") return RedirectToAction("Win");
                //
                // 4. Store updated game state back in session
                //    var jsonUpdated = JsonConvert.SerializeObject(updatedGameState);
                //    HttpContext.Session.SetString("GameState", jsonUpdated);
                //
                // 5. Get the updated cell to return
                //    var updatedCell = _gameService.GetCell(cellId, updatedGameState);

                // For now, return placeholder cell (this allows frontend testing)
                var updatedCell = new CellModel(cellId, 1, "green_button.png");

                // Pass board size to partial for cell sizing
                ViewBag.BoardSize = boardSize;

                // Return only the updated cell partial (not full board)
                return PartialView("_CellPartial", updatedCell);
            }
            catch (Exception ex)
            {
                // Log error and return 400 Bad Request
                System.Diagnostics.Debug.WriteLine($"RevealCell error: {ex.Message}");
                return BadRequest(new { error = "Error revealing cell." });
            }
        }

        // MILESTONE 3: Toggle flag on right-click
        // Called via AJAX POST, returns updated cell partial view
        [HttpPost]
        public IActionResult ToggleFlag(int cellId)
        {
            // Check authentication
            var userAuthenticated = HttpContext.Session.GetString("UserAuthenticated");
            if (string.IsNullOrEmpty(userAuthenticated) || userAuthenticated != "true")
                return Unauthorized();

            try
            {
                // Validate cellId is in valid range
                int boardSize = int.TryParse(HttpContext.Session.GetString("BoardSize"), out var size) ? size : 8;
                int maxCellId = (boardSize * boardSize) - 1;
                
                if (cellId < 0 || cellId > maxCellId)
                {
                    return BadRequest(new { error = "Invalid cell ID." });
                }

                // Get current cell states from session
                var jsonCellStates = HttpContext.Session.GetString("CellStates");
                var cellStates = string.IsNullOrEmpty(jsonCellStates)
                    ? new Dictionary<int, int>()
                    : System.Text.Json.JsonSerializer.Deserialize<Dictionary<int, int>>(jsonCellStates) 
                      ?? new Dictionary<int, int>();

                // Get current state, default to 0 (hidden) if not found
                int currentState = cellStates.ContainsKey(cellId) ? cellStates[cellId] : 0;

                // Toggle: 0 (hidden) → 2 (flagged), 2 (flagged) → 0 (hidden)
                int newState = (currentState == 2) ? 0 : 2;

                // Update cell state in dictionary
                cellStates[cellId] = newState;

                // Save updated states back to session
                var jsonUpdated = System.Text.Json.JsonSerializer.Serialize(cellStates);
                HttpContext.Session.SetString("CellStates", jsonUpdated);

                // Determine image based on new state
                string cellImage = (newState == 2) ? "red_button.png" : "blue_button.png";

                // Calculate current flag count from all cells
                int flagCount = cellStates.Values.Count(s => s == 2);

                // Return updated cell
                var updatedCell = new CellModel(cellId, newState, cellImage);
                ViewBag.BoardSize = boardSize;
                ViewBag.FlagCount = flagCount;

                return PartialView("_CellPartial", updatedCell);
            }
            catch (Exception ex)
            {
                // Log error and return 400 Bad Request
                System.Diagnostics.Debug.WriteLine($"ToggleFlag error: {ex.Message}");
                return BadRequest(new { error = "Error toggling flag." });
            }
        }

        // MILESTONE 3: Get elapsed game time
        // Called via AJAX GET every 1 second, returns JSON with formatted time
        [HttpGet]
        public IActionResult GetTimestamp()
        {
            // Check authentication
            var userAuthenticated = HttpContext.Session.GetString("UserAuthenticated");
            if (string.IsNullOrEmpty(userAuthenticated) || userAuthenticated != "true")
                return Unauthorized();

            try
            {
                // Retrieve game start timestamp from session
                var startTimeStr = HttpContext.Session.GetString("GameStartTime");
                
                if (string.IsNullOrEmpty(startTimeStr))
                {
                    // Game start time not found in session
                    return Json(new
                    {
                        success = false,
                        timeElapsed = "0:00",
                        elapsedSeconds = 0,
                        message = "Game not started"
                    });
                }

                // Parse start time and calculate elapsed seconds
                var startTime = DateTime.Parse(startTimeStr);
                var elapsedSeconds = (int)(DateTime.Now - startTime).TotalSeconds;
                
                // Format as M:SS or MM:SS
                var minutes = elapsedSeconds / 60;
                var seconds = elapsedSeconds % 60;
                var timeElapsed = $"{minutes}:{seconds:D2}";

                return Json(new
                {
                    success = true,
                    timeElapsed = timeElapsed,
                    elapsedSeconds = elapsedSeconds,
                    message = "Timestamp retrieved"
                });
            }
            catch (Exception ex)
            {
                // Log error and return 400 Bad Request
                System.Diagnostics.Debug.WriteLine($"GetTimestamp error: {ex.Message}");
                return BadRequest(new { error = "Error retrieving timestamp." });
            }
        }
    }
}
