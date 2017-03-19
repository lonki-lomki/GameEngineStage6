using System.Drawing;

namespace GameEngineStage6.Utils
{
    /// <summary>
    /// Класс, описывающий набор спрайтов, расположенных в одном изображении
    /// </summary>
    public class SpriteSheet
    {
        /// <summary>
        /// Исходное изображение
        /// </summary>
        private Image image;

        /// <summary>
        /// Ширина одного спрайта
        /// </summary>
        private int tw;

        /// <summary>
        /// Высота одного спрайта
        /// </summary>
        private int th;

        /// <summary>
        /// Промежуток между спрайтами
        /// </summary>
        private int spacing;

        /// <summary>
        /// Ширина границы вокруг спрайта
        /// </summary>
        private int margin;

        /// <summary>
        /// Размер листа спрайтов
        /// </summary>
        private Size size;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="image">изображение матрицы спрайтов</param>
        /// <param name="tw">ширина одного спрайта</param>
        /// <param name="th">высота одного спрайта</param>
        /// <param name="spacing">промежуток между спрайтами</param>
        /// <param name="margin">ширина границы вокруг спрайта</param>
        public SpriteSheet(Image image, int tw, int th, int spacing, int margin)
        {
            this.image = image;
            this.tw = tw;
            this.th = th;
            this.spacing = spacing;
            this.margin = margin;
            this.size = image.Size;
        }

        /// <summary>
        /// Получить количество спрайтов в строке
        /// </summary>
        /// <returns>количество спрайтов в строке</returns>
        public int GetHorizontalCount()
        {
            return size.Width / (tw + spacing + margin * 2);
        }

        /// <summary>
        /// Получить количество спрайтов по высоте
        /// </summary>
        /// <returns>количество спрайтов по высоте</returns>
        public int GetVerticalCount()
        {
            return size.Height / (th + spacing + margin * 2);
        }

        /// <summary>
        /// Вернуть один спрайт из матрицы спрайтов
        /// </summary>
        /// <param name="x">координата Х в матрице</param>
        /// <param name="y">координата У в матрице</param>
        /// <returns>изображение спрайта</returns>
        public Image GetSprite(int x, int y)
        {
            // Проверить корректность координат
            if (x >= GetHorizontalCount() || y >= GetVerticalCount())
            {
                return null;
            }

            // Вычислить координаты левого верхнего угла прямоугольника отсечения
            int crop_x = (tw + margin * 2 + spacing) * x;
            int crop_y = (th + margin * 2 + spacing) * y;


            // TODO: сделать!!!
            // http://stackoverflow.com/questions/734930/how-to-crop-an-image-using-c
            //Image sprite = Image.

            Rectangle cropRect = new Rectangle(crop_x, crop_y, tw, th);

            Bitmap src = new Bitmap(image);
            Bitmap target = new Bitmap(tw, th);
            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(src, new Rectangle(0, 0, tw, th), cropRect, GraphicsUnit.Pixel);
            }

            return target as Image;
        }

        /// <summary>
        /// Получить ширину одного тайла
        /// </summary>
        /// <returns>ширина одного тайла</returns>
        public int GetTileWidth()
        {
            return tw;
        }

        /// <summary>
        /// Получить высоту одного тайла
        /// </summary>
        /// <returns>высота одного тайла</returns>
        public int GetTileHeight()
        {
            return th;
        }
    }
}
