﻿using GameEngineStage6.Core;
using GameEngineStage6.Utils;
using GameEngineStage6.Scenes;
using System;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;
using NLua;

namespace GameEngineStage6
{
    public partial class Form1 : Form
    {

        // TODO: создать два спрайта для ГГ (нижняя часть и верхняя часть)
        // TODO: настроить вращение ГГ по стрелкам ВЛЕВО и ВПРАВО

        // TODO: PixelFormat.Format32bppPArgb format is the fastest
        // TODO: DrawImageUnscaled might work faster than DrawImage
        // TODO: set Graphics.CompositingMode to CompositingMode.SourceCopy before calling DrawImage()
        // TODO: Make sure Graphics.InterpolationMode isn't set to something like InterpolationMode.HighQualityBicubic. Using NearestNeighbor will be the fastest


        private Timer timer = new Timer();

        // Счётчик количества тиков
        private long tickCount = 0;
        // Для определения длины интервала времени в тиках
        private long saveTickCount = 0;

        private string old_title;	// Оригинальный текст в заголовке окна

        /// <summary>
        /// Игровые данные
        /// </summary>
        private GameData gd;

        Lua state;
        object luaVersion;


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

            // Включить двойную буфферизацию графики
            DoubleBuffered = true;

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

            old_title = this.Text;

            // Инициализация менеджера ресурсов
            gd.rm = ResourceManager.Instance;

            // Создание и настройка камеры
            gd.camera = new Camera(new Rectangle(CONFIG.START_X, CONFIG.START_Y, CONFIG.VIEWPORT_WIDTH, CONFIG.VIEWPORT_HEIGHT));

            // Создать стартовую сцену игры
            GameScene gs = new GameScene(GameData.GameState.Level, gd);
            //MainMenuScene scene = new MainMenuScene(GameData.GameState.MainMenu, gd);
            gd.curScene = gs;

            gd.curScene.Init();

            gd.sceneChange = true;

            state = new Lua();
            luaVersion = state.DoString("return _VERSION")[0];



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
            this.Text = old_title + " : " + fps + " FPS --- " + (string) luaVersion;

            // Проверить флаг смены сцены
            if (gd.sceneChange == true)
            {
                // Удалить все объекты из физ. мира
                gd.world.objects.Clear();

                // Перенести "живые" объекты из текущей сцены в физический мир
                foreach (Entity ent in gd.curScene.objects)
                {
                    if (ent.IsDestroyed() == false)
                    {
                        gd.world.Add(ent);
                    }
                }
                // Сбросить флаг
                gd.sceneChange = false;
            }

            // Обновить мир
            gd.world.Update(delta);

            // Обновить игровую сцену
            gd.curScene.Update(delta);

            // TODO: тестирование анимации
            //////anim.update(delta);

            // Проверить актуальность объектов (убрать со сцены уничтоженные объекты)
            for (int i = gd.world.objects.Count - 1; i >= 0; i--)
            {
                if (gd.world.objects[i].IsDestroyed())
                {
                    // Удалить из "мира"
                    gd.world.objects.RemoveAt(i);
                }
            }

            saveTickCount = tickCount;

            Invalidate(false);
        }

        ///////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Обработка нажатых клавиш
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        ///////////////////////////////////////////////////////////////////////
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // Вызвать обработчик нажатий клавиш текущей сцены
            gd.curScene.KeyDown(sender, e);
        }

        ///////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Обработка отпущенных клавиш
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        ///////////////////////////////////////////////////////////////////////
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            // Вызвать обработчик отпусканий клавиш текущей сцены
            gd.curScene.KeyUp(sender, e);
        }

        ///////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Обработка событий перерисовки содержимого окна
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        ///////////////////////////////////////////////////////////////////////
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;


            // Вывести фоновое изображение, если оно есть
            if (gd.backgroundImage != null)
            {
                g.DrawImage(gd.backgroundImage, 0.0f, 0.0f);
            }

            // Вызвать метод отображения текущей сцены
            gd.curScene.Render(g);
            
        }

        ///////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Обработка событий нажатия клавиши мыши
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        ///////////////////////////////////////////////////////////////////////
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            // Левая кнопка
            if (e.Button == MouseButtons.Left)
            {
                foreach (Entity ent in gd.world.objects)
                {
                    ent.OnLeftMouseButtonClick(e);
                }
            }

            // Правая кнопка
            if (e.Button == MouseButtons.Right)
            {
                foreach (Entity ent in gd.world.objects)
                {
                    ent.OnRightMouseButtonClick(e);
                }
            }
        }

        
    }
}
