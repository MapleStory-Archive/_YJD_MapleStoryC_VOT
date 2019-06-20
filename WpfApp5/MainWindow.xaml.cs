using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Security.Principal;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;

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
            LoadSettings();
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
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
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
                for (int i=0 ; i< Pbot_HotkeyList.Count ; i++ )
                    Pbot_HotkeyList[i].Unregister();
                Handling_hotkey.Unregister();
                SaveSettings();
                detect_pThread.Abort();
                detect_cThread.Abort();

            }
            catch { }
        }

        private void LoadSettings()
        {
            if (File.Exists("setting.json") /*&& MessageBox.Show("저장된 데이터가 있습니다.\n불러오시겠습니까?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes*/)
            {
                try
                {
                    JObject root = JObject.Parse(File.ReadAllText("setting.json"));
                    JArray macroArray = root["macroarray"] as JArray;

                    for (int i = 0; i < macroArray.Count; i++)
                    {
                        JObject macroObject = macroArray[i] as JObject;
                        Pbot_ComboBoxlist_macrokey[i].SelectedIndex = int.Parse(macroObject["mk"].ToString());
                        Pbot_Textboxlist_ms[i].Text = macroObject["ms"].ToString();
                        Pbot_ComboBoxlist_ss[i].SelectedIndex = int.Parse(macroObject["ssk"].ToString());
                    }

                    Pbot_ComboBox_ProcidUpdate();
                    LoadMapleProcess();
                }
                catch(Exception e)
                {
                    MessageBox.Show("저장된 데이터를 불러오는데 실패하였습니다", "", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                }
            }
        }

        private void LoadMapleProcess()
        {
            
            var list = Pbot_GetMaplestroyProcs();
            list.Sort((proc1, proc2) =>  proc1.StartTime.CompareTo(proc2.StartTime));

            for (int i = 0; i < list.Count; i++)
            {
                if (i > 4) continue;

                if (Pbot_ComboBoxlist_id[i].Items.Count > 1)
                {
                    for (int j = 1; j < Pbot_ComboBoxlist_id[i].Items.Count; j++)
                    {
                        if (Pbot_ComboBoxlist_id[i].Items[j].ToString() == list[i].Id.ToString())
                        {
                            Pbot_ComboBoxlist_id[i].SelectedIndex = i + 1;
                            break;
                        }
                    }
                }
            }
        }

        private void SaveSettings()
        {
            List<bool> setMacroList = new List<bool>() { false, false, false, false };
            for (int i = 0; i < 4; i++)
            {
                if (Pbot_ComboBoxlist_ss[i].SelectedIndex == 0 || Pbot_ComboBoxlist_ss[i].SelectedIndex == -1)
                    continue;
                if (Pbot_ComboBoxlist_macrokey[i].SelectedIndex == 0 || Pbot_ComboBoxlist_macrokey[i].SelectedIndex == -1)
                    continue;

                int.TryParse(Pbot_Textboxlist_ms[i].Text.ToString(), out int ret);

                if (ret <= 0)
                    continue;

                setMacroList[i] = true;
            }

            if (setMacroList.Find(x => x == true) == false) //세팅된 저장값이 발견되지 않을 경우
                return;

            JObject root = new JObject();
            JArray macroArray = new JArray();
            for (int i = 0; i < setMacroList.Count; i++)
            {
                JObject macroObject = new JObject();
                
                macroObject.Add("mk", Pbot_ComboBoxlist_macrokey[i].SelectedIndex);
                int.TryParse(Pbot_Textboxlist_ms[i].Text.ToString(), out int ret);
                macroObject.Add("ms", ret);
                macroObject.Add("ssk", Pbot_ComboBoxlist_ss[i].SelectedIndex);
                macroArray.Add(macroObject);
            }
            root.Add("macroarray", macroArray);
            File.WriteAllText("setting.json", root.ToString());
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
