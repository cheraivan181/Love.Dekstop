using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Love.Dekstop
{
    /// <summary>
    /// Логика взаимодействия для CreateDialogRequest.xaml
    /// </summary>
    public partial class CreateDialogRequest : Window
    {
        private ObservableCollection<RequstedContact> newRequsts;
        private ObservableCollection<RequstedContact> sendedRequsts;
        public CreateDialogRequest()
        {
            InitializeComponent();
            newRequsts = new ObservableCollection<RequstedContact>
            {

                new RequstedContact() {Name = "Pendos"},
                new RequstedContact() {Name = "Chera"},
                new RequstedContact() {Name = "Peder"},

            };
            RequstList.ItemsSource = newRequsts;
        }

        private void Back_Button_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Idi na hooi", "На хуй это читать", MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        public class RequstedContact
        {
            public string Name { get; set; }
        }

        public void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Name, "На хуй это читать", MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}
