using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        Client currentClient = new Client();
        public AddEditPage(Client selectedClient)
        {
            InitializeComponent();

            if (selectedClient != null)
            {
                currentClient = selectedClient;
                if (currentClient.GenderCode == "ж")
                    FemRadioButton.IsChecked= true;
                else
                    MalRadioButton.IsChecked = true;
                IDTextBlock.Visibility = Visibility.Visible;
                IDTextBox.Visibility = Visibility.Visible;
            }
            else
            {
                IDTextBlock.Visibility = Visibility.Hidden;
                IDTextBox.Visibility = Visibility.Hidden;
            }
            BDTB.SelectedDate = Convert.ToDateTime("01.01.1900");

            if (currentClient.Birthday > Convert.ToDateTime("01.01.1900"))
                BDTB.SelectedDate = currentClient.Birthday;
            
            

            DataContext = currentClient;
        }

        private void FemRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void MalRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (String.IsNullOrWhiteSpace( currentClient.FirstName))
                errors.AppendLine("Введите фамилию");
            else
            {
                if (!IsMatchesPattern(currentClient.FirstName))
                    errors.AppendLine("Фамилия соджержит недопустимые символы");
                if (currentClient.FirstName.Length > 50)
                    errors.AppendLine("Длинна фамилии больше 50");
            }

            if (String.IsNullOrWhiteSpace(currentClient.LastName))
                errors.AppendLine("Введите имя");
            else
            {
                if (!IsMatchesPattern(currentClient.LastName))
                    errors.AppendLine("Имя соджержит недопустимые символы");
                if (currentClient.LastName.Length > 50)
                    errors.AppendLine("Длинна имени больше 50");
            }

            if (String.IsNullOrWhiteSpace(currentClient.Patronymic))
                errors.AppendLine("Введите отчество");
            else
            {
                if (!IsMatchesPattern(currentClient.Patronymic))
                    errors.AppendLine("Отчество соджержит недопустимые символы");
                if (currentClient.Patronymic.Length > 50)
                    errors.AppendLine("Длинна отчества больше 50");
            }


            if (String.IsNullOrWhiteSpace(currentClient.Phone))
                errors.AppendLine("Введите телефон");
            else
            {
                bool containsOnlyNumbers = true;
                string phone = currentClient.Phone.Replace("+", "").Replace("-", "").Replace("(", "").Replace(")", "").Replace(" ", "");
                if (phone.Length != 11)
                    errors.AppendLine("В номере телефона должно быть 11 цифр");
                foreach(char c in phone)
                {
                    if (c < '0' || c > '9')
                        containsOnlyNumbers = false;
                }
                if (!containsOnlyNumbers)
                    errors.AppendLine("Телефон содержит недопустимые символы");
            }

            if (String.IsNullOrWhiteSpace(currentClient.Email))
                errors.AppendLine("Введите Email");
            else
            {
                string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(currentClient.Email, pattern))
                    errors.AppendLine("Укажите правильный Email");
            }




            if (BDTB.SelectedDate == null)
                errors.AppendLine("Введите дату рождения");
            else
            {
                currentClient.Birthday = Convert.ToDateTime(BDTB.SelectedDate);
            }

            if (FemRadioButton.IsChecked == false && MalRadioButton.IsChecked == false)
            {
                errors.AppendLine("Выберите пол");

            }


            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            if (currentClient.ID == 0)
            {
                currentClient.RegistrationDate = DateTime.Today;
               

                SalimgareevaLanguageEntities.GetContext().Client.Add(currentClient);
            }
            if (MalRadioButton.IsChecked.Value)
                currentClient.GenderCode = "м";
            else
                currentClient.GenderCode = "ж";
            try
            {
                SalimgareevaLanguageEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }



        private bool IsMatchesPattern(string str)
        {
            str = str.Replace("-", "").Replace(" ", "");
            bool isOK = true;
            foreach (char character in str)
            {
                if (!(character >= 'а' && character <= 'я' || character >= 'А' && character <= 'Я' || character >= 'a' && character <= 'z' || character >= 'A' && character <= 'Z'))
                {
                    isOK = false;
                    return isOK;
                }

            }

            return isOK;
        }
        private void ChangePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            string projectDirectory = GetProjectRootDirectory();
            string clientsFolderPath = System.IO.Path.Combine(projectDirectory, "Клиенты");

            if (!Directory.Exists(clientsFolderPath))
            {
                Directory.CreateDirectory(clientsFolderPath);
            }

            OpenFileDialog myOpenFileDialog = new OpenFileDialog
            {
                InitialDirectory = clientsFolderPath
            };

            if (myOpenFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = myOpenFileDialog.FileName;

                // Сохраняем относительный путь ОТНОСИТЕЛЬНО КОРНЯ ПРОЕКТА
                currentClient.PhotoPath = System.IO.Path.Combine("Клиенты", System.IO.Path.GetFileName(selectedFilePath));

                // Загружаем изображение по полному пути
                LogoImage.Source = new BitmapImage(new Uri(selectedFilePath));
            }
        }

        // Метод для получения корня проекта
        private string GetProjectRootDirectory()
        {
            // Путь к исполняемому файлу (bin/Debug)
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            // Поднимаемся на 3 уровня: bin/Debug → bin → Корень проекта
            return System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(exePath)));
        }
    }
}
