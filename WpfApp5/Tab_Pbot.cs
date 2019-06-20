using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Forms = System.Windows.Forms;


using Drawing = System.Drawing;
using System.Diagnostics;

namespace WpfApp5
{
    public partial class MainWindow : Window
    {
        class Pbot_Macro
        {
            public int Delay { get; set; }
            public Thread KeyThread { get; set; }
            public VKeys Macrokey { get; set; }
            public Key SSkey { get; set; }
            public Process proc { get; set; }
            public bool IsWorking { get; set; }
        }

        private List<Pbot_Macro> Pbot_macroList = new List<Pbot_Macro>();
        private List<ComboBox> Pbot_ComboBoxlist_ss = new List<ComboBox>();
        private List<ComboBox> Pbot_ComboBoxlist_id = new List<ComboBox>();
        private List<ComboBox> Pbot_ComboBoxlist_macrokey = new List<ComboBox>();
        private List<TextBox> Pbot_Textboxlist_ms = new List<TextBox>();
        private List<Rectangle> Pbot_Rectanglelist = new List<Rectangle>();
        private List<HotKey> Pbot_HotkeyList = new List<HotKey>();

        private Thread maple_cThread = null;
        private const int MaxMacroCount = 4;
        private bool botStarted = false;
        private int beforeMapleProcessCount = 0;
        private int afterMapleProcessCount = 0;

        private void InitializePbotComponent()
        {
            for ( int i = 0 ; i < MaxMacroCount ; i++ )
            {
                var m = new Pbot_Macro();
                Pbot_macroList.Add(m);
            }
            Pbot_StartMapleCheckThread();
            Pbot_ComboBoxInitialize();
            
            for (int i=0 ; i < 9 ; i++ )
                Pbot_HotkeyList.Add(new HotKey((Key)(Key.F1 + i), KeyModifier.None, OnHotKeyHandler));
        }

        private void OnHotKeyHandler(HotKey obj)
        {
            this.Dispatcher.Invoke(() =>
            {
                for (int i=0 ; i< Pbot_macroList.Count ; i++ )
                {
                    
                    if ( Pbot_macroList[i].Macrokey == VKeys.None ) continue;
                    if ( Pbot_macroList[i].SSkey == Key.None ) { WriteLog("Start/Stop key not selected", i + 1); continue; }
                    if ( Pbot_macroList[i].proc == null ) { WriteLog("Process not found", i + 1); continue; }
                    if ( Pbot_macroList[i].Delay < 10 ) { WriteLog("Delay should be at least 10 ms", i+1); continue; }
                    
                    if ( Pbot_macroList[i].SSkey == obj.Key )
                    {
                        if ( Pbot_macroList[i].IsWorking )
                            StopMacro(i);
                        else
                            StartMacro(i);
                    }
                }
            });
        }

        private void StopMacro(int i)
        {
            Pbot_macroList[i].IsWorking = false;
            WriteLog("Pause", i + 1);
            if (Pbot_macroList[i].KeyThread != null)
                Pbot_macroList[i].KeyThread.Abort();
            Pbot_Rectanglelist[i].Fill = new SolidColorBrush(Color.FromRgb(230, 0, 0));
        }

