using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace Salimgareevalanguage
{
    /// <summary>
    /// Логика взаимодействия для ClientPage.xaml
    /// </summary>
    public partial class ClientPage : Page
    {
        int CountRecords;
        int CountPage;
        int CurrentPage = 0;

        List<Client> CurrentPageList = new List<Client>();
        List<Client> TableList;
        public ClientPage()
        {
            InitializeComponent();

            var currentClients = SalimgareevaLanguageEntities.GetContext().Client.ToList();

            ClientListView.ItemsSource = currentClients;

            CountOfElementsComboBox.SelectedIndex = 0;
            GenderSort.SelectedIndex = 0;
            sort.SelectedIndex = 0;

            UpdateClient();


        }


        private void UpdateClient()
        {
            
            var currentClients = SalimgareevaLanguageEntities.GetContext().Client.ToList();

            if (GenderSort.SelectedIndex == 1)
            {
                currentClients = currentClients.Where(p => p.GenderCode == "ж").ToList();
            }
            if (GenderSort.SelectedIndex == 2)
            {
                currentClients = currentClients.Where(p => p.GenderCode == "м").ToList();
            }
            if (sort.SelectedIndex == 1)
            {
                currentClients = currentClients.OrderBy(p => p.FirstName).ToList();
            }
            if (sort.SelectedIndex == 2)
            {
                currentClients = currentClients.OrderByDescending(p => p.LastSessionDate).ToList();
            }
            if (sort.SelectedIndex == 3)
            {
                currentClients = currentClients.OrderByDescending(p => p.CountOfSessions).ToList();
            }
            string searchText = Search.Text.ToLower();
            currentClients = currentClients.Where(p => p.FirstName.ToLower().Contains(searchText) || p.LastName.ToLower().Contains(searchText) || p.Patronymic.ToLower().Contains(searchText)
            || p.Email.ToLower().Contains(searchText)
            || p.Phone.ToLower().Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Contains(searchText.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", ""))
            ).ToList();


            ClientListView.ItemsSource = currentClients;

            TableList = currentClients;
            ChangePage(0, 0);
        }











        private int CountOfElementsOnePage()
        {
            if (CountOfElementsComboBox.SelectedIndex == 0)
            {
                return 10;
            }
            if (CountOfElementsComboBox.SelectedIndex == 1)
            {
                return 50;
            }
            if (CountOfElementsComboBox.SelectedIndex == 2)
            {
                return 200;
            }

            return 1000; ////////////////////////////////////
        }

        private int Min()
        {
            int min = CurrentPage * CountOfElementsOnePage() + CountOfElementsOnePage() < CountRecords ? CurrentPage * CountOfElementsOnePage() + CountOfElementsOnePage() : CountRecords;
            return min;
        }

        private void ChangePage(int direction, int? selectedPage)
        {
            if (CountOfElementsComboBox.SelectedIndex != 3)
            {
                DirectionStackPanel.Visibility = Visibility.Visible;


                CurrentPageList.Clear();
                CountRecords = TableList.Count;

                if (CountRecords % CountOfElementsOnePage() > 0)
                {
                    CountPage = CountRecords / CountOfElementsOnePage() + 1;
                }
                else
                {
                    CountPage = CountRecords / CountOfElementsOnePage();
                }
                Boolean IfUpdate = true;
                if (selectedPage.HasValue)
                {
                    if (selectedPage >= 0 && selectedPage <= CountPage)
                    {
                        CurrentPage = (int)selectedPage;
                        for (int i = CurrentPage * CountOfElementsOnePage(); i < Min(); i++)
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
                                for (int i = CurrentPage * CountOfElementsOnePage(); i < Min(); i++)
                                {
                                    CurrentPageList.Add(TableList[i]);
                                }
                            }
                            else
                            {
                                IfUpdate = false;
                            }
                            break;
                        case 2:
                            if (CurrentPage < CountPage - 1)
                            {
                                CurrentPage++;
                                for (int i = CurrentPage * CountOfElementsOnePage(); i < Min(); i++)
                                {
                                    CurrentPageList.Add(TableList[i]);
                                }
                            }
                            else
                            {
                                IfUpdate = false;
                            }
                            break;
                    }


                }
                if (IfUpdate)
                {
                    PageListBox.Items.Clear();
                    for (int i = 1; i <= CountPage; i++)
                    {
                        PageListBox.Items.Add(i);
                    }
                    ClientListView.ItemsSource = CurrentPageList;

                    ClientListView.Items.Refresh();

                    TBCount.Text = Min().ToString();
                    TBAllRecords.Text = CountRecords.ToString();
                }
            }
            else
            {
                DirectionStackPanel.Visibility = Visibility.Collapsed;
                var currentClients = SalimgareevaLanguageEntities.GetContext().Client.ToList();

                ClientListView.ItemsSource = currentClients;

            }
        }

        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2, null);
        }

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem.ToString()) - 1);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var currentClient = (sender as Button).DataContext as Client;
            if (currentClient.CountOfSessions != 0)
            {
                MessageBox.Show("Невозможно выполнить удаление, так как есть записи");
            }
            else
            {
                if(MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        SalimgareevaLanguageEntities.GetContext().Client.Remove(currentClient);
                        SalimgareevaLanguageEntities.GetContext().SaveChanges();
                        UpdateClient();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
        }

        private void CountOfElementsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateClient();
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateClient();

        }

        private void GenderSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateClient();

        }

        private void sort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateClient();

        }
    }
}
