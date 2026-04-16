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

            if (File.Exists("functions.json"))
            {
                FunctionList.ItemsSource =
                    JsonSerializer.Deserialize<List<FunctionDefinition>>(
                        File.ReadAllText("functions.json"));
            }

        }


        private void OnAnimate(object sender, RoutedEventArgs e)
        {

            if (FunctionList.SelectedItem is FunctionDefinition func)
            {
                _host.ShowView(
                    new AnimationView(
                        _host,
                        func.Formula,
                        func.A,
                        func.B,
                        func.C
                    )
                );
            }
        }


        private void OnBack(object sender, RoutedEventArgs e)
        {
            _host.ShowView(new StartView(_host));
        }
    }
}