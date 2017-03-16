using GameEngineStage6.Core;
using GameEngineStage6.Utils;
using System;
using System.Windows.Forms;
using System.Reflection;


namespace GameEngineStage6
{
    public partial class Form1 : Form
    {

        private Timer timer = new Timer();

        // Счётчик количества тиков
        private long tickCount = 0;
        // Для определения длины интервала времени в тиках
        private long saveTickCount = 0;

        /// <summary>
        /// Игровые данные
        /// </summary>
        private GameData gd;


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            /* Полноэкранный режим
            this.Width = Screen.PrimaryScreen.WorkingArea.Width;
            this.Height = Screen.PrimaryScreen.WorkingArea.Height;
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            */

            Logger log = new Logger("Log.txt");

            gd = GameData.Instance;
            gd.log = log;

            // Получить доступ к ресурсам, встроенным в проект
            gd.myAssembly = Assembly.GetExecutingAssembly();

            // Размер окна программы
            this.Width = CONFIG.WIND_WIDTH;
            this.Height = CONFIG.WIND_HEIGHT;

            // Настройки окна программы
            KeyPreview = true;
            DoubleBuffered = true;

            // Начальные параметры для обработки интервалов по таймеру
            tickCount = Environment.TickCount; //GetTickCount();
            saveTickCount = tickCount;

            // Настройки таймера
            timer.Enabled = true;
            timer.Tick += new EventHandler(OnTimer);
            timer.Interval = 20;
            timer.Start();

            // Создать физический мир
            gd.world = new PhysWorld(log);





        }

        ///////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Обработка событий таймера
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="ea">Ea.</param>
        ///////////////////////////////////////////////////////////////////////
        private void OnTimer(object obj, EventArgs ea)
        {
            int delta;

            // Новое значение времени
            tickCount = Environment.TickCount;

            delta = (int)(tickCount - saveTickCount);

            if (delta == 0)
            {
                // А вдруг!
                return;
            }

            // Вычислить FPS
            float fps = 1000 / delta;

            // Вывести сообщение в заголовке окна
            this.Text = old_title + " - " + fps + " FPS";

            // Проверить флаг смены сцены
            if (gd.sceneChange == true)
            {
                // Удалить все объекты из физ. мира
                gd.world.objects.Clear();

                // Перенести "живые" объекты из текущей сцены в физический мир
                foreach (Entity ent in gd.curScene.objects)
                {
                    if (ent.isDestroyed() == false)
                    {
                        gd.world.add(ent);
                    }
                }
                // Сбросить флаг
                gd.sceneChange = false;
            }

            // Обновить мир
            gd.world.update(delta);

            // Обновить игровую сцену
            gd.curScene.Update(delta);

            // TODO: тестирование анимации
            //////anim.update(delta);

            // Проверить актуальность объектов (убрать со сцены уничтоженные объекты)
            for (int i = gd.world.objects.Count - 1; i >= 0; i--)
            {
                if (gd.world.objects[i].isDestroyed())
                {
                    // Удалить из "мира"
                    gd.world.objects.RemoveAt(i);
                }
            }

            saveTickCount = tickCount;

            Invalidate(false);
        }



    }
}