        private void StartMacro(int i)
        {
            Pbot_macroList[i].IsWorking = true;
            WriteLog("Start", i + 1);

            Action<int> action = new Action<int>((ind) =>
            {
                try
                {
                    var handle = Pbot_macroList[ind].proc.MainWindowHandle;
                    while (true)
                    {
                        if (Pbot_macroList[ind].Macrokey >= VKeys.ArrowLeft && Pbot_macroList[ind].Macrokey <= VKeys.ArrowDown)
                        {
                            Import.PostMessage(handle, (int)WMessages.WM_KEYDOWN, (int)Pbot_macroList[ind].Macrokey, new IntPtr(Import.MapVirtualKey((int)Pbot_macroList[ind].Macrokey, 0) << 16));
                            Thread.Sleep(Pbot_macroList[ind].Delay);
                            Import.PostMessage(handle, (int)WMessages.WM_KEYUP, (int)Pbot_macroList[ind].Macrokey, new IntPtr(Import.MapVirtualKey((int)Pbot_macroList[ind].Macrokey, 0) << 16));
                        }
                        else
                        {
                            Import.PostMessage(handle, (int)WMessages.WM_KEYDOWN, (int)Pbot_macroList[ind].Macrokey, new IntPtr(Import.MapVirtualKey((int)Pbot_macroList[ind].Macrokey, 0) << 16));
                            Import.PostMessage(handle, (int)WMessages.WM_KEYUP, (int)Pbot_macroList[ind].Macrokey, new IntPtr(Import.MapVirtualKey((int)Pbot_macroList[ind].Macrokey, 0) << 16));
                            Thread.Sleep(Pbot_macroList[ind].Delay);
                        }
                    }
                }
                catch (Exception excep) { /*WriteLog("쓰레드 오류발생 " + excep.Message, i + 1);*/ }
            });
            Action<int> StartMacro = new Action<int>((ind) =>
            {
                Pbot_macroList[ind].KeyThread = new Thread(() => action(ind));
                Pbot_macroList[ind].KeyThread.Start();
            });
            StartMacro(i);
            Pbot_Rectanglelist[i].Fill = new SolidColorBrush(Color.FromRgb(0, 230, 0));
        }



        #region 콤보박스 초기화
        private void Pbot_ComboBoxInitialize()
        {
            Pbot_ComboBoxlist_ss.Add(ComboBox_SSkey);
            Pbot_ComboBoxlist_macrokey.Add(ComboBox_KeyList);
            Pbot_ComboBoxlist_id.Add(ComboBox_Procid);
            Pbot_Rectanglelist.Add(Rectangle_Status);
            Pbot_Textboxlist_ms.Add(TextBox_ms);

            Pbot_ComboBoxlist_ss.Add(ComboBox_SSkey2);
            Pbot_ComboBoxlist_macrokey.Add(ComboBox_KeyList2);
            Pbot_ComboBoxlist_id.Add(ComboBox_Procid2);
            Pbot_Rectanglelist.Add(Rectangle_Status2);
            Pbot_Textboxlist_ms.Add(TextBox_ms2);

            Pbot_ComboBoxlist_ss.Add(ComboBox_SSkey3);
            Pbot_ComboBoxlist_macrokey.Add(ComboBox_KeyList3);
            Pbot_ComboBoxlist_id.Add(ComboBox_Procid3);
            Pbot_Rectanglelist.Add(Rectangle_Status3);
            Pbot_Textboxlist_ms.Add(TextBox_ms3);

            Pbot_ComboBoxlist_ss.Add(ComboBox_SSkey4);
            Pbot_ComboBoxlist_macrokey.Add(ComboBox_KeyList4);
            Pbot_ComboBoxlist_id.Add(ComboBox_Procid4);
            Pbot_Rectanglelist.Add(Rectangle_Status4);
            Pbot_Textboxlist_ms.Add(TextBox_ms4);


            for (int i=0 ; i < Pbot_ComboBoxlist_macrokey.Count ; i++ )
            {

                //Pbot_ComboBoxlist_macrokey[i].Items.Add("Ctrl");
                //Pbot_ComboBoxlist_macrokey[i].Items.Add("Space");
                //Pbot_ComboBoxlist_macrokey[i].Items.Add("Insert");
                //Pbot_ComboBoxlist_macrokey[i].Items.Add("Delete");
                //Pbot_ComboBoxlist_macrokey[i].Items.Add("Home");
                //Pbot_ComboBoxlist_macrokey[i].Items.Add("End");
                //Pbot_ComboBoxlist_macrokey[i].Items.Add("PageUp");
                //Pbot_ComboBoxlist_macrokey[i].Items.Add("PageDown");
                //for (int j=65 ; j <= 90 ; j++ )
                //    Pbot_ComboBoxlist_macrokey[i].Items.Add(((char)j).ToString());

                Enum.GetValues(typeof(VKeys))
                .Cast<VKeys>()
                .Select(v => v.ToString())
                .ToList().
                ForEach(  x =>  Pbot_ComboBoxlist_macrokey[i].Items.Add(x = x.Replace("_", string.Empty)) );
                Pbot_ComboBoxlist_macrokey[i].SelectedIndex = 0;
            }

            for ( int i = 0 ; i < Pbot_ComboBoxlist_ss.Count ; i++ )
            {
                Pbot_ComboBoxlist_ss[i].Items.Add("<Select Key>");
                Pbot_ComboBoxlist_ss[i].Items.Add("F1");
                Pbot_ComboBoxlist_ss[i].Items.Add("F2");
                Pbot_ComboBoxlist_ss[i].Items.Add("F3");
                Pbot_ComboBoxlist_ss[i].Items.Add("F4");
                Pbot_ComboBoxlist_ss[i].Items.Add("F5");
                Pbot_ComboBoxlist_ss[i].Items.Add("F6");
                Pbot_ComboBoxlist_ss[i].Items.Add("F7");
                Pbot_ComboBoxlist_ss[i].Items.Add("F8");
                Pbot_ComboBoxlist_ss[i].Items.Add("F9");
                Pbot_ComboBoxlist_ss[i].SelectedIndex = 0;
            }

            Pbot_ComboBox_ProcidUpdate();
        }
        #endregion

