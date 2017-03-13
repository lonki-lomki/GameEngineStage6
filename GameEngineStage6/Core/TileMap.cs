using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace GameEngineStage6.Core
{
    public class TileMap
    {

        //private Logger log;

        public List<string> map;
        public Hashtable legend;

        /// <summary>
        /// Ширина карты в тайлах
        /// </summary>
        public int width;

        /// <summary>
        /// Высота карты в тайлах
        /// </summary>
        public int height;

        public TileMap()
        {
            //log = new Logger ("tilemap.log");
        }

        /// <summary>
        /// Получить тип тайла по указанным координатам
        /// </summary>
        /// <param name="x">Координата Х</param>
        /// <param name="y">Координата У</param>
        /// <returns></returns>
        public char GetTile(int x, int y)
        {
            //log.write ("x:"+x+" y:"+y);
            //log.write ("tile code:"+map[y].Substring(x, 1));
            //log.flush ();
            return map[y].Substring(x, 1).ToCharArray()[0];
        }

        public char GetTile(PointF pos)
        {
            return GetTile((int)pos.X, (int)pos.Y);
        }

        public void LoadMap(Stream levelFile)
        {
            this.Load(levelFile, out map, out legend);
            // Получить высоту игрового поля по количеству строк
            this.height = map.Count;
            // Получить ширину игрового поля по длине первой строки
            this.width = map[0].Length;
        }

        private void Load(Stream levelFile, out List<string> map, out Hashtable legend)
        {
            TextReader tr = null;
            map = null;
            legend = null;

            //log.write (filename);

            //tr = new StringReader(File.ReadAllText(filename));

            tr = new StreamReader(levelFile);

            string str;

            while ((str = tr.ReadLine()) != null)
            {

                if (str.StartsWith(";") == true)
                {
                    // Пропускаем комментарий
                    continue;
                }

                if (str.ToUpper().StartsWith("[MAP]"))
                {
                    // Загрузка карты
                    //log.write ("[MAP]");
                    map = ParseMap(tr);
                    continue;
                }
                if (str.ToUpper().StartsWith("[LEGEND]"))
                {
                    // Загрузка легенды
                    //log.write ("[LEGEND]");
                    legend = ParseLegend(tr);
                    continue;
                }
                if (str.ToUpper().StartsWith("[END]"))
                {
                    // Окончание обработки входного файла
                    //log.write ("[END]");
                    break;
                }

            }

            tr.Close();
            //log.close ();
        }

        private List<string> ParseMap(TextReader tr)
        {
            List<string> result = new List<string>();
            string str;

            while (tr.Peek() != -1 && tr.Peek() != '[')
            {

                str = tr.ReadLine();

                // Пропустить комментарии
                if (str.StartsWith(";") == true)
                {
                    continue;
                }
                //log.write (str);
                result.Add(str);
            }
            return result;
        }

        private Hashtable ParseLegend(TextReader tr)
        {
            Hashtable result = new Hashtable();
            string str;

            while (tr.Peek() != -1 && tr.Peek() != '[')
            {

                str = tr.ReadLine();

                // Пропустить комментарии
                if (str.StartsWith(";") == true)
                {
                    continue;
                }

                string[] arr = str.Split(':');
                if (arr.Length == 2)
                {
                    result.Add(arr[0], arr[1]);
                    //log.write ("0:"+arr[0]+" 1:"+arr[1]);
                }
            }
            return result;
        }
    }
}
