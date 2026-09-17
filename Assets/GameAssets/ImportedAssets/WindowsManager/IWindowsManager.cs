using System;
using System.Collections.Generic;

namespace WindowsManager
{
    public interface IWindowsManager
    {
        public event Action<Window> OnNewWindowOpened;
        /// <summary>
        /// Открыть выбранное окно
        /// </summary>
        /// <param name="data">Дата окна</param>
        /// <param name="closeLastWindow"> Нужно ли закрывать предыдущее окно </param>
        public Window OpenWindow(WindowData data, bool closeLastWindow = false);

        /// <summary>
        /// Открыть предыдущее окно
        /// </summary>
        /// <param name="closeLastWindow"> Нужно ли закрывать выбранное окно </param>
        public void OpenPreviousWindow(bool closeLastWindow = true);

        /// <summary>
        /// Закрыть выбранное окно
        /// </summary>
        /// <param name="data">Дата окна</param>
        public void CloseWindow(WindowData data);

        /// <summary>
        /// Закрыть выбранные окна
        /// </summary>
        /// <param name="data">Окна</param>
        public void CloseWindows(List<WindowData> windows);

        public void CloseAllWindows();
        public IEnumerable<Window> GetOpenedWindows();

        /// <summary>
        /// Получить инстанс окна по дате
        /// </summary>
        /// <param name="data">Дата окна</param>
        public Window GetWindowByData(WindowData data);

        /// <summary>
        /// Добавить окно в список открытых окон
        /// </summary>
        /// <param name="window"> Окно </param>
        public void AddWindowToList(Window window);

        /// <summary>
        /// Удалить окно из списка открытых окон
        /// </summary>
        /// <param name="window"> Окно </param>
        public void RemoveWindowFromList(Window window);
    }
}