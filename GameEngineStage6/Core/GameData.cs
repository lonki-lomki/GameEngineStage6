using GameEngineStage6.Entities;
using GameEngineStage6.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace GameEngineStage6.Core
{
    public class GameData
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

        private static GameData instance;

        public Image backgroundImage;

        public HashSet<Keys> PressedKeys = new HashSet<Keys>();

        public Bitmap worldImage;   // Буфер для отображения мира (общая карта, из которой камера будет отображать некоторую часть)

        public TiledMapObject tmo;


        /////////////////////////////////////////////////////////

        public Scene curScene = null;

        public bool sceneChange = false;

        public Map map;

        public SpriteSheet ss;

        public List<int> canMove = new List<int>(); // Массив проходимости тайлов - заносятся индексы проходимых тайлов

        public Camera camera;

        public Player player;


        private GameData()
        {
        }

        public static GameData Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameData();
                }
                return instance;
            }
        }

        /// <summary>
        /// Загрузка уровня и инициализация всех переменных для выбранного этапа
        /// </summary>
        public void Init()
        {
        }


    }
}