        #region Process ID 리스트 업데이트 버튼클릭
        private void Button_ProcidUpdate_Click(object sender, RoutedEventArgs e)
        {
            for ( int i = 0 ; i < Pbot_macroList.Count ; i++ )
            {
                if ( Pbot_macroList[i].IsWorking )
                {
                    WriteLog("<Fail> There should be no working macros");
                    return;
                }
            }
            Pbot_ComboBox_ProcidUpdate();
        }
        #endregion

        #region Process ID 콤보박스 리스트들 업데이트
        private void Pbot_ComboBox_ProcidUpdate()
        {
            var list = Pbot_GetMaplestroyProcs();

            for (int i = 0; i < Pbot_ComboBoxlist_id.Count; i++)
            {
                Pbot_ComboBoxlist_id[i].Items.Clear();
                Pbot_ComboBoxlist_id[i].Items.Add("<Select PID>");
                for (int j = 0; j < list.Count; j++)
                    Pbot_ComboBoxlist_id[i].Items.Add(list[j].Id.ToString());

                Pbot_ComboBoxlist_id[i].SelectedIndex = 0;
            }
        }
        #endregion

        #region SS Key Selection
        private void ComboBox_SSkey_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tb = sender as ComboBox;
            if ( tb is null ) return;
            int index = -1;
            for ( int i = 0 ; i < Pbot_ComboBoxlist_ss.Count ; i++ )
            {
                if ( tb == Pbot_ComboBoxlist_ss[i] )
                    index = i;
            }
            if ( index == -1 ) return;
            if ( tb.SelectedIndex == 0 )
            {
                Pbot_macroList[index].SSkey = Key.None;
                return;
            }
            Pbot_macroList[index].SSkey = (Key)(Key.F1 - 1 + tb.SelectedIndex);
        }
        #endregion

        #region SS Key DropDownOpened
        private void ComboBox_SSkey_DropDownOpened_1(object sender, EventArgs e)
        {
            var tb = sender as ComboBox;
            if ( tb is null ) return;
            int index = -1;
            for ( int i = 0 ; i < Pbot_ComboBoxlist_ss.Count ; i++ )
            {
                if ( tb == Pbot_ComboBoxlist_ss[i] )
                    index = i;
            }
            if ( index == -1 ) return;

            #region 작동 여부
            if ( Pbot_macroList[index].IsWorking )
            {
                WriteLog("<Fail> The macro is currently in use. It can be modified after stopping", (index + 1));
                tb.IsDropDownOpen = false;
                return;
            }
            #endregion
        }
        #endregion

