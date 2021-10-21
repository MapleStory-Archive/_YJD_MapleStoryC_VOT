using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CMapleVot
{
    public partial class CMapleMainWindow : Window
    {
        enum DetectStatus : int
        {
            Pause,
            Run,
            Lock
        }

        public static CMapleChangeTitleWindow Handling_changeTitleWindow = null;
        private Thread detect_pThread = null;
        private Thread detect_cThread = null;
        private Process detect_process = null;
        private DetectStatus detectStatus = DetectStatus.Pause;
        private object _locker = new object();

        #region InitializeHandlingComponent
        private void InitializeHandlingComponent()
        {
            detect_pThread = new Thread(() =>
            {
                try
                {
                    while ( true )
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            lock ( _locker )
                            {
                                if ( detect_process != null )
                                {
                                    Handling_TextBlock_Helper.Visibility = Visibility.Visible;
                                    Handling_Button_Kill.IsEnabled = true;
                                    Handling_Button_Suspend.IsEnabled = true;
                                    Handling_Button_ChangeTitle.IsEnabled = true;

                                    Handling_TextBlock_ProcessID.Text = "Process ID : " + detect_process.Id.ToString();
                                    try
                                    {
                                        Handling_TextBlock_MainModuleName.Text = "Process MainModuleName : " + detect_process.MainModule.ModuleName;
                                    }catch { Handling_TextBlock_MainModuleName.Text = "Process MainModuleName : " + "Not Found"; }
                                Handling_TextBlock_ProcessTitleName.Text = "Process Title : " + detect_process.MainWindowTitle;
                                    Handling_TextBlock_ProcessName.Text = "Process Name : " + detect_process.ProcessName;
                                    Rect rect = new Rect();
                                    CMapleImport.GetWindowRect(detect_process.MainWindowHandle, ref rect);
                                    Handling_TextBlock_XPos.Text = "Position X : " + rect.Left.ToString();
                                    Handling_TextBlock_YPos.Text = "Position Y : " + rect.Top.ToString();

                                }
                                else
                                {
                                    Handling_TextBlock_Helper.Visibility = Visibility.Hidden;
                                    Handling_Button_Kill.IsEnabled = false;
                                    Handling_Button_Suspend.IsEnabled = false;
                                    Handling_Button_ChangeTitle.IsEnabled = false;
                                }
                            }
                        });
                        Thread.Sleep(100);
                    }
                }
                catch { }
            });
            detect_pThread.Start();
            UpdateDetectionStatus();
        }
        #endregion

        private void OnProcessHandlingHotkeyPressed()
        {
            if ( detectStatus == DetectStatus.Pause )
                return;

            else if ( detectStatus == DetectStatus.Run )
            {
                detectStatus = DetectStatus.Lock;
                UpdateDetectionStatus();
            }

            else if ( detectStatus == DetectStatus.Lock )
            {
                detectStatus = DetectStatus.Run;
                UpdateDetectionStatus();
            }
        }


        private void Handling_Button_Start_Click(object sender, RoutedEventArgs e)
        {
            if ( detectStatus == DetectStatus.Pause)
            {
                Handling_StartMapleCheckThread();
                WriteLog("Detection started");
                detectStatus = DetectStatus.Run;
                UpdateDetectionStatus();
                Handling_Button_Start.Content = "Stop";
            }
            else if ( detectStatus == DetectStatus.Run || detectStatus == DetectStatus.Lock)
            {
                if ( detect_cThread != null)
                {
                    detect_cThread.Abort();
                    detect_cThread = null;
                }
                
                detect_process = null;
                WriteLog("Detection paused");
                detectStatus = DetectStatus.Pause;
                UpdateDetectionStatus();
                Handling_Button_Start.Content = "Detect Start";
            }
        }

        private void UpdateDetectionStatus()
        {
            Handling_TextBlock_DetectStatus.Dispatcher.Invoke(() =>
            {
                if ( detectStatus == DetectStatus.Pause )
                {
                    Handling_TextBlock_DetectStatus.Text = "Pause";
                    Handling_TextBlock_DetectStatus.Foreground = new SolidColorBrush(Color.FromArgb(255, 230, 0, 0));
                }
                else if ( detectStatus == DetectStatus.Run )
                {
                    Handling_TextBlock_DetectStatus.Text = "Running";
                    Handling_TextBlock_DetectStatus.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 230, 0));
                }
                else if ( detectStatus == DetectStatus.Lock )
                {
                    Handling_TextBlock_DetectStatus.Text = "Lock";
                    Handling_TextBlock_DetectStatus.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 230));
                }
            });
        }

        private void Handling_Button_Kill_Click(object sender, RoutedEventArgs e)
        {
            if (detect_process is null)
            {
                WriteLog("<Fail> Process is nullptr");
                return;
            }

            detect_process.Kill();
            WriteLog("<Success> The process has been terminated");
            detect_process = null;
        }

        private void Handling_Button_ChangeTitle_Click(object sender, RoutedEventArgs e)
        {
            if ( detect_process is null )
            {
                WriteLog("<Fail> Process is nullptr");
                return;
            }

            if ( Handling_changeTitleWindow == null )
            {
                Handling_changeTitleWindow = new CMapleChangeTitleWindow(detect_process.MainWindowTitle);
                if ( Handling_changeTitleWindow.ShowDialog() == true )
                {
                    if ( detect_process is null )
                    {
                        WriteLog("<Fail> Process is nullptr");
                        return;
                    }
                    CMapleImport.SetWindowText(detect_process.MainWindowHandle, CMapleChangeTitleWindow.ChangedTitle);
                }
            }
        }

        private void Handling_Button_Suspend_Click(object sender, RoutedEventArgs e)
        {
            if ( detect_process is null )
            {
                WriteLog("<Fail> Process is nullptr");
                return;
            }

            foreach ( ProcessThread thread in detect_process.Threads )
            {
                var pOpenThread = CMapleImport.OpenThread(CMapleImport.ThreadAccess.SUSPEND_RESUME, false, ( uint )thread.Id);
                if ( pOpenThread == IntPtr.Zero )
                    break;
                CMapleImport.SuspendThread(pOpenThread);
            }
            WriteLog("<Success> The process has been suspended");
        }

        #region 메이플 런처 쓰레드
        private void Handling_StartMapleCheckThread()
        {
            if ( detect_cThread is null )
            {
                detect_cThread = new Thread(() =>
                {
                    try
                    {
                        while ( true )
                        {
                            try
                            {
                                if ( detectStatus != DetectStatus.Lock )
                                {
                                    IntPtr hwnd = CMapleImport.GetForegroundWindow();
                                    CMapleImport.GetWindowThreadProcessId(hwnd, out uint pid);
                                    Process p = Process.GetProcessById(( int )pid);
                                    detect_process = p;
                                }
                                Thread.Sleep(100);
                            }
                            catch { }
                        }
                    }
                    catch { }
                });
                detect_cThread.Start();
            }
        }
        #endregion
    }
}
