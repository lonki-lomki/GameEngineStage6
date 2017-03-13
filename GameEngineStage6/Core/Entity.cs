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

        public void setId(String value)
        {
            this.id = value;
        }

        public String getId()
        {
            return this.id;
        }

        /// <summary>
        /// Установка нового положения объекта в пространстве окна
        /// </summary>
        /// <param name="x">Координата Х</param>
        /// <param name="y">Координата У</param>
        virtual public void setPosition(float x, float y)
        {
            position.X = x;
            position.Y = y;

            //bbox.Location = position;
            if (this.cldr != null)
            {
                // Так как объект прямоугольный, найти центр объекта не сложно
                float center_x = x + this.size.Width / 2;
                float center_y = y + this.size.Height / 2;
                this.cldr.SetCenter(center_x, center_y);
            }
        }

        /// <summary>
        /// Установка нового положения объекта в пространстве окна
        /// </summary>
        /// <param name="p">Координаты</param>
        virtual public void setPosition(PointF p)
        {
            setPosition(p.X, p.Y);
        }

        /// <summary>
        /// Получить текущее положение объекта в пространстве окна
        /// </summary>
        /// <returns>Координаты (объект PointF)</returns>
        virtual public PointF getPosition()
        {
            return position;
        }

        /// <summary>
        /// Установить новое положение объекта в тайловых координатах
        /// </summary>
        /// <param name="x">Новая тайловая координата Х.</param>
        /// <param name="y">Новая тайловая координата У.</param>
        virtual public void setTilePosition(float x, float y)
        {
            tilePosition.X = x;
            tilePosition.Y = y;

            // Откорректировать пиксельную позицию
            this.setPosition(CONFIG.START_X + x * CONFIG.TILE_SIZE, CONFIG.START_Y + y * CONFIG.TILE_SIZE);
        }

        /// <summary>
        /// Установить новое положение объекта в тайловых координатах
        /// </summary>
        /// <param name="p">Новые тайловые координаты</param>
        virtual public void setTilePosition(PointF p)
        {
            setTilePosition(p.X, p.Y);
        }

        /// <summary>
        /// Получить текущее положение объекта в тайловых координатах
        /// </summary>
        /// <returns>Тайловые координаты объекта</returns>
        virtual public PointF getTilePosition()
        {
            return tilePosition;
        }

        /// <summary>
        /// Установка размера объекта
        /// </summary>
        /// <param name="width">Ширина</param>
        /// <param name="height">Высота</param>
        virtual public void setSize(float width, float height)
        {
            size.Width = width;
            size.Height = height;
            bbox.Size = size;
        }

        /// <summary>
        /// Получить текущий размер объекта
        /// </summary>
        /// <returns>размер объекта</returns>
        virtual public SizeF getSize()
        {
            return size;
        }

        virtual public void setVelocity(float x, float y)
        {
            velocity.X = x;
            velocity.Y = y;
        }

        virtual public PointF getVelocity()
        {
            return velocity;
        }

        /// <summary>
        /// Установка полигона, по которому будет проверяться коллизия
        /// </summary>
        /// <param name="poly">полигон</param>
        virtual public void setBbox2(Polygon poly)
        {
            this.bbox2 = poly;
        }

        /// <summary>
        /// Изменить скорость объекта
        /// </summary>
        /// <param name="diff">добавка к скорости</param>
        virtual public void addVelocity(PointF diff)
        {
            velocity.X += diff.X;
            velocity.Y += diff.Y;
        }

        /// <summary>
        /// Инкрементально повернуть объект на указанный угол
        /// </summary>
        /// <param name="angle">угол поворота (в градусах)</param>
        virtual public void addAngle(float angle)
        {
            // Повернуть объект
            setAngle(this.angle + angle);

        }

        /// <summary>
        /// Установка нового угла поворота объекта (абсолютный угол)
        /// </summary>
        /// <param name="angle">абсолютный угол поворота объекта в градусах</param>
        virtual public void setAngle(float angle)
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
            if (this.cldr != null)
            {
                this.cldr.SetAngle(this.angle);
            }

        }

        /// <summary>
        /// Получить текущий угол поворота объекта
        /// </summary>
        /// <returns>текущий угол поворота</returns>
        virtual public float getAngle()
        {
            return this.angle;
        }

        /// <summary>
        /// Функция получения прямоугольника, описывающего положение и размер объекта
        /// </summary>
        /// <returns>прямоугольник, описывающий положение и размер объекта</returns>
        virtual public RectangleF getBbox()
        {
            return bbox;
        }

        /// <summary>
        /// Получить коллайдер из объекта
        /// </summary>
        /// <returns>объект-коллайдер или null</returns>
        //virtual public PolygonCollider getCollider()
        virtual public Collider getCollider()
        {
            return this.cldr;
        }

        /// <summary>
        /// Проверка коллизии текущего и указанного объектов
        /// </summary>
        /// <param name="obj">объект для проверки коллизии</param>
        /// <returns>true - если есть факт коллизии, иначе false</returns>
        virtual public bool hasCollision(Entity obj)
        {
            if (this.cldr == null)
            {
                return false;
            }
            return this.cldr.HasCollision(obj.getCollider());
        }

        /// <summary>
        /// Сохранить состояние объекта
        /// </summary>
        virtual public void saveState()
        {
            savedPos = position;
            savedAngle = angle;
            savedVelocity = velocity;
            isSaved = true;
        }

        /// <summary>
        /// Восстановить состояние объекта
        /// </summary>
        virtual public void restoreState()
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
        virtual public void setGravity(bool value)
        {
            isGravity = value;
        }

        /// <summary>
        /// Получить состояние флага гравитации для объекта
        /// </summary>
        /// <returns>состояние флага гравитации</returns>
        virtual public bool hasGravity()
        {
            return isGravity;
        }

        /// <summary>
        /// Имеет ли объект крылья и может ли он планировать
        /// </summary>
        /// <returns>true - если может</returns>
        virtual public bool mayGlide()
        {
            return isGlider;
        }

        /// <summary>
        /// Имеет ли объект двигатель
        /// </summary>
        /// <returns>true, если есть двигатель</returns>
        virtual public bool hasEngine()
        {
            return isEngine;
        }

        /// <summary>
        /// Установить признак наличия у объекта двигателя
        /// </summary>
        /// <param name="value">true - есть двигатель, false - нет двигателя</param>
        virtual public void setEngine(bool value)
        {
            this.isEngine = value;
        }

        /// <summary>
        /// Установить картинку для отрисовки объекта
        /// </summary>
        /// <param name="img">графический объект - картинка</param>
        virtual public void setImage(Image img)
        {
            this.img = img;
            // Размер объекта равен размеру картинки
            setSize(img.Size.Width, img.Size.Height);
            //this.size = img.Size;
        }

        /// <summary>
        /// Получить картинку, которую необходимо отрисовать
        /// </summary>
        /// <returns>картинка</returns>
        virtual public Image getImage()
        {
            return img;
        }

        //virtual public void setCollider(PolygonCollider c)
        virtual public void setCollider(Collider c)
        {
            this.cldr = c;
        }

        /// <summary>
        /// Установить номер визуального уровня
        /// </summary>
        /// <param name="value">Новое значение визуального уровня</param>
        virtual public void setLayer(int value)
        {
            this.layer = value;
        }

        /// <summary>
        /// Получить номер визуального уровня
        /// </summary>
        /// <returns>Текущий номер визуального уровня</returns>
        virtual public int getLayer()
        {
            return this.layer;
        }

        /// <summary>
        /// Получить информацию, уничтожен объект или нет.
        /// </summary>
        /// <returns>true - объект уничтожен, иначе - объект действующий</returns>
        virtual public bool isDestroyed()
        {
            return this.destroyed;
        }

        /// <summary>
        /// Установить значение свойства, указывающего уничтожен объект или нет.
        /// </summary>
        /// <param name="value">true - объект надо уничтожить</param>
        virtual public void setDestroyed(bool value)
        {
            this.destroyed = value;
        }

        /// <summary>
        /// Получить текущее значение тяги двигателя
        /// </summary>
        /// <returns>текущая тяга двигателя</returns>
        virtual public float getEngPower()
        {
            return this.engPower;
        }

        /// <summary>
        /// Установить значение можности двигателя
        /// </summary>
        /// <param name="value">новое значение тяги двигателя</param>
        virtual public void setEngPower(float value)
        {
            this.engPower = value;
        }

        /// <summary>
        /// Добавить к тяге двигателя данное значение
        /// </summary>
        /// <param name="value">значение, которое будет добавлено к тяге</param>
        virtual public void addEngPower(float value)
        {
            this.engPower += value;
            // Проверка граничных значений
            if (this.engPower < 0.0f)
            {
                this.engPower = 0.0f;
            }
            if (this.engPower > CONFIG.MAX_ENG_POWER)
            {
                this.engPower = CONFIG.MAX_ENG_POWER;
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

        /// <summary>
        /// Вывод объекта на сцену
        /// </summary>
        /// <param name="g">графический контекст</param>
        virtual public void render(Graphics g)
        {
            if (this.img != null)
            {
                // Проверить необходимость поворота изображения
                if (this.angle != 0.0f)
                {
                    // Поворот
                    Bitmap returnBitmap = new Bitmap(this.img.Width, this.img.Height);
                    using (Graphics graphics = Graphics.FromImage(returnBitmap))
                    {
                        graphics.TranslateTransform((float)this.img.Width / 2, (float)this.img.Height / 2);
                        graphics.RotateTransform(this.angle);
                        graphics.TranslateTransform(-(float)this.img.Width / 2, -(float)this.img.Height / 2);
                        graphics.DrawImage(this.img, 0.0f, 0.0f, this.img.Width, this.img.Height);
                    }
                    g.DrawImage(returnBitmap, this.getPosition().X, this.getPosition().Y, this.getSize().Width, this.getSize().Height);
                }
                else
                {
                    // Вывод изображения на экран без поворота
                    g.DrawImage(this.img, this.getPosition().X, this.getPosition().Y, this.getSize().Width, this.getSize().Height);
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
        virtual public void update(int delta)
        {
            //velocity.X = engPower;
            // delta - (в милисекундах) время, прошедшее после прошлого запуска функции
            //position.X += (velocity.X / 1000.0f) * delta;
            //position.Y += (velocity.Y / 1000.0f) * delta;
            setPosition(position.X + (velocity.X / 1000.0f) * delta, position.Y + (velocity.Y / 1000.0f) * delta);

        }
    }
}
