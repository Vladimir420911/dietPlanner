using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.Globalization;
using testMaui.Models;
using testMaui.Sevices;

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
        private async Task AddProduct()
        {
            // Валидация с инвариантной культурой
            if (string.IsNullOrWhiteSpace(NewProductName))
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Введите название продукта", "OK");
                return;
            }

            if (!double.TryParse(NewProductCalories, NumberStyles.Any, CultureInfo.InvariantCulture, out double cal) || cal < 0)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Некорректная калорийность", "OK");
                return;
            }

            if (!double.TryParse(NewProductProteins, NumberStyles.Any, CultureInfo.InvariantCulture, out double prot) || prot < 0)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Некорректное значение белков", "OK");
                return;
            }

            if (!double.TryParse(NewProductFats, NumberStyles.Any, CultureInfo.InvariantCulture, out double fat) || fat < 0)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Некорректное значение жиров", "OK");
                return;
            }

            if (!double.TryParse(NewProductCarbs, NumberStyles.Any, CultureInfo.InvariantCulture, out double carb) || carb < 0)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Некорректное значение углеводов", "OK");
                return;
            }

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
                await Application.Current.MainPage.DisplayAlert("Ошибка", $"Не удалось добавить продукт: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private void SelectProduct(Product product)
        {
            if (SelectedProduct == product)
                SelectedProduct = null;
            else
                SelectedProduct = product;
        }

        partial void OnSelectedProductChanged(Product value)
        {
            if (value != null)
            {
                NewProductName = value.Name;
                NewProductCalories = value.Calories.ToString(CultureInfo.InvariantCulture);
                NewProductProteins = value.Proteins.ToString(CultureInfo.InvariantCulture);
                NewProductFats = value.Fats.ToString(CultureInfo.InvariantCulture);
                NewProductCarbs = value.Carbs.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                NewProductName = string.Empty;
                NewProductCalories = string.Empty;
                NewProductProteins = string.Empty;
                NewProductFats = string.Empty;
                NewProductCarbs = string.Empty;
            }
        }

        [RelayCommand]
        private async Task UpdateProduct()
        {
            if (SelectedProduct == null)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Выберите продукт для обновления", "OK");
                return;
            }

            // Валидация с инвариантной культурой
            if (string.IsNullOrWhiteSpace(NewProductName))
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Введите название продукта", "OK");
                return;
            }

            if (!double.TryParse(NewProductCalories, NumberStyles.Any, CultureInfo.InvariantCulture, out double cal) || cal < 0)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Некорректная калорийность", "OK");
                return;
            }

            if (!double.TryParse(NewProductProteins, NumberStyles.Any, CultureInfo.InvariantCulture, out double prot) || prot < 0)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Некорректное значение белков", "OK");
                return;
            }

            if (!double.TryParse(NewProductFats, NumberStyles.Any, CultureInfo.InvariantCulture, out double fat) || fat < 0)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Некорректное значение жиров", "OK");
                return;
            }

            if (!double.TryParse(NewProductCarbs, NumberStyles.Any, CultureInfo.InvariantCulture, out double carb) || carb < 0)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Некорректное значение углеводов", "OK");
                return;
            }

            // Обновляем выбранный продукт
            SelectedProduct.Name = NewProductName;
            SelectedProduct.Calories = cal;
            SelectedProduct.Proteins = prot;
            SelectedProduct.Fats = fat;
            SelectedProduct.Carbs = carb;

            try
            {
                Database.UpdateProduct(SelectedProduct);

                // Обновляем элемент в коллекции
                var index = Products.IndexOf(SelectedProduct);
                if (index >= 0)
                {
                    Products[index] = SelectedProduct;
                }

                // Сбрасываем выделение
                SelectedProduct = null;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", $"Не удалось обновить продукт: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task DeleteProduct()
        {
            if (SelectedProduct == null)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Выберите продукт для удаления", "OK");
                return;
            }

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Подтверждение",
                $"Удалить продукт '{SelectedProduct.Name}'? Все связанные записи в дневнике также будут удалены.",
                "Да", "Нет");
            if (!confirm) return;

            try
            {
                Database.DeleteProduct(SelectedProduct.Id);
                Products.Remove(SelectedProduct);
                SelectedProduct = null;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", $"Не удалось удалить продукт: {ex.Message}", "OK");
            }
        }
    }
}