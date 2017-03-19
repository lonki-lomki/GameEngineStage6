using GameEngineStage6.Core;
using System.Drawing;

namespace GameEngineStage6.Entities
{
    /// <summary>
    /// Класс, описывающий визуальный объект, который отображается на сцене как текст
    /// </summary>
    public class TextObject : Entity
    {
        /// <summary>
        /// Время жизни объекта
        /// </summary>
        private int timeToLive = 2000;

        // Констркуторы
        public TextObject()
            : base()
        {
        }

        public TextObject(string id)
            : base(id)
        {
        }

        public TextObject(string id, GameData gd)
            : base(id, gd)
        {
        }

        public override void Render(System.Drawing.Graphics g)
        {
            //gd.log.write("Rendered!");
            //gd.log.flush();
            g.DrawString(id, new Font("Arial", 15), Brushes.Red, GetPosition());
        }

        public override void Update(int delta)
        {
            // Отработать базовое поведение
            base.Update(delta);
            // Уменьшить время жизни
            timeToLive -= delta;
            if (timeToLive <= 0)
            {
                // Пометить объект для уничтожения
                SetDestroyed(true);
            }
        }
    }
}
