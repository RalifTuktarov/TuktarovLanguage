using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TuktarovLanguage
{
    /// <summary>
    /// Логика взаимодействия для ClientPage.xaml
    /// </summary>
    public partial class ClientPage : Page
    {
        int CountRecords;
        int CountPage;
        int CurrentPage = 0;
        private int recordsPerPage = 10;

        List<Client> CurrentPageList = new List<Client>();
        List<Client> TableList;
        public ClientPage()
        {
            InitializeComponent();
            var currentClients = TuktarovLanguageEntities.GetContext().Client.ToList();
            ClientListView.ItemsSource = currentClients;
            UpdateClient();
        }
        private void UpdateClient()
        {
            var CurrentClient = TuktarovLanguageEntities.GetContext().Client.ToList();
            CurrentClient = CurrentClient.Where(p => p.FirstName.ToLower().Contains(TBoxSear4.Text.ToLower())).ToList();
            string searchText = TBoxSear4.Text.ToLower();
            CurrentClient = CurrentClient.Where(p => p.FirstName.Replace(" ", "").ToLower().Contains(searchText) || p.LastName.Replace(" ", "").ToLower().Contains(searchText) || p.Patronymic.Replace(" ", "").ToLower().Contains(searchText)
).ToList();
            
            if (Combobox.SelectedIndex == 0)
            {
                CurrentClient = CurrentClient.Where(p =>
                    p.Gender.Name.Contains("мужской") ||
                    p.Gender.Name.Contains("женский")
                ).ToList();
            }
            if (Combobox.SelectedIndex == 1)
            {
                CurrentClient = CurrentClient.Where(p => (p.Gender.Name.Contains("женский"))).ToList();
            }
            if (Combobox.SelectedIndex == 2)
            {
                CurrentClient = CurrentClient.Where(p => (p.Gender.Name.Contains("мужской"))).ToList();
            }
            if (Combopoisk.SelectedIndex == 1)
            {
                CurrentClient = CurrentClient.OrderBy(p => p.FirstName).ToList();
            }

            if (Combopoisk.SelectedIndex == 2)
            {
                CurrentClient = CurrentClient.OrderBy(p => p.RegistrationDate).ToList();
            }
            if (Combopoisk.SelectedIndex == 3)
            {
                CurrentClient = CurrentClient.OrderBy(p => p.VisitCount).ToList();
            }
            ClientListView.ItemsSource = CurrentClient.ToList();
            ClientListView.ItemsSource = CurrentClient;
            TableList = CurrentClient;
            ChangePage(0, 0);
        }

        private void ChangePage(int direction, int? selectedPage)
        {
            CurrentPageList.Clear();
            CountRecords = TableList.Count;
            if (CountRecords % recordsPerPage > 0)
            {
                CountPage = CountRecords / recordsPerPage + 1;
            }
            else
            {
                CountPage = CountRecords / recordsPerPage;
            }

            Boolean Ifupdate = true;

            int min;

            if (selectedPage.HasValue)
            {
                if (selectedPage >= 0 && selectedPage <= CountPage)
                {
                    CurrentPage = (int)selectedPage;
                    min = CurrentPage * recordsPerPage + recordsPerPage < CountRecords ? CurrentPage * recordsPerPage + recordsPerPage : CountRecords;
                    for (int i = CurrentPage * recordsPerPage; i < min; i++)
                    {
                        CurrentPageList.Add(TableList[i]);
                    }
                }
            }
            else
            {
                switch (direction)
                {
                    case 1:
                        if (CurrentPage > 0)
                        {
                            CurrentPage--;
                            min = CurrentPage * recordsPerPage + recordsPerPage < CountRecords ? CurrentPage * recordsPerPage + recordsPerPage : CountRecords;
                            for (int i = CurrentPage * recordsPerPage; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            Ifupdate = false;
                        }
                        break;


                    case 2:
                        if (CurrentPage < CountPage - 1)
                        {
                            CurrentPage++;
                            min = CurrentPage * recordsPerPage + recordsPerPage < CountRecords ? CurrentPage * recordsPerPage + recordsPerPage : CountRecords;
                            for (int i = CurrentPage * recordsPerPage; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            Ifupdate = false;
                        }
                        break;
                }
            }
            if (Ifupdate)
            {
                PageListBox.Items.Clear();
                for (int i = 1; i <= CountPage; i++)
                {
                    PageListBox.Items.Add(i);
                }
                PageListBox.SelectedIndex = CurrentPage;

                min = CurrentPage * recordsPerPage + recordsPerPage < CountRecords ? CurrentPage * recordsPerPage + recordsPerPage : CountRecords;
                TBCount.Text = min.ToString();
                TBAllRecords.Text = " из " + CountRecords.ToString();
                ClientListView.ItemsSource = CurrentPageList;
                ClientListView.Items.Refresh();
            }
        }
        private void Order_Click(object sender, RoutedEventArgs e)
        {

        }

        private void YdalitCLT_Click(object sender, RoutedEventArgs e)
        {
            Client client = (sender as Button).DataContext as Client;

            if (client.VisitCount != 0)
            {
                MessageBox.Show("У клиента есть посещения, удаление невозможно!");
                return;
            }

            if (client.VisitCount == 0)
            {
                if (MessageBox.Show("Удалить клиента?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        TuktarovLanguageEntities.GetContext().Client.Remove(client);
                        TuktarovLanguageEntities.GetContext().SaveChanges();

                        ClientListView.ItemsSource = TuktarovLanguageEntities.GetContext().Client.ToList();
                        UpdateClient();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Непредвиденная ошибка. Обратитесь к адинистратору.\nПодробнее: " + ex.ToString());
                    }
                }
                else
                {
                    MessageBox.Show("Удаление клиента отменено.");
                }
            }
        }

        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
        }

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem.ToString()) - 1);
        }


        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2, null);
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Получение выбранного значения из ComboBox
            ComboBoxItem selectedItem = (ComboBoxItem)ComboType.SelectedItem;
            string selectedValue = selectedItem.Content.ToString();

            // Установка количества записей на странице в зависимости от выбора пользователя
            switch (selectedValue)
            {
                case "10":
                    recordsPerPage = 10;
                    break;
                case "50":
                    recordsPerPage = 50;
                    break;
                case "200":
                    recordsPerPage = 200;
                    break;
                case "Все":
                    recordsPerPage = CountRecords; // Выводим все записи
                    break;
            }

            // Перезагрузка страницы с новым количеством записей
            ChangePage(0, 0);
        }

        private void TBoxSear4_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateClient();
        }


        private void Combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateClient();
        }



        private void Combopoisk_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            UpdateClient();
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
