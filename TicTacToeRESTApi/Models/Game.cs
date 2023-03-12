using System.ComponentModel.DataAnnotations.Schema;

namespace TicTacToeRESTApi.Models
{
    public class Game
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public Guid? GuestId { get; set; }
        public Statuses Status { get; set; }
        public Guid? WinnerId { get; set; }
        public string Field { get; set; }
        public bool OwnerTurn { get; set; }

        public enum Statuses
        {
            WaitingGuest,
            Active,
            Finished
        }
    }
}
