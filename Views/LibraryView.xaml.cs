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
        private readonly MainWindow _host;

        public LibraryView(MainWindow host)
        {
            InitializeComponent();
            _host = host;


            try
            {
                if (File.Exists("functions.json"))
                {
                    string json = File.ReadAllText("functions.json");

                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        FunctionList.ItemsSource =
                            JsonSerializer.Deserialize<List<FunctionDefinition>>(json);
                    }
                    else
                    {
                        FunctionList.ItemsSource = new List<FunctionDefinition>();
                    }
                }
                else
                {
                    FunctionList.ItemsSource = new List<FunctionDefinition>();
                }
            }
            catch
            {
                // Falls alte / kaputte JSON-Datei existiert
                File.Delete("functions.json");
                FunctionList.ItemsSource = new List<FunctionDefinition>();
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

    }
}