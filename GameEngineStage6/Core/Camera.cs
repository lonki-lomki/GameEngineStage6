

using System.Drawing;
using System.Drawing.Drawing2D;

namespace GameEngineStage6.Core
{
    /// <summary>
    /// Класс, описывающий камеру, через которую будет отображаться часть игрового мира
    /// </summary>
    public class Camera
    {
        /// <summary>
        /// Геометрия камеры (расположение на экране и размер)
        /// </summary>
        private Rectangle geometry;

        private GameData gd;

        /// <summary>
        /// "Точка зрения камеры" - привязка левым верхним углом к "мировым" координатам
        /// </summary>
        private Point pointOfView;


        public Camera(Rectangle geom)
        {
            geometry = new Rectangle(geom.Location, geom.Size);
            pointOfView = new Point(0, 0);
            gd = GameData.Instance;
        }

        /// <summary>
        /// Отображение части игрового мира, который попадает в поле зрения камеры
        /// </summary>
        /// <param name="g"></param>
        public void Render(Graphics g)
        {
            // Вывести часть тайловой карты, которая попадает в поле зрения камеры
            g.DrawImage(gd.tmo.image, new Rectangle(geometry.X, geometry.Y, geometry.Width, geometry.Height), new Rectangle(pointOfView, new Size(CONFIG.VIEWPORT_WIDTH, CONFIG.VIEWPORT_HEIGHT)), GraphicsUnit.Pixel);


            // Лист для отрисовки объектов
            Graphics gg = Graphics.FromImage(gd.worldImage);
            gg.CompositingMode = CompositingMode.SourceCopy;
            gg.InterpolationMode = InterpolationMode.NearestNeighbor;
            // Очистить прозрачным цветом
            gg.Clear(Color.Transparent);

            //...Нарисовать объекты в мировых координатах и вывести часть, попадающую в область видимости камеры

            // Цикл отображения всех объектов на всех уровнях
            // Цикл по уровням (пока 3 уровня)
            for (int i = 0; i < 3; i++)
            {
                foreach (Entity ent in gd.world.objects)
                {
                    if (ent.GetLayer() == i)
                    {
                        ent.Render(gg);
                    }
                }
            }

            // Вывести часть слоя с объектами, которая попадает в поле зрения камеры
            g.DrawImage(gd.worldImage, new Rectangle(geometry.X, geometry.Y, geometry.Width, geometry.Height), new Rectangle(pointOfView, new Size(CONFIG.VIEWPORT_WIDTH, CONFIG.VIEWPORT_HEIGHT)), GraphicsUnit.Pixel);

            // Нарисовать границы области видимости игрового поля
            g.DrawRectangle(Pens.LightGreen, geometry.X, geometry.Y, geometry.Width, geometry.Height);

        }
    }
}
