namespace GameEngineStage6.Core
{
    public class CONFIG
    {
        // Размер окна программы
        public static readonly int WIND_WIDTH = 1024;
        public static readonly int WIND_HEIGHT = 600;

        // Координата начала игрового поля
        public static readonly int START_X = 20;
        public static readonly int START_Y = 30;

        public static readonly int VIEWPORT_WIDTH = 800;
        public static readonly int VIEWPORT_HEIGHT = 512;

        // Размер тайла
        public static readonly int TILE_SIZE = 48;

        public static readonly float PHYS_GRAVITY = 1.1f; //5.0f; // Гравитация для физ. движка

        public static readonly float MAX_ENG_POWER = 5.0f;  // Максимальная мощность двигателя
    }
}
