namespace TicTacToeRESTApi.Models
{
    public class Player
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; }
        public Guid? CurrentGameId { get; set; }
    }
}
