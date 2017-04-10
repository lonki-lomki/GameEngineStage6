using System;
using System.Drawing;

namespace GameEngineStage6.Core
{
    public class Player : Entity
    {
        public Player() : base()
        {

        }

        public Player(String id) : base(id)
        {

        }

        public Player(String id, GameData gd) : base(id, gd)
        {

        }

        public override void Render(Graphics g)
        {
            // TODO: Грязный хак: перебросить ГГ в центр области отрисовки карты

            TODO: центр спрайта!!!

            PointF save = GetPosition();
            SetPosition(CONFIG.START_X + CONFIG.VIEWPORT_WIDTH / 2, CONFIG.START_Y + CONFIG.VIEWPORT_HEIGHT / 2);

            // Отрисовка ГГ
            base.Render(g);

            // Вернуть ГГ на место в мировых координатах
            SetPosition(save);
        }

        public override void Update(int delta)
        {
            base.Update(delta);
        }
    }
}