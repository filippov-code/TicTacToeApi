using System.Text;

namespace TicTacToe
{
    public static class Instruments
    {
        public const char XSign = 'X';
        public const char OSign = 'O';
        public const char EmptySign = '.';
        /// <summary>
        /// Возвращает пустое поле в виде строки
        /// </summary>
        /// <returns></returns>
        public static string GetEmptyField()
        {
            return string.Join("", Enumerable.Range(0, 9).Select(x => EmptySign));
        }
        /// <summary>
        /// Конвертирует строковое представление игрового поля в таблицу символов
        /// </summary>
        /// <param name="field"></param>
        /// <returns>Игровое поле в виде двумерного массива</returns>
        public static char[,] ConvertStringToFieldTable(string field)
        {
            char[,] fieldChars = new char[3, 3];
            for (int y = 0; y < 3; y++)
                for (int x = 0; x < 3; x++)
                    fieldChars[x, y] = field[y * 3 + x];

            return fieldChars;
        }
        /// <summary>
        /// Ковертирует таблицу игрового поля в строку
        /// </summary>
        /// <param name="fieldTable"></param>
        /// <returns>Игровое поле в виде строки</returns>
        public static string ConvertFieldTableToString(char[,] fieldTable)
        {
            StringBuilder sb = new();
            for (int y = 0; y < 3; y++)
                for (int x = 0; x < 3; x++)
                    sb.Append(fieldTable[x, y]);

            //foreach (char c in fieldTable)
            //    sb.Append(c);

            string result = sb.ToString();
            return result;
        }
    }
}
