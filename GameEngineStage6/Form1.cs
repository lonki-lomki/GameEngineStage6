using GameEngineStage6.Core;
using System;
using System.Windows.Forms;

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
        }
    }
}
