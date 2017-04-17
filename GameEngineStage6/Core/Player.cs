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
            //TODO: центр спрайта!!!

            base.Render(g);
        }

        public override void Update(int delta)
        {
            base.Update(delta);
        }
    }
}