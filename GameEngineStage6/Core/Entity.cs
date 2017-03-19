using GameEngineStage6.Utils;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GameEngineStage6.Core
{
    /// <summary>
    /// Класс, описывающий различные объекты
    /// </summary>
    public class Entity
    {
        /// <summary>
        /// Идентификатор объекта
        /// </summary>
        protected String id;

        /// <summary>
        /// Игровые данные
        /// </summary>
        protected GameData gd;

        /// <summary>
        /// Положение объекта в игровом мире
        /// </summary>
        protected PointF position = new PointF(0.0f, 0.0f);

        /// <summary>
        /// Положение объекта в игровом мире в тайловых координатах
        /// </summary>
        private PointF tilePosition = new PointF(0.0f, 0.0f);

        /// <summary>
        /// Угол поворота объекта в градусах (0 угол - это положительное направление оси Х)
        /// </summary>
        protected float angle = 0.0f;

        /// <summary>
        /// Размер объекта
        /// </summary>
        private SizeF size = new SizeF(0.0f, 0.0f);

        /// <summary>
        /// Масса объекта
        /// </summary>
        private float mass = 1.0f;

        /// <summary>
        /// Скорость объекта по двум осям X и Y. Задаётся в количестве пикселей за секунду.
        /// </summary>
        private PointF velocity = new PointF(0.0f, 0.0f);

        /// <summary>
        /// Флаг, указывающий, действует ли на данный объект гравитация
        /// </summary>
        private bool isGravity = false;

        /// <summary>
        /// Флаг, указывающий, что объект может планировать (крыло имеет подъёмную силу)
        /// </summary>
        private bool isGlider = false;

        /// <summary>
        /// У объекта есть двигатель
        /// </summary>
        private bool isEngine = false;

        /// <summary>
        /// Текущая установленная мощность двигателя (равна горизонтальной скорости)
        /// </summary>
        private float engPower = 0.0f;

        /// <summary>
        /// Картинка - визуальное изображение объекта
        /// </summary>
        private Image img = null;

        /// <summary>
        /// Прямоугольник - положение и габариты объекта для определения коллизии
        /// </summary>
        private RectangleF bbox = new RectangleF(0.0f, 0.0f, 0.0f, 0.0f);

        /// <summary>
        /// Полигон для задания положения и габаритов объекта - будет использоваться для определения коллизии
        /// </summary>
        private Polygon bbox2;

        /// <summary>
        /// Коллайдер, который будет использоваться для определения коллизии
        /// </summary>
        //private PolygonCollider cldr = null;
        private Collider cldr = null;

        /// <summary>
        /// Номер визуального уровня (для отображения нескольких объектов с наложением)
        /// </summary>
        private int layer = 0;

        /// <summary>
        /// Информация для внешнего хранилища объектов, что объект надо убрать со сцены, он уничтожен
        /// </summary>
        private bool destroyed = false;

        // Переменные для сохранения состояния объекта
        private PointF savedPos;
        private float savedAngle;
        private PointF savedVelocity;
        private bool isSaved;

        //Logger log;

        /// <summary>
        /// Конструктор класса Entity
        /// </summary>
        public Entity()
        {
        }

        /// <summary>
        /// Конструктор класса Entity с параметром
        /// </summary>
        public Entity(String id)
        {
            this.id = id;
            //log = new Logger("Entity.log");
        }

        public Entity(String id, GameData gd)
        {
            this.id = id;
            this.gd = gd;
        }

        public void SetId(String value)
        {
            id = value;
        }

        public String GetId()
        {
            return id;
        }

        /// <summary>
        /// Установка нового положения объекта в пространстве окна
        /// </summary>
        /// <param name="x">Координата Х</param>
        /// <param name="y">Координата У</param>
        virtual public void SetPosition(float x, float y)
        {
            position.X = x;
            position.Y = y;

            //bbox.Location = position;
            if (cldr != null)
            {
                // Так как объект прямоугольный, найти центр объекта не сложно
                float center_x = x + size.Width / 2;
                float center_y = y + size.Height / 2;
                cldr.SetCenter(center_x, center_y);
            }
        }

        /// <summary>
        /// Установка нового положения объекта в пространстве окна
        /// </summary>
        /// <param name="p">Координаты</param>
        virtual public void SetPosition(PointF p)
        {
            SetPosition(p.X, p.Y);
        }

        /// <summary>
        /// Получить текущее положение объекта в пространстве окна
        /// </summary>
        /// <returns>Координаты (объект PointF)</returns>
        virtual public PointF GetPosition()
        {
            return position;
        }

        /// <summary>
        /// Установить новое положение объекта в тайловых координатах
        /// </summary>
        /// <param name="x">Новая тайловая координата Х.</param>
        /// <param name="y">Новая тайловая координата У.</param>
        virtual public void SetTilePosition(float x, float y)
        {
            tilePosition.X = x;
            tilePosition.Y = y;

            // Откорректировать пиксельную позицию
            SetPosition(CONFIG.START_X + x * CONFIG.TILE_SIZE, CONFIG.START_Y + y * CONFIG.TILE_SIZE);
        }

        /// <summary>
        /// Установить новое положение объекта в тайловых координатах
        /// </summary>
        /// <param name="p">Новые тайловые координаты</param>
        virtual public void SetTilePosition(PointF p)
        {
            SetTilePosition(p.X, p.Y);
        }

        /// <summary>
        /// Получить текущее положение объекта в тайловых координатах
        /// </summary>
        /// <returns>Тайловые координаты объекта</returns>
        virtual public PointF GetTilePosition()
        {
            return tilePosition;
        }

        /// <summary>
        /// Установка размера объекта
        /// </summary>
        /// <param name="width">Ширина</param>
        /// <param name="height">Высота</param>
        virtual public void SetSize(float width, float height)
        {
            size.Width = width;
            size.Height = height;
            bbox.Size = size;
        }

        /// <summary>
        /// Получить текущий размер объекта
        /// </summary>
        /// <returns>размер объекта</returns>
        virtual public SizeF GetSize()
        {
            return size;
        }

        virtual public void SetVelocity(float x, float y)
        {
            velocity.X = x;
            velocity.Y = y;
        }

        virtual public PointF GetVelocity()
        {
            return velocity;
        }

        /// <summary>
        /// Установка полигона, по которому будет проверяться коллизия
        /// </summary>
        /// <param name="poly">полигон</param>
        virtual public void SetBbox2(Polygon poly)
        {
            bbox2 = poly;
        }

        /// <summary>
        /// Изменить скорость объекта
        /// </summary>
        /// <param name="diff">добавка к скорости</param>
        virtual public void AddVelocity(PointF diff)
        {
            velocity.X += diff.X;
            velocity.Y += diff.Y;
        }

        /// <summary>
        /// Инкрементально повернуть объект на указанный угол
        /// </summary>
        /// <param name="angle">угол поворота (в градусах)</param>
        virtual public void AddAngle(float angle)
        {
            // Повернуть объект
            SetAngle(this.angle + angle);

        }

        /// <summary>
        /// Установка нового угла поворота объекта (абсолютный угол)
        /// </summary>
        /// <param name="angle">абсолютный угол поворота объекта в градусах</param>
        virtual public void SetAngle(float angle)
        {
            // Нормализация угла поворота (то есть, вернуть угол в диапазон от -180 до 180)
            if (Math.Abs(angle) > 180)
            {
                // 270 = -90
                // -270 = 90
                // 630 mod 360 = 270 = -90

                // Исключить полные 360 градусов
                float tmp = angle % 360;

                // Учесть знак угла
                if (tmp >= 0)
                {
                    this.angle = tmp - 360;
                }
                else
                {
                    this.angle = 360 + tmp;
                }
            }
            else
            {
                // Угол нормализован, можно сразу присваивать
                this.angle = angle;
            }

            //log.write("before:"+angle+" after:"+this.angle);

            // выполнить поворот коллайдера на тот же угол
            if (cldr != null)
            {
                cldr.SetAngle(this.angle);
            }

        }

        /// <summary>
        /// Получить текущий угол поворота объекта
        /// </summary>
        /// <returns>текущий угол поворота</returns>
        virtual public float GetAngle()
        {
            return angle;
        }

        /// <summary>
        /// Функция получения прямоугольника, описывающего положение и размер объекта
        /// </summary>
        /// <returns>прямоугольник, описывающий положение и размер объекта</returns>
        virtual public RectangleF GetBbox()
        {
            return bbox;
        }

        /// <summary>
        /// Получить коллайдер из объекта
        /// </summary>
        /// <returns>объект-коллайдер или null</returns>
        //virtual public PolygonCollider getCollider()
        virtual public Collider GetCollider()
        {
            return cldr;
        }

        /// <summary>
        /// Проверка коллизии текущего и указанного объектов
        /// </summary>
        /// <param name="obj">объект для проверки коллизии</param>
        /// <returns>true - если есть факт коллизии, иначе false</returns>
        virtual public bool HasCollision(Entity obj)
        {
            if (cldr == null)
            {
                return false;
            }
            return cldr.HasCollision(obj.GetCollider());
        }

        /// <summary>
        /// Сохранить состояние объекта
        /// </summary>
        virtual public void SaveState()
        {
            savedPos = position;
            savedAngle = angle;
            savedVelocity = velocity;
            isSaved = true;
        }

        /// <summary>
        /// Восстановить состояние объекта
        /// </summary>
        virtual public void RestoreState()
        {
            if (isSaved == true)
            {
                position = savedPos;
                angle = savedAngle;
                velocity = savedVelocity;
                isSaved = false;
            }
        }

        /// <summary>
        /// Установить значение флага гравитации для объекта
        /// </summary>
        /// <param name="value">новое значение флага гравитации</param>
        virtual public void SetGravity(bool value)
        {
            isGravity = value;
        }

        /// <summary>
        /// Получить состояние флага гравитации для объекта
        /// </summary>
        /// <returns>состояние флага гравитации</returns>
        virtual public bool HasGravity()
        {
            return isGravity;
        }

        /// <summary>
        /// Имеет ли объект крылья и может ли он планировать
        /// </summary>
        /// <returns>true - если может</returns>
        virtual public bool MayGlide()
        {
            return isGlider;
        }

        /// <summary>
        /// Имеет ли объект двигатель
        /// </summary>
        /// <returns>true, если есть двигатель</returns>
        virtual public bool HasEngine()
        {
            return isEngine;
        }

        /// <summary>
        /// Установить признак наличия у объекта двигателя
        /// </summary>
        /// <param name="value">true - есть двигатель, false - нет двигателя</param>
        virtual public void SetEngine(bool value)
        {
            isEngine = value;
        }

        /// <summary>
        /// Установить картинку для отрисовки объекта
        /// </summary>
        /// <param name="img">графический объект - картинка</param>
        virtual public void SetImage(Image img)
        {
            this.img = img;
            // Размер объекта равен размеру картинки
            SetSize(img.Size.Width, img.Size.Height);
            //this.size = img.Size;
        }

        /// <summary>
        /// Получить картинку, которую необходимо отрисовать
        /// </summary>
        /// <returns>картинка</returns>
        virtual public Image GetImage()
        {
            return img;
        }

        //virtual public void setCollider(PolygonCollider c)
        virtual public void SetCollider(Collider c)
        {
            cldr = c;
        }

        /// <summary>
        /// Установить номер визуального уровня
        /// </summary>
        /// <param name="value">Новое значение визуального уровня</param>
        virtual public void SetLayer(int value)
        {
            layer = value;
        }

        /// <summary>
        /// Получить номер визуального уровня
        /// </summary>
        /// <returns>Текущий номер визуального уровня</returns>
        virtual public int GetLayer()
        {
            return layer;
        }

        /// <summary>
        /// Получить информацию, уничтожен объект или нет.
        /// </summary>
        /// <returns>true - объект уничтожен, иначе - объект действующий</returns>
        virtual public bool IsDestroyed()
        {
            return destroyed;
        }

        /// <summary>
        /// Установить значение свойства, указывающего уничтожен объект или нет.
        /// </summary>
        /// <param name="value">true - объект надо уничтожить</param>
        virtual public void SetDestroyed(bool value)
        {
            destroyed = value;
        }

        /// <summary>
        /// Получить текущее значение тяги двигателя
        /// </summary>
        /// <returns>текущая тяга двигателя</returns>
        virtual public float GetEngPower()
        {
            return engPower;
        }

        /// <summary>
        /// Установить значение можности двигателя
        /// </summary>
        /// <param name="value">новое значение тяги двигателя</param>
        virtual public void SetEngPower(float value)
        {
            engPower = value;
        }

        /// <summary>
        /// Добавить к тяге двигателя данное значение
        /// </summary>
        /// <param name="value">значение, которое будет добавлено к тяге</param>
        virtual public void AddEngPower(float value)
        {
            engPower += value;
            // Проверка граничных значений
            if (engPower < 0.0f)
            {
                engPower = 0.0f;
            }
            if (engPower > CONFIG.MAX_ENG_POWER)
            {
                engPower = CONFIG.MAX_ENG_POWER;
            }
        }


        virtual public void OnLeftMouseButtonClick(MouseEventArgs args)
        {

        }

        virtual public void OnRightMouseButtonClick(MouseEventArgs args)
        {

        }


        /// <summary>
        /// Движение объекта по тайловым координатам
        /// </summary>
        /// <param name="x">Изменение координаты Х</param>
        /// <param name="y">Изменение координаты У</param>
        /*
        virtual public void TileMove(int x, int y, TileMap tm, PhysWorld world)
        {
            PointF pos = this.getTilePosition();
            pos.X += x;
            pos.Y += y;
            // TODO: заменить на справочник проходимости
            if (tm.GetTile((int)pos.X, (int)pos.Y) == '.'
                || tm.GetTile((int)pos.X, (int)pos.Y) == '+'
                || tm.GetTile((int)pos.X, (int)pos.Y) == '*'
                || Char.IsDigit(tm.GetTile((int)pos.X, (int)pos.Y)))
            {
                // Переход только на проходимую ячейку
                // Проверить наличие на новой координате пазла
                // Вариант для игрока
                if (this.getLayer() == 2)
                {
                    foreach (Entity ent in world.objects)
                    {
                        if (ent.getLayer() == 1)
                        {
                            // 1 - уровень для пазлов
                            if (pos.Equals(ent.getTilePosition()) == true)
                            {
                                // Новая позиция игрока совпадает с пазлом
                                // Передвигаем пазл в том же направлении
                                ent.TileMove(x, y, tm, world);
                                // Проверить, произошло ли перемещение
                                if (pos.Equals(ent.getTilePosition()) == true)
                                {
                                    // Нет, пазл не переместился - остаёмся на месте
                                    pos = this.getTilePosition();
                                }
                            }
                        }
                    }
                }
                // Вариант для пазла
                if (this.getLayer() == 1)
                {
                    foreach (Entity ent in world.objects)
                    {
                        if (ent.getLayer() == 1)
                        {
                            // 1 - уровень для пазлов
                            if (pos.Equals(ent.getTilePosition()) == true)
                            {
                                // Новая позиция пазла совпадает с пазлом
                                // Не передвигаемся дальше
                                pos = this.getTilePosition();
                            }
                        }
                    }
                }
                // Выполнить перемещение
                this.setTilePosition(pos);
            }
            // Установить угол поворота объекта в сторону движения
            // Только для игрока
            if (this.getLayer() == 2)
            {
                if (x == 1 && y == 0)
                {
                    this.setAngle(0.0f);
                }
                if (x == -1 && y == 0)
                {
                    this.setAngle(180.0f);
                }
                if (x == 0 && y == 1)
                {
                    this.setAngle(90.0f);
                }
                if (x == 0 && y == -1)
                {
                    this.setAngle(-90.0f);
                }
            }
        }
        */

        /// <summary>
        /// Вывод объекта на сцену
        /// </summary>
        /// <param name="g">графический контекст</param>
        virtual public void Render(Graphics g)
        {
            if (img != null)
            {
                // Проверить необходимость поворота изображения
                if (angle != 0.0f)
                {
                    // Поворот
                    Bitmap returnBitmap = new Bitmap(img.Width, img.Height);
                    using (Graphics graphics = Graphics.FromImage(returnBitmap))
                    {
                        graphics.TranslateTransform((float)img.Width / 2, (float)img.Height / 2);
                        graphics.RotateTransform(angle);
                        graphics.TranslateTransform(-(float)img.Width / 2, -(float)img.Height / 2);
                        graphics.DrawImage(img, 0.0f, 0.0f, img.Width, img.Height);
                    }
                    g.DrawImage(returnBitmap, GetPosition().X, GetPosition().Y, GetSize().Width, GetSize().Height);
                }
                else
                {
                    // Вывод изображения на экран без поворота
                    g.DrawImage(img, GetPosition().X, GetPosition().Y, GetSize().Width, GetSize().Height);
                }
            } // if image not null

            // Вывести границы коллайдера
            if (cldr != null)
            {
                cldr.Render(g);
            }
        }

        /// <summary>
        /// Перемещение объекта в мире с учетом текущих скоростей
        /// </summary>
        /// <param name="delta"></param>
        virtual public void Update(int delta)
        {
            //velocity.X = engPower;
            // delta - (в милисекундах) время, прошедшее после прошлого запуска функции
            //position.X += (velocity.X / 1000.0f) * delta;
            //position.Y += (velocity.Y / 1000.0f) * delta;
            SetPosition(position.X + (velocity.X / 1000.0f) * delta, position.Y + (velocity.Y / 1000.0f) * delta);

        }
    }
}
