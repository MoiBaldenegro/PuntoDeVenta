using System.Windows;
using Tomate.Views;

namespace Tomate.Utils
{
    public class WindowUtils
    {
        public static MainWindow getMainWindow()
        {
            return ((MainWindow)Application.Current.MainWindow);
        }
    }
}
