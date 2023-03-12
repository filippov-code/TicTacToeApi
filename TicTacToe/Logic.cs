
namespace TicTacToe
{
    /// <summary>
    /// Содержит логику игру в крестики нолики
    /// </summary>
    public static class Logic
    {
        /// <summary>
        /// Проверяет есть ли победитель на игровом поле
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static bool ThereIsAWinnerOnField(char[,] field)
        {
            //Check horisontals
            for (int y = 0; y < 3; y++)
            {
                if (field[0, y] == Instruments.EmptySign)
                    continue;
                if (field[0, y] == field[1, y] && field[0, y] == field[2, y])
                    return true;
            }
            //Check verticals
            for (int x = 0; x < 3; x++)
            {
                if (field[x, 0] == Instruments.EmptySign)
                    continue;
                if (field[x, 0] == field[x, 1] && field[x, 0] == field[x, 2])
                    return true;
            }
            //Check main diagonal
            if (field[0, 0] != Instruments.EmptySign)
            {
                if (field[0, 0] == field[1, 1] && field[0, 0] == field[2, 2])
                    return true;
            }
            //Check second diagonal
            if (field[2, 0] != Instruments.EmptySign)
            {
                if (field[2, 0] == field[1, 1] && field[2, 0] == field[0, 2])
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Проверяет возможен ли ход в заданной таблице игрового поля
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static bool MoveHaveEndedOnField(char[,] field)
        {
            foreach (var sign in field)
                if (sign == Instruments.EmptySign)
                    return false;
            return true;
        }
    }}