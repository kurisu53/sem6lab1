using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using ClassLibraryV3;

namespace AppV3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        V3MainCollection MCollection = new V3MainCollection();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = MCollection;
        }

        // пункты меню File

        private void New_Click(object sender, RoutedEventArgs e)
        {
            if (MCollection.WasChanged)
            {
                UnsavedData();
            }
            MCollection = new V3MainCollection();
            DataContext = MCollection;
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MCollection.WasChanged)
                {
                    UnsavedData();
                }
                OpenFileDialog OpenDialog = new OpenFileDialog();
                if ((bool)OpenDialog.ShowDialog())
                {
                    MCollection = new V3MainCollection();
                    MCollection.Load(OpenDialog.FileName);
                    DataContext = MCollection;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Opening file failed. " + ex.Message);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog SaveDialog = new SaveFileDialog();
                if ((bool)SaveDialog.ShowDialog())
                {
                    MCollection.Save(SaveDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Saving file failed. " + ex.Message);
            }
        }

        // пункты меню Edit

        private void AddDef_Click(object sender, RoutedEventArgs e)
        {
            MCollection.AddDefaults();
            DataContext = MCollection;
        }

        private void AddDefColl_Click(object sender, RoutedEventArgs e)
        {
            MCollection.AddDefaultDataCollection();
        }

        private void AddDefGrid_Click(object sender, RoutedEventArgs e)
        {
            MCollection.AddDefaultDataOnGrid();
        }

        private void AddFromFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog OpenDialog = new OpenFileDialog();
                if ((bool)OpenDialog.ShowDialog())
                {
                    MCollection.AddElementFromFile(OpenDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Adding from file failed. " + ex.Message);
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            List<V3Data> Items = new List<V3Data>();
            Items.AddRange(listBox_Main.SelectedItems.Cast<V3Data>());
            foreach (V3Data item in Items)
            {
                MCollection.Remove(item.Measures, item.MeasureTime);
            }
        }

        // случай несохраненных изменений

        private bool UnsavedData()
        {
            MessageBoxResult UnsavedMsg = MessageBox.Show("There are unsaved changes. Do you want to save them?", "Save", MessageBoxButton.YesNoCancel);
            if (UnsavedMsg == MessageBoxResult.Yes)
            {
                SaveFileDialog SaveDialog = new SaveFileDialog();
                if ((bool)SaveDialog.ShowDialog())
                {
                    MCollection.Save(SaveDialog.FileName);
                }
            }
            else if (UnsavedMsg == MessageBoxResult.Cancel)
            {
                return true;
            }
            return false;
        }

        // фильтры

        private void FilterCollection(object sender, FilterEventArgs args)
        {
            if (args.Item != null)
            {
                if (args.Item.GetType() == typeof(V3DataCollection)) 
                    args.Accepted = true;
                else 
                    args.Accepted = false;
            }
        }

        private void FilterGrid(object sender, FilterEventArgs args)
        {
            if (args.Item != null)
            {
                if (args.Item.GetType() == typeof(V3DataOnGrid)) 
                    args.Accepted = true;
                else 
                    args.Accepted = false;
            }
        }

        // закрытие приложения

        private void OnAppClosing(object sender, CancelEventArgs e)
        {
            if (MCollection.WasChanged)
            {
                e.Cancel = UnsavedData();
            }
        }
    }
}
