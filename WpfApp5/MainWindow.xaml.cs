using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Security.Principal;
using System.IO;

namespace WpfApp5
{

    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow window = null;
        public static Window1 windowInfo = null;
        public static ChangeTitleWindow Pbot_changeTitleWindow = null;

        public MainWindow()
        {
            PriviliegeChecking();
            InitializeComponent();
            TitleGet();
            InitializePbotComponent();
            InitializeHandlingComponent();
            window = this;
        }

        private void TitleGet()
        {
            if ( File.Exists("title.txt") == false )
                return;

            string text = File.ReadAllText("title.txt");
            if (text.Length>40)
            {
                MessageBox.Show("타이틀은 40자 이하여야합니다.");
                return;
            }
            else if (text.Length <= 0)
                return;
            ChangeTitle(text);
        }

        private void TitleSave()
        {
            this.Dispatcher.Invoke(() => File.WriteAllText("title.txt", this.Title));
        }

        private void ChangeTitle(string text)
        {
            this.Dispatcher.Invoke(() => this.Title = text);
        }

        #region 관리자권한 체크
        private void PriviliegeChecking()
        {
            bool isElevated = false;
            using ( WindowsIdentity identity = WindowsIdentity.GetCurrent() )
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }

            if (isElevated == false)
            {
                MessageBox.Show("관리자 권한으로 실행해주세요");
                Application.Current.Shutdown();
            }
        }
        #endregion

        #region 중앙 위치
        private void LocateCenter()
        {
            this.Top = ProgramStaticVariables.MAX_WINDOW_HEIGHT / 2 - this.Height / 2;
            this.Left = ProgramStaticVariables.MAX_WINDOW_WIDTH / 2 - this.Width / 2;
        }
        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                for (int i=0 ; i< Pbot_macroList.Count ; i++ )
                {
                    if ( Pbot_macroList[i].KeyThread != null)
                        Pbot_macroList[i].KeyThread.Abort();
                }
                maple_cThread.Abort();
                detect_pThread.Abort();
                detect_cThread.Abort();
                for (int i=0 ; i< Pbot_HotkeyList.Count ; i++ )
                    Pbot_HotkeyList[i].Unregister();
                Handling_hotkey.Unregister();
            }
            catch { }
        }

        #region 로그
        private void WriteLog(string Log, int MacroNum = 0)
        {
            ListView_Log.Dispatcher.Invoke(new Action(() =>
            {
                if ( ListView_Log.Items.Count > 100 )
                    ListView_Log.Items.RemoveAt(0);

                ListViewItem item = new ListViewItem();
                if ( Log.Contains("<Fail>") )
                    item.Background = new SolidColorBrush(Color.FromArgb(60, 230, 0, 0));
                else if ( Log.Contains("<Success>") )
                    item.Background = new SolidColorBrush(Color.FromArgb(60, 0, 230, 0));
                if ( MacroNum >= 1 )
                {
                    item.Content = "[" + MacroNum + "] " + Log;
                    ListView_Log.Items.Add(item);
                }
                else
                {
                    item.Content = Log;
                    ListView_Log.Items.Add(item);
                }


                ListView_Log.ScrollIntoView(ListView_Log.Items[ListView_Log.Items.Count - 1]);
            }));
        }



        #endregion

        private void Button_Info_Click(object sender, RoutedEventArgs e)
        {
            if ( windowInfo is null )
            {
                windowInfo = new Window1();
                windowInfo.Show();
            }
        }
    }
}
