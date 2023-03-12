using Microsoft.AspNetCore.Mvc;
using TicTacToeRESTApi.DataBase;
using TicTacToeRESTApi.Models;
using TicTacToe;

namespace TicTacToeRESTApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TicTacToeController : ControllerBase
    {
        private ApplicationDbContext _dbContext;

        public TicTacToeController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        /// <summary>
        /// Создаёт нового игрока
        /// </summary>
        /// <param name="nickname"></param>
        /// <returns>Новый игрок</returns>
        [HttpPost]
        public async Task<IActionResult> CreatePlayer(string nickname)
        {
            if (string.IsNullOrEmpty(nickname))
                return BadRequest();

            Player newPlayer = new Player { Nickname = nickname };
            await _dbContext.Players.AddAsync(newPlayer);
            await _dbContext.SaveChangesAsync();
            return Ok(newPlayer);
        }
        /// <summary>
        /// Возвращает игрока по Id
        /// </summary>
        /// <param name="playerId">Id игрока</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetPlayer(Guid playerId)
        {
            if (playerId == Guid.Empty)
                return BadRequest("Id cannot be empty.");
            Player player = await _dbContext.Players.FindAsync(playerId);
            if (player == null)
                return BadRequest("Player not found.");

            return Ok(player);
        }
        /// <summary>
        /// Возвращает список всех игроков
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<Player> GetAllPlayers()
        {
            return _dbContext.Players.ToArray();
        }
        /// <summary>
        /// Возвращает все доступные игры
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<Game> GetAllAvailableGames()
        {
            return _dbContext.Games
                .Where(x => x.Status == Game.Statuses.WaitingGuest && x.GuestId == null)
                .ToArray();
        }
        /// <summary>
        /// Создает новую игру и подключает владельца к ней
        /// </summary>
        /// <param name="ownerId">Id владельца игры</param>
        /// <returns>Созданная игра</returns>
        [HttpPost]
        public async Task<IActionResult> CreateGame(Guid ownerId)
        {
            if (ownerId == Guid.Empty)
                return BadRequest("Id cannot be empty");
            Player owner = _dbContext.Players.Find(ownerId);
            if (owner == null)
                return BadRequest("Player not found.");

            Game newGame = new Game
            {
                Id = Guid.NewGuid(),
                OwnerId = owner.Id,
                Status = Game.Statuses.WaitingGuest,
                Field = Instruments.GetEmptyField()
            };
            owner.CurrentGameId = newGame.Id;
            await _dbContext.Games.AddAsync(newGame);
            _dbContext.Players.Update(owner);
            await _dbContext.SaveChangesAsync();

            return Ok(newGame);
        }
        /// <summary>
        /// Присоединяет игрока к игре
        /// </summary>
        /// <param name="gameId">Id игры</param>
        /// <param name="playerId">Id подключаемого игрока</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ConnectToGame(Guid gameId, Guid playerId)
        {
            if (gameId == Guid.Empty || playerId == Guid.Empty)
                return BadRequest("Id cannot be empty.");
            Game game = await _dbContext.Games.FindAsync(gameId);
            if (game == null)
                return BadRequest("Game not found.");
            Player player = await _dbContext.Players.FindAsync(playerId);
            if (player == null)
                return BadRequest("Player not found.");
            if (player.CurrentGameId != null)
                return BadRequest("The player is already connected to another game");
            if (player.Id == game.OwnerId || player.Id == game.GuestId)
                return BadRequest("The player is already connected to this game");

            game.GuestId = player.Id;
            game.Status = Game.Statuses.Active;
            player.CurrentGameId = gameId;
            _dbContext.Games.Update(game);
            _dbContext.Players.Update(player);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
        /// <summary>
        /// Возвращает игру с заданным Id
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetGame(Guid gameId)
        {
            if (gameId == Guid.Empty)
                return BadRequest("Id cannot be empty.");
            Game game = await _dbContext.Games.FindAsync(gameId);
            if (game == null)
                return BadRequest("Game not found.");
            
            return Ok(game);
        }
        /// <summary>
        /// Делает ход от лица указанного игрока в указанную позицию игрового поля
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="playerId"></param>
        /// <param name="x">X позиция хода (0-2)</param>
        /// <param name="y">Y позиция хода (0-2)</param>
        /// <returns>Обновленная игра</returns>
        [HttpPost]
        public async Task<IActionResult> MakeMove(Guid gameId, Guid playerId, int x, int y)
        {
            if (gameId == Guid.Empty || playerId == Guid.Empty ||
                x < 0 || x > 2 ||  y < 0 || y > 2)
                return BadRequest("Incorrect parameters");
            Game game = await _dbContext.Games.FindAsync(gameId);
            if (game == null)
                return BadRequest("Game not found.");
            if (playerId != game.OwnerId && playerId != game.GuestId)
                return BadRequest("There is no such player in this game.");
            Player player = _dbContext.Players.Find(playerId);
            if (player == null)
                return BadRequest("Player not found.");
            if (game.Status == Game.Statuses.Finished)
                return BadRequest("The game has already finished.");
            if (game.OwnerTurn && player.Id != game.OwnerId ||
                !game.OwnerTurn && player.Id == game.OwnerId)
                return BadRequest("It's not your turn now.");
            char[,] fieldTable = Instruments.ConvertStringToFieldTable(game.Field);
            if (fieldTable[x, y] != Instruments.EmptySign)
                return BadRequest("This position is occupied.");

            bool playerIsOwner = player.Id == game.OwnerId;
            char sign = playerIsOwner ? Instruments.OSign : Instruments.XSign;
            fieldTable[x, y] = sign;
            game.Field = Instruments.ConvertFieldTableToString(fieldTable);
            if (Logic.ThereIsAWinnerOnField(fieldTable))
            {
                game.Status = Game.Statuses.Finished;
                game.WinnerId = playerId;
            }
            else if (Logic.MoveHaveEndedOnField(fieldTable))
            {
                game.Status = Game.Statuses.Finished;
            }
            else
            {
                game.OwnerTurn = !game.OwnerTurn;
            }
            _dbContext.Games.Update(game);
            await _dbContext.SaveChangesAsync();
            return Ok(game);
        }
        /// <summary>
        /// Отсоединяет игрока от игры
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="playerId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DisconnectFromGame(Guid gameId, Guid playerId)
        {
            if (gameId == Guid.Empty || playerId == Guid.Empty)
                return BadRequest("Id cannot be empty.");
            Game game = await _dbContext.Games.FindAsync(gameId);
            if (game == null)
                return BadRequest("Game not found.");
            if (playerId != game.OwnerId && playerId != game.GuestId)
                return BadRequest("There is no such player in this game.");
            Player player = await _dbContext.Players.FindAsync(playerId);
            if (player == null)
                return BadRequest("Player not found.");

            game.Field = Instruments.GetEmptyField();
            game.Status = Game.Statuses.WaitingGuest;
            game.WinnerId = null;
            game.OwnerTurn = false;
            if (playerId == game.OwnerId) //выходит админ
            {
                if (game.GuestId != null) //гость присутствует
                {
                    player.CurrentGameId = null;
                    game.OwnerId = game.GuestId.Value;
                    game.GuestId = null;
                    _dbContext.Games.Update(game);
                    _dbContext.Players.Update(player);
                }
                else //сейчас только админ
                {
                    player.CurrentGameId = null;
                    _dbContext.Players.Update(player);
                    _dbContext.Games.Remove(game);
                }
            }
            else //выходит гость
            {
                player.CurrentGameId = null;
                game.GuestId = null;
                _dbContext.Games.Update(game);
                _dbContext.Players.Update(player);
            }
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        /// <summary>
        /// Перезапускает игру
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns>Перезапущенная игра</returns>
        [HttpPost]
        public async Task<IActionResult> RestartGame(Guid gameId)
        {
            if (gameId == Guid.Empty)
                return BadRequest("Id cannot be empty.");
            Game game = await _dbContext.Games.FindAsync(gameId);
            if (game == null)
                return BadRequest("Game not found.");

            game.Field = Instruments.GetEmptyField();
            game.Status = Game.Statuses.Active;
            game.WinnerId = null;
            game.OwnerTurn = false;
            _dbContext.Games.Update(game);
            await _dbContext.SaveChangesAsync();
            return Ok(game);
        }
        /// <summary>
        /// Возвращает символы обозначающие X, O, Пустое значение
        /// </summary>
        /// <returns>символ X, символ O, символ пустого значения</returns>
        [HttpGet]
        public IActionResult GetXOEmptySigns()
        {
            var result = new
            {
                X = Instruments.XSign,
                O = Instruments.OSign,
                Empty = Instruments.EmptySign
            };
            return Ok(result);
        }
    }
}
