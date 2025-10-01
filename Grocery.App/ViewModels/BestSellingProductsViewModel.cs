
using System.Collections.ObjectModel;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using Grocery.Core.Services;

namespace Grocery.App.ViewModels
{
    public partial class BestSellingProductsViewModel : BaseViewModel
    {
        private readonly IGroceryListItemsService _groceryListItemsService;
        private readonly IBoughtProductsService _boughtProductsService;
        public ObservableCollection<BestSellingProducts> Products { get; set; } = [];
        public BestSellingProductsViewModel(IGroceryListItemsService groceryListItemsService, IBoughtProductsService boughtProductsService,)
        {
            _groceryListItemsService = groceryListItemsService;
            _boughtProductsService = boughtProductsService;
            Products = [];
            Load();
        }

        public override void Load()
        {
            Products.Clear();
            foreach (BestSellingProducts item in _groceryListItemsService.GetBestSellingProducts())
            {
                Products.Add(item);
            }
        }

        public override void OnAppearing()
        {
            Load();
        }

        public override void OnDisappearing()
        {
            Products.Clear();
        }
    }
}
