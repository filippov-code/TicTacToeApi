# TicTacToeApi - API для игры в крестики нолики
## Состав
+ TicTacToeRESTApi - WebApi
+ TicTacToe - библиотека с логикой для работы WebApi
+ JsClient - пример простейшего клиента на JavaScript
## Стек
+ С#, .NET 6
+ SQLite, EntityFramework Core
## Описание API
+ TicTacToe/CreatePlayer/{nickname:string} - Создаёт нового игрока
+ TicTacToe/GetPlayer/{playerId:Guid} - Возвращает игрока
+ TicTacToe/GetAllPlayers - Возвращает всех игроков
+ TicTacToe/GetAllAvailableGames - Возвращает все доступные игры
+ TicTacToe/CreateGame/{ownerId:Guid} - Создает игру и подключает к ней владельца
+ TicTacToe/ConnectToGame/{gameId:Guid}{playerId:Guid} - Подключает игрока к указанной игре
+ TicTacToe/GetGame/{gameId:Guid} - Возвращает игру
+ TicTacToe/MakeMove/{gameId:Guid}{playerId:Guid}{x:int}{y:int} - Делает ход в игре от указанного игрока в указанной позиции
+ TicTacToe/DisconnectFromGame/{gameId:Guid}{playerId:Guid} - Отключает игрока от указанной игры
+ TicTacToe/RestartGame/{gameId:Guid} - Перезапускает игру (сбрасывает прогресс)
+ TicTacToe/GetXOEmptySigns - Возвращает символы которые используются для обозначения X, O и незанятой позиции
![](https://github.com/filippov-code/TicTacToeApi/blob/master/screenshots/apis.png)
![](https://github.com/filippov-code/TicTacToeApi/blob/master/screenshots/gameplay.png)

