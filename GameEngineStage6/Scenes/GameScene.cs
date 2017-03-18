using GameEngineStage6.Core;
using NLua;
using System.Windows.Forms;

namespace GameEngineStage6.Scenes
{
    /// <summary>
    /// Класс, описывающий сцену, на которой будет проходить основная игра
    /// </summary>
    public class GameScene : Scene
    {

        

        public GameScene(GameData.GameState ID, GameData gd) : base(ID, gd)
        {

        }

        /// <summary>
        /// Инициализация сцены
        /// </summary>
        public override void Init()
        {
            base.Init();

            // Создать объект - тайловую карту и загрузить данные из файла
            //gd.map = Map.Load(@"Resources\Level_001.tmx");

            

        }

        public override void KeyDown(object sender, KeyEventArgs e)
        {
            base.KeyDown(sender, e);
        }

        public override void Render()
        {
            base.Render();
        }

        public override void Update(int delta)
        {
            base.Update(delta);
        }
    }
}
