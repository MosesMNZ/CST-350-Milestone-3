using System.ComponentModel.DataAnnotations;

namespace CST_350_Milestone.Models
{
    public class CellModel
    {
        // Cell state: 0=hidden, 1=revealed, 2=flagged, 3=mine
        public int Id { get; set; }
        public int CellState { get; set; }
        public string CellImage { get; set; }

        public CellModel(int id, int cellState, string cellImage)
        {
            Id = id;
            CellState = cellState;
            CellImage = cellImage;
        }

        // MILESTONE 2: needed by the game service to place mines, count
        // adjacent mines, and drive flood-reveal / win-loss logic.
        public bool IsMine { get; set; }
        public int AdjacentMines { get; set; }

        public CellModel() { }
    }
}
