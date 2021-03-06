using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CMapleVot
{
    /// <summary>
    /// Window1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CMapleInfoWindow : Window
    {
        public CMapleInfoWindow()
        {
            InitializeComponent();
            LocateWindowCenter();
            this.Topmost = true;
        }

        #region 중앙 위치
        private void LocateWindowCenter()
        {
            this.Top = CMapleMainWindow.window.Top + CMapleMainWindow.window.Height / 2 - this.Height / 2;
            this.Left = CMapleMainWindow.window.Left + CMapleMainWindow.window.Width / 2 - this.Width / 2;
        }
        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CMapleMainWindow.windowInfo = null;
        }
    }
}
