using GameEngineStage6.Core;
using GameEngineStage6.Utils;
using System;
using System.Drawing;
using System.Drawing.Imaging;

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

        public Bitmap image;

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
            image = new Bitmap(map.Layers["Layer 1"].Width * CONFIG.TILE_SIZE, map.Layers["Layer 1"].Height * CONFIG.TILE_SIZE, PixelFormat.Format32bppPArgb);
            gd.worldImage = new Bitmap(map.Layers["Layer 1"].Width * CONFIG.TILE_SIZE, map.Layers["Layer 1"].Height * CONFIG.TILE_SIZE, PixelFormat.Format32bppPArgb);

            Graphics gg = Graphics.FromImage(image);

            // Нарисовать всю карту на виртуальном холсте
            for (int j = 0; j < map.Layers["Layer 1"].Height; j++)
            {
                for (int i = 0; i < map.Layers["Layer 1"].Width; i++)
                {
                    int tileCode = map.Layers["Layer 1"].Tiles[i + j * map.Layers["Layer 1"].Width];
                    if (tileCode > 0)
                    {
                        gg.DrawImage(gd.rm.GetImage("tileset-" + tileCode), i * gd.ss.GetTileWidth(), j * gd.ss.GetTileHeight(), gd.ss.GetTileWidth(), gd.ss.GetTileHeight());
                    }
                }
            }
        }

        public override void Render(Graphics g)
        {

            // TODO: перенести в новый класс - Камера
            // Вывести часть игрового поля, на которую указывает viewPort
            //g.DrawImage(gd.worldImage, new Rectangle(CONFIG.START_X, CONFIG.START_Y, viewPort.Width, viewPort.Height), viewPort, GraphicsUnit.Pixel);

            // Нарисовать границы области видимости игрового поля
            //g.DrawRectangle(Pens.LightGreen, CONFIG.START_X, CONFIG.START_Y, viewPort.Width, viewPort.Height);

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
