using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;


namespace SofdesQuiz3_1
{
    public sealed partial class MainWindow : Window, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private List<User> _users;
        private List<User> Users
        {
            get => _users;
            set { _users = value; OnPropertyChanged(); }
        }

        public MainWindow()
        {
            InitializeComponent();
            LoadData();
            genderInput.SelectedItem = "Unspecified";
        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        private async void Remove(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(idInput.Text))
            {
                await new ContentDialog
                {
                    Title = "No user selected",
                    Content = "Select the user you want to delete from the list first.",
                    CloseButtonText = "Okay",
                    XamlRoot = Content.XamlRoot,
                }.ShowAsync();
                return;
            }

            var response = await new ContentDialog
            {
                Title = "Delete user",
                Content = "Are you sure you want to delete this user?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No",
                XamlRoot = Content.XamlRoot,
            }.ShowAsync();

            if (response == ContentDialogResult.Primary)
            {
                var id = int.Parse(idInput.Text);
                UsersDb.Delete(id);
                Clear();
                LoadData();
            }
        }

        private async void AddUpdate(object sender, RoutedEventArgs e)
        {
            var user = await ParseUserAsync();
            if (user != null)
            {
                UsersDb.InsertUpdate(user);
                Clear();
                LoadData();
            }
        }

        private void Search(object sender, TextChangedEventArgs e)
        {
            var search = searchInput.Text;
            Users = UsersDb.GetAll(search);
        }

        private void SelectUser(object sender, DoubleTappedRoutedEventArgs e)
        {
            var user = usersDataGrid.SelectedItem as User;
            if (user != null)
            {
                idInput.Text = user.Id.ToString();
                fullNameInput.Text = user.FullName;
                emailInput.Text = user.Email;
                birthdateInput.SelectedDate = user.Birthdate;
                genderInput.SelectedItem = user.Gender;
                addressInput.Text = user.Address;
            }
        }

        private async Task<User> ParseUserAsync()
        {
            int? id;

            if (string.IsNullOrEmpty(idInput.Text))
            {
                id = null;
            }
            else
            {
                id = int.Parse(idInput.Text);
            }

            var fullName = fullNameInput.Text;
            var email = emailInput.Text;
            var birthdate = birthdateInput.SelectedDate;
            var gender = genderInput.SelectedItem as string;
            var address = addressInput.Text;

            if (string.IsNullOrEmpty(fullName) ||
                string.IsNullOrEmpty(email) ||
                birthdate == null ||
                gender == null ||
                string.IsNullOrEmpty(address))
            {
                await new ContentDialog
                {
                    Title = "Empty fields",
                    Content = "None of the fields can be empty.",
                    CloseButtonText = "Okay",
                    XamlRoot = Content.XamlRoot,
                }.ShowAsync();
                return null;
            }
            return new User(fullName, email, (DateTimeOffset)birthdate, gender, address, id);
        }

        private void Clear()
        {
            idInput.Text = string.Empty;
            fullNameInput.Text = string.Empty;
            emailInput.Text = string.Empty;
            birthdateInput.SelectedDate = null;
            genderInput.SelectedItem = "Unspecified";
            addressInput.Text = string.Empty;
            usersDataGrid.SelectedItem = null;
        }

        private void LoadData()
        {
            Users = UsersDb.GetAll();
        }

        private void idInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(idInput.Text))
            {
                addButton.Content = "Add";
            }
            else
            {
                addButton.Content = "Update";
            }
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
