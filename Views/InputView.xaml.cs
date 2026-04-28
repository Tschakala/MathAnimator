using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MathAnimator.Model;
using MathAnimator.MathCore;
using MathAnimator.Rendering;
using System.Linq;

namespace MathAnimator
{
    public partial class InputView : UserControl
    {
        private readonly MainWindow _host;
        private bool _isInitialized = false;

        WriteableBitmap _bitmap;
        GraphRenderer _renderer;

        public InputView(MainWindow host)
        {
            InitializeComponent();
            _host = host;

            _bitmap = new WriteableBitmap(
                600, 300, 96, 96, PixelFormats.Bgra32, null);

            _renderer = new GraphRenderer(600, 300);
            Preview.Source = _bitmap;

            Loaded += (_, _) =>
            {
                _isInitialized = true;
                UpdateModeVisibility();
                LoadFolders();
                RenderPreview();
            };
        }

        private void OnAnimate(object sender, RoutedEventArgs e)
        {
            try
            {
                double a = double.Parse(ABox.Text);
                double b = double.Parse(BBox.Text);
                double c = double.Parse(CBox.Text);

                if (FunctionMode.IsChecked == true)
                {
                    _host.ShowView(
                        new AnimationView(
                            _host,
                            GraphMode.Function,
                            FormulaBox.Text,
                            "",
                            "",
                            a, b, c
                        )
                    );
                }
                else
                {
                    _host.ShowView(
                        new AnimationView(
                            _host,
                            GraphMode.Parametric,
                            "",
                            XFormulaBox.Text,
                            YFormulaBox.Text,
                            a, b, c
                        )
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Fehler beim Animieren");
            }
        }

        private void OnBack(object sender, RoutedEventArgs e)
        {
            _host.GoBack();
        }

        void RenderPreview()
        {

            if (!_isInitialized || _bitmap == null || _renderer == null)
                return;

            try
            {
                ErrorText.Text = "";

                double a = double.Parse(ABox.Text);
                double b = double.Parse(BBox.Text);
                double c = double.Parse(CBox.Text);

                if (FunctionMode.IsChecked == true)
                {
                    var func = MathParser.Parse(FormulaBox.Text);

                    _renderer.Render(
                        _bitmap,
                        func,
                        a, b, c
                    );
                }
                else
                {
                    var fx = MathParser.Parse(XFormulaBox.Text);
                    var fy = MathParser.Parse(YFormulaBox.Text);

                    _renderer.RenderParametric(
                        _bitmap,
                        fx,
                        fy,
                        a, b, c,
                        -10,
                        10,
                        0.02
                    );
                }
            }
            catch (Exception ex)
            {
                ErrorText.Text = ex.Message;
            }
        }

        private void OnAddToLibrary(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FolderBox.SelectedItem is not LibraryFolder uiFolder)
                {
                    MessageBox.Show("Bitte einen Ordner auswählen.");
                    return;
                }

                string selectedFolderName = uiFolder.Name;

                var library = LibraryStore.Load();

                var targetFolder = library.Folders
                    .FirstOrDefault(f => f.Name == selectedFolderName);

                if (targetFolder == null)
                {
                    MessageBox.Show("Der ausgewählte Ordner existiert nicht mehr.");
                    return;
                }

                var func = new FunctionDefinition
                {
                    A = double.Parse(ABox.Text),
                    B = double.Parse(BBox.Text),
                    C = double.Parse(CBox.Text)
                };

                if (FunctionMode.IsChecked == true)
                {
                    func.Mode = GraphMode.Function;
                    func.Formula = FormulaBox.Text;
                }
                else
                {
                    func.Mode = GraphMode.Parametric;
                    func.XFormula = XFormulaBox.Text;
                    func.YFormula = YFormulaBox.Text;
                }

                targetFolder.Functions.Add(func);

                LibraryStore.Save(library);

                MessageBox.Show($"Funktion wurde zu „{selectedFolderName}“ hinzugefügt.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Fehler");
            }
        }

        private void OnInputChanged(object sender, TextChangedEventArgs e)
        {
            RenderPreview();
        }

        private void OnModeChanged(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized)
                return;

            UpdateModeVisibility();
            RenderPreview();
        }

        private void LoadFolders()
        {
            var library = LibraryStore.Load();
            FolderBox.ItemsSource = library.Folders;
            FolderBox.SelectedIndex = 0;
        }

        private void UpdateModeVisibility()
        {
            bool isFunction = FunctionMode.IsChecked == true;

            FormulaBox.Visibility = isFunction
                ? Visibility.Visible
                : Visibility.Collapsed;

            XFormulaBox.Visibility = isFunction
                ? Visibility.Collapsed
                : Visibility.Visible;

            YFormulaBox.Visibility = isFunction
                ? Visibility.Collapsed
                : Visibility.Visible;
        }
    }
}