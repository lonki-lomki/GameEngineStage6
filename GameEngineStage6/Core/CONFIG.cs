namespace GameEngineStage6.Core
{
    public class CONFIG
    {
        // Размер окна программы
        public static readonly int WIND_WIDTH = 1024;
        public static readonly int WIND_HEIGHT = 600;

        // Координата начала игрового поля
        public static readonly int START_X = 20;
        public static readonly int START_Y = 20;

        // Размер тайла
        public static readonly int TILE_SIZE = 64;

        public static readonly float PHYS_GRAVITY = 1.1f; //5.0f; // Гравитация для физ. движка

        public static readonly float MAX_ENG_POWER = 5.0f;  // Максимальная мощность двигателя

        //public static readonly float timeBetweenWaves = 25.0f;  // Время между волнами
        //public static readonly float timeBetweenMob = 1.0f;     // Время между монстрами внутри волны
    }
}
