

using System.Drawing;

namespace GameEngineStage6.Core
{
    /// <summary>
    /// Класс, описывающий камеру, через которую будет отображаться часть игрового мира
    /// </summary>
    public class Camera
    {
        /// <summary>
        /// Геометрия камеры (расположение и размеры)
        /// </summary>
        private Rectangle geometry;

        /// <summary>
        /// "Точка зрения камеры" - привязка левым верхним углом к "мировым" координатам
        /// </summary>
        private Point pointOfView;


        public Camera(Rectangle geom)
        {
            geometry = new Rectangle(geom.Location, geom.Size);
        }
    }
}
