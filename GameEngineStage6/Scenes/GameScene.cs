using GameEngineStage6.Core;
using GameEngineStage6.Entities;
using GameEngineStage6.Utils;
using System.Drawing;
using System.Windows.Forms;

namespace GameEngineStage6.Scenes
{
    /// <summary>
    /// Класс, описывающий сцену, на которой будет проходить основная игра
    /// </summary>
    public class GameScene : Scene
    {

        TiledMapObject tmo;

        public GameScene(GameData.GameState ID, GameData gd) : base(ID, gd)
        {

        }

        /// <summary>
        /// Инициализация сцены
        /// </summary>
        public override void Init()
        {
            base.Init();

            // Загрузить ресурсы, необходимые для данной сцены
            gd.rm.Clear();

            // Создать объект - тайловую карту и загрузить данные из файла
            gd.map = Map.Load(@"Resources\Level_001.tmx");

            // Загрузить отдельные спрайты в менеджер ресурсов как самостоятельные изображения (для ускорения отображения)
            Tileset ts = gd.map.Tilesets["Tiles"];

            int tileCount = ts.FirstTileID;
            // Загрузить базовое изображение
            gd.rm.AddElementAsImage(ts.Image, @"Resources\" + ts.Image);

            // Создать объект - карту спрайтов
            gd.ss = new SpriteSheet(gd.rm.GetImage(ts.Image), ts.TileWidth, ts.TileHeight, ts.Spacing, ts.Margin);

            // Цикл по спрайтам внутри матрицы спрайтов
            for (int j = 0; j < ts.ImageHeight / ts.TileHeight; j++)
            {
                for (int i = 0; i < ts.ImageWidth / ts.TileWidth; i++)
                {
                    // Добавить этот спрайт в хранилище и наименованием tileset-<порядковый номер спрайта>
                    // TODO: как быть, если наборов тайлов несколько? Как вести нумерацию?
                    gd.rm.AddElementAsImage("tileset-" + tileCount, gd.ss.GetSprite(i, j));
                    tileCount++;
                }
            }

            // Заполнить массив проходимости тайлов
            foreach (int key in ts.TileProperties.Keys)
            {
                Tileset.TilePropertyList tpl = ts.TileProperties[key];
                //gd.log.write("key:" + (ts.FirstTileID + key) + " passability:" + tpl["Passability"]);
                if ("1".Equals(tpl["Passability"]))
                {
                    gd.canMove.Add(ts.FirstTileID + key);
                }
            }

            // Создать объект для отображения карты
            tmo = new TiledMapObject("TiledMapObject", gd, gd.map);
            // Координаты игрового поля на экране
            tmo.SetPosition(CONFIG.START_X, CONFIG.START_Y);
            // Другие параметры
            tmo.SetLayer(1);

            // Добавить объект на сцену
            objects.Add(tmo);
            
        }

        public override void KeyDown(object sender, KeyEventArgs e)
        {
            base.KeyDown(sender, e);

            if (e.KeyCode == Keys.Escape)
            {
                Application.Exit();
            }

            // Временно: сдвиг камеры над игровым полем
            if (e.KeyCode == Keys.W)
            {
                Rectangle rect = tmo.ViewPort;
                tmo.ViewPort = new Rectangle(rect.X, rect.Y - 5, rect.Width, rect.Height);
            }
            if (e.KeyCode == Keys.S)
            {
                Rectangle rect = tmo.ViewPort;
                tmo.ViewPort = new Rectangle(rect.X, rect.Y + 5, rect.Width, rect.Height);
            }
            if (e.KeyCode == Keys.A)
            {
                Rectangle rect = tmo.ViewPort;
                tmo.ViewPort = new Rectangle(rect.X - 5, rect.Y, rect.Width, rect.Height);
            }
            if (e.KeyCode == Keys.D)
            {
                Rectangle rect = tmo.ViewPort;
                tmo.ViewPort = new Rectangle(rect.X + 5, rect.Y, rect.Width, rect.Height);
            }
        }

        public override void Render()
        {
            base.Render();
        }

        public override void Update(int delta)
        {
            base.Update(delta);
        }
    }
}
