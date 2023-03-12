var protocol = 'https'
var server = 'localhost';
var port = '7164';
var controller = 'TicTacToe';
var url = protocol+'://'+server+':'+port+'/'+controller+'/';

var myPlayer;
var myGame;
var updatingGame = false;
var xSign;
var oSign;
var emptySign;

var myNickname;
var gamePlug;
var gameArea;
var nickname1;
var nickname2;
var fieldCells = [];
var info;
var restartButton;

init();
registration();
showAvailableGames();
updateMyGame();

async function init(){
    var responce = await fetch(url + 'GetXOEmptySigns',{
        method: 'GET',
        headers: {
            'Accept': 'application/json',
        }
    });
    if (responce.ok){
        var result = await responce.json();
        xSign = result.x;
        oSign = result.o;
        emptySign = result.empty;
    }
    else{
        alert('Сервис недоступен. Error: ' + responce.status)
        return;
    }

    myNickname = document.getElementById('myNickname');
    gamePlug = document.getElementById('gamePlug');
    gameArea = document.getElementById('gameArea');
    nickname1 = document.getElementById('nickname1');
    nickname2 = document.getElementById('nickname2');
    let table = document.getElementById('fieldTable');
    for (let y = 0; y < 3; y++){
        for (let x = 0; x < 3; x++){
            table.rows[y].cells[x].onclick = function() { makeMove(x, y) };
            fieldCells.push(table.rows[y].cells[x]);
        }
    }
    info = document.getElementById('gameInfo');
    restartButton = document.getElementById('restartButton');
}
async function registration(){
    let nickname;
    while(true){
        alert('Пройдите регистрацию');
        nickname = prompt("Ваш никнейм: ", '');
        if (nickname == '')
        {
            alert('Никнейм не может быть пустым');
        }
        else break;
    }

    var responce = await fetch(url + 'CreatePlayer?nickname=' + nickname,{
        method: 'POST',
        headers: {
            'Accept': 'application/json',
        }
    });
    if (responce.ok){
        var result = await responce.json();
        //console.log(result);
        myPlayer = result;
        myNickname.innerHTML = 'Вход: ' + myPlayer.nickname;
    }
    else{
        alert("HTTP Error: " + responce.status);
    }

}
async function showAvailableGames(){
    var responce = await fetch(url + 'GetAllAvailableGames',{
        method: 'GET',
        headers: {
            'Accept': 'application/json',
        }
    });
    if (responce.ok){
        var games = await responce.json();
        var roomsContainer = document.getElementById('roomsContainer');
        roomsContainer.innerHTML = ''; 
        for (i in games){
            if (myPlayer != null && myPlayer.id == games[i].ownerId)
                continue;

            let game = await getGame(games[i].id);
            //console.log(game.id);
            let owner = await getPlayer(game.ownerId);

            let div = document.createElement('div');
            div.onclick = function(){
                connectToGame(game.id);
            };

            div.innerHTML = owner.nickname;
            div.classList.add('border-secondary-dark', 'p-1', 'm-1');
            roomsContainer.appendChild(div);
        }
    }
    else{
        alert("HTTP Error: " + responce.status);
    }
}
async function getGame(guid)
{
    var responce = await fetch(url + 'GetGame?gameId=' + guid, {
        method: 'GET',
        headers: {
            'Accept': 'application/json',
        }
    });
    if (responce.ok){
        let result = await responce.json();
        //showGame(result);
        return result;
    }
    else{
        alert("HTTP Error: " + responce.status);
    }
}
async function getPlayer(guid)
{
    var responce = await fetch(url + 'GetPlayer?playerId=' + guid, {
        method: 'GET',
        headers: {
            'Accept': 'application/json',
        }
    });
    if (responce.ok){
        return responce.json();
    }
    else{
        alert("HTTP Error: " + responce.status);
    }
}
async function connectToGame(guid)
{
    //alert('подключаемся епта');
    var responce = await fetch(url + 'ConnectToGame?gameId='+guid+'&playerId='+myPlayer.id,{
        method: 'POST',
        headers: {
            'Accept': 'application/json',
        }
    });
    if (responce.ok){
        myGame = await getGame(guid);
        showGameArea();
        showGame(myGame);
        updatingGame = true;
    }
    else
        alert('HTTP Error:' + responce.status);
}
function showGameArea()
{
    gameArea.classList.remove('hidden');
    gamePlug.classList.add('hidden');
}
function showGamePlug()
{
    gamePlug.classList.remove('hidden');
    gameArea.classList.add('hidden');
}
async function updateMyGame(){
    if (myGame != null && updatingGame){
        myGame = await getGame(myGame.id);
        showGame(myGame);
    }
    setTimeout(updateMyGame, 1000);
}
async function showGame(game){
    let owner = await getPlayer(game.ownerId);
    nickname1.innerHTML = owner.nickname + '('+ oSign +')';
    if (game.guestId != null){
        let guest = await getPlayer(game.guestId);
        nickname2.innerHTML = guest.nickname + ''+xSign+')'; 
    }
    else{
        nickname2.innerHTML = '-('+xSign+')';
    }
    for (i in fieldCells){
        let sign = game.field.at(i)
        if (sign == emptySign)
            sign = '';
        fieldCells[i].innerHTML = sign;
    }
    switch (game.status){
        case 0:
            info.innerHTML = 'Ожидание игрока';
            showRestartButton(false);
            break;
        case 1:
            let imOwner = game.ownerId == myPlayer.id;
            if (imOwner){
                if (game.ownerTurn){
                    info.innerHTML = 'Ваш ход';
                }
                else{
                    info.innerHTML = 'Ход противника';
                }
            }  
            else{
                if (game.ownerTurn){
                    info.innerHTML = 'Ход противника';
                }
                else{
                    
                    info.innerHTML = 'Ваш ход';
                }
            }
            showRestartButton(false);
            break;
        case 2:
            info.innerHTML = 'Игра завершена. ';
            if (game.winnerId != null){
                let winner = await getPlayer(game.winnerId);
                info.innerHTML += ('Победитель ' + winner.nickname); 
            }
            else{
                info.innerHTML += ('Ничья'); 
            }
            showRestartButton(true);
            break;
    }
}
function showRestartButton(show)
{
    if (show){
        //restartButton.classList.replace(/hidden/g,''); //https://stackoverflow.com/questions/10398931/how-to-remove-text-from-a-string
        restartButton.classList.remove('hidden');
    }
    else{
        restartButton.classList.remove('hidden');
        restartButton.classList.add('hidden');
    }
}
async function makeMove(x, y){
    var responce = await fetch(url + 'MakeMove?gameId='+myGame.id+'&playerId='+myPlayer.id+'&x='+x+'&y='+y,{
        method: 'POST',
        headers: {
            'Accept': 'application/json',
        }
    });
    if (!responce.ok){
        alert('HTTP Error: '+responce.status);
    }
}
async function createGame()
{
    var responce = await fetch(url + 'CreateGame?ownerId='+myPlayer.id,{
        method: 'POST',
        headers: {
            'Accept': 'application/json',
        }
    });
    if (responce.ok){
        myGame = await responce.json();
        showGameArea();
        showGame(myGame);
        updatingGame = true;
    }
    else{
        alert('HTTP Error: '+responce.status);
    }
}
async function disconnectFromGame()
{
    var responce = await fetch(url + 'DisconnectFromGame?gameId='+myGame.id+'&playerId='+myPlayer.id,{
        method: 'POST',
        headers: {
            'Accept': 'application/json',
        }
    });
    if (responce.ok){
        myGame = null;
        updatingGame = false;
        showGamePlug();
    }
    else{
        alert('HTTP Error: '+responce.status);
    }
}
async function restartGame()
{
    var responce = await fetch(url + 'RestartGame?gameId='+myGame.id,{
        method: 'POST',
        headers: {
            'Accept': 'application/json',
        }
    });
    if (responce.ok){
        myGame = await responce.json();
    }
    else{
        alert('HTTP Error: ' + responce.status);
    }
}

