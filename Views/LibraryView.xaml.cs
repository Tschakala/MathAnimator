using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using MathAnimator.Model;

namespace MathAnimator
{
    public partial class LibraryView : UserControl
    {
        LibraryData _library;
        LibraryFolder? _selectedFolder;
        private readonly MainWindow _host;

        public LibraryView(MainWindow host)
        {
            InitializeComponent();
            _host = host;

            _library = LibraryStore.Load();

            FolderList.ItemsSource = _library.Folders;
            MoveTargetFolder.ItemsSource = _library.Folders;

            if (_library.Folders.Count > 0)
            {
                _selectedFolder = _library.Folders[0];
                FolderList.SelectedItem = _selectedFolder;
                FunctionList.ItemsSource = _selectedFolder.Functions;

                MoveTargetFolder.SelectedItem = _selectedFolder;
            }
        }

        private void OnAnimate(object sender, RoutedEventArgs e)
        {
            if (FunctionList.SelectedItem is FunctionDefinition func)
            {
                _host.ShowView(
                    new AnimationView(
                        _host,
                        func.Mode,
                        func.Formula,
                        func.XFormula,
                        func.YFormula,
                        func.A,
                        func.B,
                        func.C
                    )
                );
            }
        }

        private void OnBack(object sender, RoutedEventArgs e)
        {
            _host.GoBack();
        }

        private void OnFolderChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedFolder = FolderList.SelectedItem as LibraryFolder;

            if (_selectedFolder == null)
                return;

            FunctionList.ItemsSource = _selectedFolder.Functions;

            MoveTargetFolder.SelectedItem = _selectedFolder;
        }

        private void OnAddFolder(object sender, RoutedEventArgs e)
        {
            var folder = new LibraryFolder { Name = "Neuer Ordner" };
            _library.Folders.Add(folder);
            LibraryStore.Save(_library);

            FolderList.Items.Refresh();
            FolderList.SelectedItem = folder;
        }

        private void OnRenameFolder(object sender, RoutedEventArgs e)
        {
            if (_selectedFolder == null) return;

            string name = Microsoft.VisualBasic.Interaction.InputBox(
                "Neuer Ordnername:", "Ordner umbenennen", _selectedFolder.Name);

            if (string.IsNullOrWhiteSpace(name)) return;

            _selectedFolder.Name = name;
            LibraryStore.Save(_library);
            FolderList.Items.Refresh();
        }

        private void OnDeleteFolder(object sender, RoutedEventArgs e)
        {
            if (_selectedFolder == null) return;
            if (_library.Folders.Count == 1)
            {
                MessageBox.Show("Mindestens ein Ordner muss vorhanden sein.");
                return;
            }

            if (MessageBox.Show(
                $"Ordner '{_selectedFolder.Name}' löschen?",
                "Bestätigung",
                MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            _library.Folders.Remove(_selectedFolder);
            LibraryStore.Save(_library);

            FolderList.Items.Refresh();
            FolderList.SelectedIndex = 0;
        }

        private void OnDeleteFunction(object sender, RoutedEventArgs e)
        {
            if (_selectedFolder == null)
                return;

            if (FunctionList.SelectedItem is not FunctionDefinition func)
                return;

            var result = MessageBox.Show(
                "Diese Funktion wirklich löschen?",
                "Funktion löschen",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            _selectedFolder.Functions.Remove(func);
            LibraryStore.Save(_library);

            FunctionList.Items.Refresh();
        }

        private void OnMoveFunction(object sender, RoutedEventArgs e)
        {
            if (_selectedFolder == null)
                return;

            if (FunctionList.SelectedItem is not FunctionDefinition func)
            {
                MessageBox.Show("Bitte eine Funktion auswählen.");
                return;
            }

            if (MoveTargetFolder.SelectedItem is not LibraryFolder targetFolder)
            {
                MessageBox.Show("Bitte Zielordner auswählen.");
                return;
            }

            if (targetFolder == _selectedFolder)
            {
                MessageBox.Show("Quelle und Ziel sind identisch.");
                return;
            }

            _selectedFolder.Functions.Remove(func);
            targetFolder.Functions.Add(func);

            LibraryStore.Save(_library);

            FunctionList.Items.Refresh();

            MessageBox.Show(
                $"Funktion nach „{targetFolder.Name}“ verschoben.");
        }
    }
}