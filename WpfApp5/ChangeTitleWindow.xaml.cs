﻿using System;
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

namespace WpfApp5
{
    /// <summary>
    /// ChangeTitleWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ChangeTitleWindow : Window
    {
        public static string ChangedTitle { get; set; }

        public ChangeTitleWindow(string Ctitle)
        {
            InitializeComponent();
            LocateWindowCenter();
            TextBox_Name.Text = Ctitle;
            ChangedTitle = Ctitle;
        }

        #region 중앙 위치
        private void LocateWindowCenter()
        {
            this.Top = MainWindow.window.Top + MainWindow.window.Height / 2 - this.Height / 2;
            this.Left = MainWindow.window.Left + MainWindow.window.Width / 2 - this.Width / 2;
        }
        #endregion

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if ( TextBox_Name.Text.Length <= 0 )
            {
                MessageBox.Show("입력을 해주세요");
                return;
            }
            else if ( TextBox_Name.Text.Length > 40 )
            {
                MessageBox.Show("타이틀은 40자 이하로 작성해주세요");
                return;
            }

            ChangedTitle = TextBox_Name.Text;
            this.DialogResult = true;
            this.Close();
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void TextBox_Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBlock_TitleCount.Text = TextBox_Name.Text.Length + " / " + 40;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if ( MainWindow.Pbot_changeTitleWindow  != null)
                MainWindow.Pbot_changeTitleWindow = null;
            if ( MainWindow.Handling_changeTitleWindow != null )
                MainWindow.Handling_changeTitleWindow = null;
        }
    }
}
