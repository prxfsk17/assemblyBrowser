using AssemblyExplorer.Models;
using AssemblyExplorer.ViewModel;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;

namespace MPP_3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MyViewModel VM;
        public MainWindow()
        {
            InitializeComponent();
            VM = new MyViewModel();
            DataContext = VM;
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if(e.NewValue is AssemblyNode) VM.SelectedAssm = (AssemblyNode)e.NewValue;
        }
    }
}