        #region ProcID Selection
        private void ComboBox_Procid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tb = sender as ComboBox;
            if ( tb is null ) return;
            int index = -1;
            for ( int i = 0 ; i < Pbot_ComboBoxlist_id.Count ; i++ )
            {
                if ( tb == Pbot_ComboBoxlist_id[i] )
                    index = i;
            }
            if ( index == -1 ) return;
            if ( tb.SelectedIndex == 0 || tb.SelectedIndex == -1)
            {
                tb.SelectedIndex = 0;
                return;
            }
            string sItem = ( string )Pbot_ComboBoxlist_id[index].SelectedItem;
            int.TryParse(sItem, out int Pid);
            if ( Pid == 0 )
            {
                Pbot_macroList[index].proc = null;
                WriteLog("<Fail> The macro process ID parsing failed", (index + 1));
                return;
            }

            
            Process proc = null;
            try
            {
                proc = Process.GetProcessById(Pid);
            } catch { WriteLog("<Fail> Need to update process id"); }
            Pbot_macroList[index].proc = proc;
        }
        #endregion

        #region ProcID DropDownOpened
        private void ComboBox_Procid_DropDownOpened(object sender, EventArgs e)
        {
            var tb = sender as ComboBox;
            if ( tb is null ) return;
            int index = -1;
            for ( int i = 0 ; i < Pbot_ComboBoxlist_id.Count ; i++ )
            {
                if ( tb == Pbot_ComboBoxlist_id[i] )
                    index = i;
            }
            if ( index == -1 ) return;

            #region 작동 여부
            if ( Pbot_macroList[index].IsWorking )
            {
                WriteLog((index + 1) + "번째 매크로는 현재 사용중입니다. 중지 후 수정가능");
                tb.IsDropDownOpen = false;
                return;
            }
            #endregion
        }
        #endregion

        #region  KeyList Selection
        private void ComboBox_KeyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tb = sender as ComboBox;
            if ( tb is null ) return;
            int index = -1;
            for ( int i = 0 ; i < Pbot_ComboBoxlist_macrokey.Count ; i++ )
            {
                if ( tb == Pbot_ComboBoxlist_macrokey[i] )
                    index = i;
            }
            if ( index == -1 ) return;
            if (tb.SelectedIndex == 0) return;

            string keyString = tb.SelectedItem.ToString();
            char[] keyCharArray = keyString.ToCharArray();

            if (keyCharArray[0] >= 48 && keyCharArray[0] <= 57)
                keyString = keyString.Insert(0, "_");

            Pbot_macroList[index].Macrokey = (VKeys)Enum.Parse(typeof(VKeys), keyString);
            int a = 20;
        }
        #endregion

        #region KeyList DropDownOpened
        private void ComboBox_KeyList_DropDownOpened(object sender, EventArgs e)
        {
            var tb = sender as ComboBox;
            if ( tb is null ) return;
            int index = -1;
            for ( int i = 0 ; i < Pbot_ComboBoxlist_macrokey.Count ; i++ )
            {
                if ( tb == Pbot_ComboBoxlist_macrokey[i] )
                    index = i;
            }
            if ( index == -1 ) return;

            #region 작동 여부
            if ( Pbot_macroList[index].IsWorking )
            {
                WriteLog("The macro is currently in use. It can be modified after stopping", (index + 1));
                tb.IsDropDownOpen = false;
                return;
            }
            #endregion
        }
        #endregion

        #region ms TextBox Changed
        private void TextBox_ms_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;
            if ( tb is null ) return;
            int index = -1;
            for ( int i = 0 ; i <Pbot_Textboxlist_ms.Count ; i++ )
            {
                if ( tb == Pbot_Textboxlist_ms[i] )
                    index = i;
            }
            if ( index == -1 ) return;

            int.TryParse(tb.Text, out int num);
            Pbot_macroList[index].Delay = num;
        }
        #endregion

        #region 메이플스토리 프로세스 얻기
        public List<Process> Pbot_GetMaplestroyProcs()
        {
            List<Process> mapleInfo = new List<Process>();

            foreach ( var proc in Process.GetProcesses() )
            {
                if ( proc.ProcessName == "MapleStory" )
                    mapleInfo.Add(proc);
            }

            return mapleInfo;
        }
        #endregion

        #region 메이플 런처 쓰레드
        private void Pbot_StartMapleCheckThread()
        {
            if ( maple_cThread is null )
            {
                maple_cThread = new Thread(() =>
                {
                    try
                    {
                        while ( true )
                        {
                            try
                            {
                                var list = Pbot_GetMaplestroyProcs();
                                if (botStarted == false )
                                {
                                    beforeMapleProcessCount = list.Count;
                                    botStarted = true;
                                }
                                afterMapleProcessCount = list.Count;

                                if ( afterMapleProcessCount > beforeMapleProcessCount )
                                {
                                    beforeMapleProcessCount = afterMapleProcessCount;
                                    WriteLog("MapleStory.exe execution detected. need to update process id");
                                }
                                else if ( afterMapleProcessCount < beforeMapleProcessCount )
                                {
                                    beforeMapleProcessCount = afterMapleProcessCount;
                                    WriteLog("MapleStory.exe termination detected. need to update process id");
                                }

                                IntPtr hwnd = Import.GetForegroundWindow();
                                Import.GetWindowThreadProcessId(hwnd, out uint pid);
                                Process p = Process.GetProcessById(( int )pid);
                                this.Dispatcher.Invoke(() =>
                                {
                                    if ( list.Count > 0 )
                                    {
                                        TextBlock_MapleCount.Text = list.Count.ToString() + " Active";
                                        TextBlock_MapleCount.Foreground = new SolidColorBrush(Color.FromRgb(0, 230, 0));
                                    }
                                    else
                                    {
                                        TextBlock_MapleCount.Text = "Inactive";
                                        TextBlock_MapleCount.Foreground = new SolidColorBrush(Color.FromRgb(230, 0, 0));
                                    }

                                    if ( p.ProcessName == "MapleStory" )
                                    {
                  
                                        TextBlock_MapleForeground.Text = p.Id.ToString();
                                        TextBlock_MapleForeground.Foreground = new SolidColorBrush(Color.FromRgb(0, 230, 0));
                                    }
                                    else
                                    {
                                        TextBlock_MapleForeground.Text = "Not found";
                                        TextBlock_MapleForeground.Foreground = new SolidColorBrush(Color.FromRgb(230, 0, 0));
                                    }
                                });
                                Thread.Sleep(100);
                            }
                            catch { }
                        }
                    }
                    catch { }
                });
                maple_cThread.Start();
            }
        }
        #endregion

        #region 숫자만 입력
        private void TextBox_ms_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }
        #endregion

        #region 타이틀 변경 버튼 클릭
        private void Button_ChangeTitle_Click(object sender, RoutedEventArgs e)
        {
            if ( Pbot_changeTitleWindow == null )
            {
                Pbot_changeTitleWindow = new ChangeTitleWindow(this.Title);
                if ( Pbot_changeTitleWindow.ShowDialog() == true )
                {
                    this.Dispatcher.Invoke(() => this.Title = ChangeTitleWindow.ChangedTitle);
                    TitleSave();
                }
            }
        }
        #endregion
    }
}


//var _hotKey2 = new HotKey(Key.F2, KeyModifier.Shift/* | KeyModifier.Win*/, OnHotKeyHandler);