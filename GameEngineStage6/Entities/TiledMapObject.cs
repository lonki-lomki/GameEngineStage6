using GameEngineStage6.Core;
using GameEngineStage6.Utils;
using System;
using System.Drawing;

namespace GameEngineStage6.Entities
{
    /// <summary>
    /// Класс, описывиющий тайловую карту, импортированную из редактора Tiled
    /// </summary>
    public class TiledMapObject : Entity
    {
        public Map map;

        /// <summary>
        /// Размеры камеры
        /// </summary>
        private Rectangle viewPort = new Rectangle(0, 0, CONFIG.VIEWPORT_WIDTH, CONFIG.VIEWPORT_HEIGHT);

        private Bitmap image;

        public TiledMapObject() : base()
        {
        }

        public TiledMapObject(String id) : base(id)
        {
        }

        public TiledMapObject(String id, GameData gd) : base(id, gd)
        {
        }

        public TiledMapObject(String id, GameData gd, Map map) : base(id, gd)
        {
            this.map = map;
            image = new Bitmap(map.Layers["Layer 1"].Width * CONFIG.TILE_SIZE, map.Layers["Layer 1"].Height * CONFIG.TILE_SIZE);
            Graphics g = Graphics.FromImage(image);

            // Нарисовать всю карту на виртуальном холсте
            for (int j = 0; j < map.Layers["Layer 1"].Height; j++)
            {
                for (int i = 0; i < map.Layers["Layer 1"].Width; i++)
                {
                    int tileCode = map.Layers["Layer 1"].Tiles[i + j * map.Layers["Layer 1"].Width];
                    if (tileCode > 0)
                    {
                        g.DrawImage(gd.rm.GetImage("tileset-" + tileCode), i * gd.ss.GetTileWidth(), j * gd.ss.GetTileHeight(), gd.ss.GetTileWidth(), gd.ss.GetTileHeight());
                    }
                }
            }
        }

        public override void Render(Graphics g)
        {
            base.Render(g);

            // Отображение тайловой карты в виде матрицы
            //map.Layers.Count - количество уровней тайлов
            //map.Layers[i] - описание уровня
            //map.Layers[i].Tiles - массив чисел, описывающих тайлы (матрица развернута в одну строку)
            //map.Width - ширина карты в тайлах
            //map.Height - высота карты в тайлах
            //map.Layers[i].Width - ширина уровня в тайлах
            //map.Layers[i].Height - высота уровня в тайлах

            // Имя файла с тайлами
            //int ccc = map.Tilesets.Count;

            /*
            // Циклы по ячейкам
            for (int j = 0; j < map.Layers["Layer 1"].Height; j++)
            {
                for (int i = 0; i < map.Layers["Layer 1"].Width; i++)
                {
                    int tileCode = map.Layers["Layer 1"].Tiles[i + j * map.Layers["Layer 1"].Width];
                    if (tileCode > 0)
                    {
                        //g.DrawString("" + tileCode, new Font("Arial", 12), Brushes.Black, position.X + i * 14, position.Y + j * 14);
                        //g.DrawImage(gd.ss.getSprite(tileCode-1, 0), position.X + i * gd.ss.getTileWidth(), position.Y + j * gd.ss.getTileHeight(), gd.ss.getTileWidth(), gd.ss.getTileHeight());
                        g.DrawImage(gd.rm.GetImage("tileset-" + tileCode), position.X + i * gd.ss.GetTileWidth(), position.Y + j * gd.ss.GetTileHeight(), gd.ss.GetTileWidth(), gd.ss.GetTileHeight());
                    }
                }
            }
            //g.DrawString(s.Substring(i, 1), new Font("Arial", 12), Brushes.Black, position.X + i * 12, position.Y + j * 12);
            */

            /*
            // Предварительный расчет параметров для циклов
            int startTileX = viewPort.X / CONFIG.TILE_SIZE; // Начальный тайл (левый верхний)
            int startTileY = viewPort.Y / CONFIG.TILE_SIZE;
            int endTileX = startTileX + viewPort.Width / CONFIG.TILE_SIZE;   // Конечный тайл (правый нижний)
            int endTileY = startTileY + viewPort.Height / CONFIG.TILE_SIZE;

            // Проверка выхода за пределы диапазона тайловой карты
            if (startTileX < 0) startTileX = 0;
            if (startTileY < 0) startTileY = 0;
            if (endTileX > map.Layers["Layer 1"].Width) endTileX = map.Layers["Layer 1"].Width;
            if (endTileY > map.Layers["Layer 1"].Height) endTileY = map.Layers["Layer 1"].Height;
            */

            // TODO: вывод части тайла 
            // position.X - координата в окне, откуда начинать выводить карту
            // viewPort.X - координата тайловой карты, начиная с которой надо начинать выводить карту

            // Цикл вывода тайлов, которые попадают в область видимости камеры
            /*
            for (int j = startTileY, jj = 0; j < endTileY; j++, jj++)
            {
                for (int i = startTileX, ii = 0; i < endTileX; i++, ii++)
                {
                    int tileCode = map.Layers["Layer 1"].Tiles[i + j * map.Layers["Layer 1"].Width];
                    if (tileCode > 0)
                    {
                        g.DrawImage(gd.rm.GetImage("tileset-" + tileCode), position.X + ii * gd.ss.GetTileWidth(), position.Y + jj * gd.ss.GetTileHeight(), gd.ss.GetTileWidth(), gd.ss.GetTileHeight());
                    }
                }
            }
            */

            // Вывести часть игрового поля, на которую указывает viewPort
            g.DrawImage(image, new Rectangle(CONFIG.START_X, CONFIG.START_Y, viewPort.Width, viewPort.Height), viewPort, GraphicsUnit.Pixel);

            // Нарисовать границы области видимости игрового поля
            g.DrawRectangle(Pens.LightGreen, CONFIG.START_X, CONFIG.START_Y, viewPort.Width, viewPort.Height);

        }

        public Rectangle ViewPort
        {
            get
            {
                return viewPort;
            }
            set
            {
                viewPort = value;
            }
        }
    }
}
