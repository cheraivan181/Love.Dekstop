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
    /// Логика взаимодействия для ContactsRequestPage.xaml
    /// </summary>
    public partial class ContactsRequestPage : Window
    {
        private ObservableCollection<TestNewContact> requsts;
        private TestNewContact selectredItem;
        public ContactsRequestPage()
        {
            InitializeComponent();
            requsts = new ObservableCollection<TestNewContact>
            {

                new TestNewContact() {Name = "Pendos"},
                new TestNewContact() {Name = "Chera"},
                new TestNewContact() {Name = "Peder"},

            };
            RequstList.ItemsSource = requsts;
        }

        private void RequstList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectredItem = (TestNewContact) RequstList.SelectedItem;
        }


        private void Aply_Button_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(selectredItem.Name.ToString(), "На хуй это читать", MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void Deny_Button_OnClick(object sender, RoutedEventArgs e)
        {
            requsts.Remove(selectredItem);
        }

        private void Back_Button_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Idi na hooi", "На хуй это читать", MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
    public class TestNewContact
    {
        public string Name { get; set; }
    }
}
