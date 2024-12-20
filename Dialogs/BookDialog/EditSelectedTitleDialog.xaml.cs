﻿using SQLLab2.ViewModel;
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

namespace SQLLab2.Dialogs
{
    /// <summary>
    /// Interaction logic for EditSelectedTitleDialog.xaml
    /// </summary>
    public partial class EditSelectedTitleDialog : Window
    {
        public EditSelectedTitleDialog()
        {
            DataContext = new EditSelectedTitleViewModel((App.Current.MainWindow as MainWindow).DataContext as MainWindowViewModel, false);
            InitializeComponent();
        }
        public void CloseDialog(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
