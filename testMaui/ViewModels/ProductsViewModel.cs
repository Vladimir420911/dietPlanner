using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MySql.Data.MySqlClient;
using testMaui.Models;
using testMaui.Sevices;
using System.Collections.ObjectModel;

namespace testMaui.ViewModels
{
    public partial class ProductsViewModel : BaseViewModel
    {
        [ObservableProperty]
        private ObservableCollection<Product> products;

        [ObservableProperty]
        private Product selectedProduct;

        [ObservableProperty]
        private string newProductName;

        [ObservableProperty]
        private string newProductCalories;

        [ObservableProperty]
        private string newProductProteins;

        [ObservableProperty]
        private string newProductFats;

        [ObservableProperty]
        private string newProductCarbs;

        public ProductsViewModel()
        {
            Products = new ObservableCollection<Product>(Database.GetAllProducts());
        }

        [RelayCommand]
        private void AddProduct()
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(NewProductName))
                return;

            if (!double.TryParse(NewProductCalories, out double cal) || cal < 0)
                return;

            if (!double.TryParse(NewProductProteins, out double prot) || prot < 0)
                return;

            if (!double.TryParse(NewProductFats, out double fat) || fat < 0)
                return;

            if (!double.TryParse(NewProductCarbs, out double carb) || carb < 0)
                return;

            var product = new Product
            {
                Name = NewProductName,
                Calories = cal,
                Proteins = prot,
                Fats = fat,
                Carbs = carb
            };

            try
            {
                int newId = Database.AddProduct(product);
                product.Id = newId;
                Products.Add(product);

                // Очистка полей
                NewProductName = string.Empty;
                NewProductCalories = string.Empty;
                NewProductProteins = string.Empty;
                NewProductFats = string.Empty;
                NewProductCarbs = string.Empty;
            }
            catch (Exception ex)
            {
                // Здесь можно добавить отображение ошибки пользователю
                System.Diagnostics.Debug.WriteLine($"Ошибка добавления продукта: {ex.Message}");
            }
        }

        [RelayCommand]
        private void UpdateProduct()
        {
            if (SelectedProduct == null)
                return;

            try
            {
                Database.UpdateProduct(SelectedProduct);
                // Обновляем элемент в коллекции (если нужно, но он уже обновлён)
                var index = Products.IndexOf(SelectedProduct);
                if (index >= 0)
                {
                    Products[index] = SelectedProduct;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка обновления продукта: {ex.Message}");
            }
        }

        [RelayCommand]
        private void DeleteProduct()
        {
            if (SelectedProduct == null)
                return;

            try
            {
                Database.DeleteProduct(SelectedProduct.Id);
                Products.Remove(SelectedProduct);
            }
            catch (MySqlException ex) when (ex.Number == 1451) // FOREIGN KEY constraint fails
            {
                // Продукт используется в приёмах пищи — нельзя удалить
                // Здесь можно показать предупреждение пользователю
                System.Diagnostics.Debug.WriteLine("Продукт нельзя удалить, так как он используется в дневнике.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка удаления продукта: {ex.Message}");
            }
        }
    }
}