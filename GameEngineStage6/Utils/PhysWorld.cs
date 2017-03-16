using GameEngineStage6.Core;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GameEngineStage6.Utils
{
    /// <summary>
    /// Класс, позволяющий хранить и обрабатывать объекты по физическим законам
    /// (законы упрощённые, применительно к летающим объектам)
    /// </summary>
    public class PhysWorld
    {
        public List<Entity> objects = new List<Entity>();
        public List<Entity> tmp_objects = new List<Entity>();

        private Logger log;


        /// <summary>
        /// Конструктор класса PhysWorld
        /// </summary>
        public PhysWorld()
        {
        }

        public PhysWorld(Logger log)
        {
            this.log = log;
        }

        public void Add(Entity e)
        {
            objects.Add(e);
        }

        /// <summary>
        /// (КОСТЫЛЬ) Добавить объект во временный массив, для последующего слияния с основным массивом
        /// </summary>
        /// <param name="e"></param>
        public void Add2(Entity e)
        {
            tmp_objects.Add(e);
        }

        /// <summary>
        /// Обновление состояния объектов физического мира
        /// </summary>
        public void Update(int delta)
        {
            // Добавить объекты из временного массива в основной
            for (int i = tmp_objects.Count - 1; i >= 0; i--)
            {
                Add(tmp_objects[i]);
                tmp_objects.RemoveAt(i);
            }

            // Цикл расчёта новых координат объектов
            foreach (Entity e in objects)
            {
                // Сохранить предыдущее состояние объекта
                e.saveState();

                // Вычислить действующие силы: гравитацию и тягу двигателя
                // Гравитация
                float gravityForce = (e.hasGravity() == true) ? CONFIG.PHYS_GRAVITY : 0.0f;
                // Тяга двигателя
                float engineForce = (e.hasEngine() == true) ? e.getEngPower() : 0.0f;

                // Разложить тягу двигателя по направлениям, в зависимости от угла поворота
                float angle2 = e.getAngle() * ((float)Math.PI / 180); // Перевод градусов в радианы
                float tmp_x = engineForce * (float)Math.Cos(angle2);
                float tmp_y = engineForce * (float)Math.Sin(angle2);

                // Рассчитать новые значения скоростей по Х и У с учетом тяги двигателя и гравитации
                // ... использовать новую формулу (тяга двигателя вверх (-), гравитация вниз (+))
                e.addVelocity(new PointF(tmp_x * delta / 1000.0f, (gravityForce - tmp_y) * (delta / 1000.0f)));

                // Обновить позицию объекта
                e.update(delta);

                // Проверить коллизию
                foreach (Entity e2 in objects)
                {
                    if (e.GetHashCode() != e2.GetHashCode())
                    {
                        if (e.hasCollision(e2) == true)
                        {
                            e.setVelocity(0.0f, 0.0f);
                            e2.setVelocity(0.0f, 0.0f);
                            e.restoreState();
                        }
                    }
                }
            }
        }

        // HACK иногда зависает (скорее всего, когда шаг назад не выводит из коллизии)

        // TODO при коллизии сделать интерполяцию движения, чтобы остановиться резко, а не с медленным долётом

        // TODO обработка событий клавиатуры

        // TODO как сделать поворот объекта на произвольный угол?

        // TODO доделать работу с полигонами для проверки коллизии
    }

}
