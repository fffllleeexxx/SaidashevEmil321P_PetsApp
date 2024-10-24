﻿using System;
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

namespace PetsApp
{
    public partial class MainWindow : Window
    {
        private Users currentUser;
        private List<Pet> originalPets;
        private List<Pet> filteredPets;

        public MainWindow(Users user)
        {
            InitializeComponent();
            currentUser = user;
            LoadData();
        }

        private void LoadData()
        {
            using (var context = new PetsModelContainer())
            {
                var query = context.Pet.AsQueryable();

                if (currentUser.Role == "Админ")
                {
                    query = context.Pet.AsQueryable();
                }
                else if (currentUser.Role == "Ра")
                {
                    query = context.Pet.Where(p => p.Name == "Ра").AsQueryable();
                }
                else if (currentUser.Role == "Нуби")
                {
                    query = context.Pet.Where(p => p.Name == "Нуби").AsQueryable();
                }

                originalPets = query.ToList();
                filteredPets = originalPets.ToList();
                ApplySort();
            }
        }

        private void ApplySearch()
        {
            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                filteredPets = originalPets.Where(p => p.Description.Contains(txtSearch.Text)).ToList();
            }
            else
            {
                filteredPets = originalPets.ToList();
            }

            ApplySort();
        }

        private void ApplySort()
        {
            switch (cmbSort.SelectedIndex)
            {
                case 0: 
                    filteredPets = filteredPets.OrderBy(p => p.Name).ToList();
                    break;
                case 1: 
                    filteredPets = filteredPets.OrderByDescending(p => p.Name).ToList();
                    break;
                case 2: 
                    filteredPets = filteredPets.OrderBy(p => p.Description).ToList();
                    break;
                case 3: 
                    filteredPets = filteredPets.OrderByDescending(p => p.Description).ToList();
                    break;
            }

            DisplayPets();
        }

        private void DisplayPets()
        {
            petStackPanel.Children.Clear();

            foreach (var pet in filteredPets)
            {
                var petPanel = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(0, 50, 0, 0) 
                };

                var headerPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };

                var nameTextBlock = new TextBlock
                {
                    Text = pet.Name,
                    FontSize = 20,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 10, 0)
                };

                var image = new Image
                {
                    Source = new BitmapImage(new Uri(pet.ImagePath, UriKind.Absolute)),
                    Width = 100,
                    Height = 100,
                    Margin = new Thickness(200, 0, 0, 0) 
                };

                headerPanel.Children.Add(nameTextBlock);
                headerPanel.Children.Add(image);

                var descriptionTextBlock = new TextBlock
                {
                    Text = pet.Description,
                    FontSize = 14,
                    Margin = new Thickness(0, 20, 0, 0) 
                };

                petPanel.Children.Add(headerPanel);
                petPanel.Children.Add(descriptionTextBlock);

                petStackPanel.Children.Add(petPanel);
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplySearch();
        }

        private void cmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplySort();
        }

        private void btnAddPet_Click(object sender, RoutedEventArgs e)
        {
            AddPetWindow addPetWindow = new AddPetWindow();
            addPetWindow.ShowDialog();
            LoadData(); 
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
