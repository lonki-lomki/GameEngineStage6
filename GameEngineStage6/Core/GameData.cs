using GameEngineStage6.Utils;
using System;
using System.Reflection;

namespace GameEngineStage6.Core
{
    class GameData
    {
        // Набор значений для определения текущего состояния игры
        public enum GameState
        {
            NotStarted,
            MainMenu,
            Level,
            LevelWin,
            GameWin,
            GameOver
        }

        public PhysWorld world;

        public Assembly myAssembly;

        public GameState currentGameState = GameState.Level;

        public int score = 0;

        public Random rnd = new Random();

        public Logger log;

        public ResourceManager rm;

        /////////////////////////////////////////////////////////

    }
}
