using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
namespace Holo2factoryConfigurator
{
    public partial class MainWindow : Window
    {
        private List<PieceCotation> CotationList { get; set; } = new List<PieceCotation>();
        private string ModelPath { get; set; }
        private bool IsSaved { get; set; } = false;
        private string CurrentDirectory { get; set; } = Directory.GetCurrentDirectory();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            AddCotation();
        }

        private void AddCotation()
        {
            ProductionWindow WindowProduction = new ProductionWindow(ModelPath);
            PieceCotation newCotation = new PieceCotation
            {
                Id = CotationList.Count()
            };
            if (WindowProduction.Execute(newCotation))
            {
                CotationList.Add(newCotation);
                RefreshListBox();
                IsSaved = false;
            }
        }
        private void RefreshListBox()
        {
            Production_ListBox.Items.Clear();
            foreach (PieceCotation cotation in CotationList)
            {
                string dumpedJson = JsonConvert.SerializeObject(cotation);
                dumpedJson = dumpedJson.Replace('{', ' ').Replace('}', ' ').Replace('"', ' ').Replace(',', '\n');
                Production_ListBox.Items.Add(dumpedJson);
            }
            if (Production_ListBox.Items.Count > 0)
            {
                ButtonSave2.IsEnabled = true;
            }
            else
            {
                ButtonSave2.IsEnabled = false;
            }
        }

        private void ButtonModify_Click(object sender, RoutedEventArgs e)
        {
            ModifyCotation();
        }

        private void ModifyCotation()
        {
            ProductionWindow WindowProduction = new ProductionWindow(ModelPath);
            PieceCotation selCotation = CotationList.ElementAt(Production_ListBox.SelectedIndex);
            if (WindowProduction.Execute(selCotation))
            {
                RefreshListBox();
                IsSaved = false;
            }
        }
        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you really want to delete this cotation ?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                int selIndex = Production_ListBox.SelectedIndex;
                CotationList.RemoveAt(selIndex);
                RemapId();
                RefreshListBox();
            }
        }

        private void ButtonSave2_Click(object sender, RoutedEventArgs e)
        {
            SavePiece();
        }
        // erase actual piece
        private void ButtonNewJson_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("This will erase the actual recipe, do you want to conitnue ?", "Clear recipe", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                ClearPiece();
                IsSaved = false;
            }
        }
        private void ButtonLoadJson_Click(object sender, RoutedEventArgs e)
        {
            if (LoadJson())
            {
                ButtonSave2.IsEnabled = true;
                IsSaved = true;
            }
            else
            {
                ButtonSave2.IsEnabled = false;
            }
            RefreshListBox();
        }

        private bool LoadJson()
        {
            const int ExtensionLength = 5;
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                FileName = "Load json piece file",
                DefaultExt = ".json",
                Filter = "Text documents (.json)|*.json"
            };
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                try
                {
                    string jsonString = File.ReadAllText(dlg.FileName);
                    CotationList = JsonConvert.DeserializeObject<List<PieceCotation>>(jsonString);
                }
                catch (Exception e)
                {
                    _ = MessageBox.Show("Unable to load Json file\nDetails: " + e.Message,
                        "Load Json", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                string filename = System.IO.Path.GetFileName(dlg.FileName);
                TextBlockPieceName.Text = filename.Substring(0, filename.Length - ExtensionLength);
                TextBlockFilename.Text = filename;
                return true;
            }
            return false;
        }

        private void Production_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Production_ListBox.SelectedItem != null)
            {
                ButtonModify.IsEnabled = true;
                ButtonDelete.IsEnabled = true;
                MenuItemModify.IsEnabled = true;
                MenuItemDelete.IsEnabled = true;
            }
            else
            {
                ButtonModify.IsEnabled = false;
                ButtonDelete.IsEnabled = false;
                MenuItemModify.IsEnabled = false;
                MenuItemDelete.IsEnabled = false;
            }
        }
        private void RemapId()
        {
            foreach (var (cotation, index) in CotationList.Select((value, i) => (value, i)))
            {
                cotation.Id = index;
            }
        }

        private void MenuItemNew_Click(object sender, RoutedEventArgs e)
        {
            ClearPiece();
        }
        private void MenuItemSave_Click(object sender, RoutedEventArgs e)
        {
            SavePiece();
        }
        private void MenuItemOpen_Click(object sender, RoutedEventArgs e)
        {
            LoadJson();
        }
        private void ButtonNew_Click(object sender, RoutedEventArgs e)
        {
            ClearPiece();
        }
        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            LoadJson();
        }
        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {

            Exit();
        }
        private void Exit()
        {
            if (!IsSaved)
            {
                MessageBoxResult result = MessageBox.Show("Do you really want to exit ? All unsave data will be lost", "Quit", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    Environment.Exit(0);
                }
            }
            else
            {
                Environment.Exit(0);
            }
        }
        private void MenuItemAdd_Click(object sender, RoutedEventArgs e)
        {
            AddCotation();
        }

        private void MenuItemModify_Click(object sender, RoutedEventArgs e)
        {
            ModifyCotation();
        }

        private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteCotation();
        }

        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            _ = MessageBox.Show("Holo2factoryConfigurator\n2022, Quentin Fornerod", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MenuItemSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SavePiece();
        }
        private void WindowMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Exit();
        }
        private void ClearPiece()
        {
            MessageBoxResult result = MessageBox.Show("Do you really want to clear the list", "Clear list", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                CotationList.Clear();
                RefreshListBox();
            }
        }
        private void DeleteCotation()
        {
            MessageBoxResult result = MessageBox.Show("Do you really want to delete this cotation ?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                int selIndex = Production_ListBox.SelectedIndex;
                CotationList.RemoveAt(selIndex);
                RemapId();
                RefreshListBox();
            }
        }
        private void SavePiece()
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "myNewPiece",
                DefaultExt = ".json",
                Filter = "json files (.json)|*.json"
            };
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                try
                {
                    string jsonString = JsonConvert.SerializeObject(CotationList);
                    File.AppendAllText(dlg.FileName, jsonString);
                    IsSaved = true;
                }
                catch (Exception e)
                {
                    _ = MessageBox.Show("Unable to save the cotation list\nDetails: " + e.Message, "Save cotation list", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void Load3DModel()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                FileName = "myModel.obj",
                DefaultExt = ".obj",
                Filter = "Obj files (.obj)|*.obj"
            };
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                TextBlockModel.Text = System.IO.Path.GetFileName(dlg.FileName);
                ModelPath = dlg.FileName;
            }
        }
        private void ButtonLoadModel_Click(object sender, RoutedEventArgs e)
        {
            Load3DModel();
        }
        private void MenuItemDocumentation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                string path = CurrentDirectory + "/Ressources/Holo2factoryConfiguratorUserGuide.pdf";
                Uri pdf = new Uri(path, UriKind.RelativeOrAbsolute);
                process.StartInfo.FileName = pdf.LocalPath;
                process.Start();
                process.WaitForExit();
            }
            catch (Exception exc)
            {
                _ = MessageBox.Show("Unable to load the documentation, please check your file\nDetails:" + exc.Message, "Documentation error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            SavePiece();
        }
    }
}
