namespace GameEngineStage6.Utils
{
    /// <summary>
    /// Класс для ведения журнала событий
    /// </summary>
    class Logger
    {
        /// <summary>
        /// Файл журнала
        /// </summary>
        private System.IO.StreamWriter file;

        /// <summary>
        /// Конструктор с параметром
        /// </summary>
        /// <param name="filename">имя файла для сохранения журнала</param>
        public Logger(string filename)
        {
            file = new System.IO.StreamWriter(filename);
        }

        /// <summary>
        /// Записать данную строку в журнал
        /// </summary>
        /// <param name="text">строка для записи в журнал</param>
        public void Write(string text)
        {
            if (file != null)
            {
                file.WriteLine(text);
                file.Flush();
            }
        }

        /// <summary>
        /// Закрыть файл журнала
        /// </summary>
        public void Close()
        {
            if (file != null)
            {
                file.Close();
            }
        }

        public void Flush()
        {
            if (file != null)
            {
                file.Flush();
            }
        }
    }
}
