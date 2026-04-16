
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



namespace MathAnimator
{
    public partial class InputView : UserControl
    {
        private readonly MainWindow _host;


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
            Loaded += (_, _) => RenderPreview();
        }


        private void OnAnimate(object sender, RoutedEventArgs e)
        {
            try
            {
                string formula = FormulaBox.Text;

                double a = double.Parse(ABox.Text);
                double b = double.Parse(BBox.Text);
                double c = double.Parse(CBox.Text);



                _host.ShowView(
                    new AnimationView(
                        _host,
                        formula,
                        a,
                        b,
                        c
                    )
                );


            }
            catch
            {
                MessageBox.Show(
                    "Bitte alle Felder korrekt ausfüllen.",
                    "Fehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }


        private void OnBack(object sender, RoutedEventArgs e)
        {
            _host.ShowView(new StartView(_host));
        }




        void RenderPreview()
        {
            if (_bitmap == null || _renderer == null)
                return;

            try
            {
                ErrorText.Text = "";

                var func = MathParser.Parse(FormulaBox.Text);

                double a = double.Parse(ABox.Text);
                double b = double.Parse(BBox.Text);
                double c = double.Parse(CBox.Text);

                _renderer.Render(_bitmap, func, a, b, c);
            }
            catch (Exception ex)
            {
                ErrorText.Text = ex.Message;
            }
        }




        private void OnAddToLibrary(object sender, RoutedEventArgs e)
        {

            var func = new FunctionDefinition
            {
                Formula = FormulaBox.Text,
                A = double.Parse(ABox.Text),
                B = double.Parse(BBox.Text),
                C = double.Parse(CBox.Text)
            };


            var list = new List<FunctionDefinition>();

            if (File.Exists("functions.json"))
            {
                list = JsonSerializer.Deserialize<List<FunctionDefinition>>(
                    File.ReadAllText("functions.json"))!;
            }

            list.Add(func);

            File.WriteAllText("functions.json",
                JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true }));

            MessageBox.Show("Zur Bibliothek hinzugefügt!");
        }

        private void OnInputChanged(object sender, TextChangedEventArgs e)
        {
            RenderPreview();
        }


    }
}