using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class GroceryListItemsService : IGroceryListItemsService
    {
        private readonly IGroceryListItemsRepository _groceriesRepository;
        private readonly IProductRepository _productRepository;
        private readonly IBoughtProductsService _boughtProductsService;

        public GroceryListItemsService(IGroceryListItemsRepository groceriesRepository, IProductRepository productRepository)
        {
            _groceriesRepository = groceriesRepository;
            _productRepository = productRepository;
        }

        public List<GroceryListItem> GetAll()
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int groceryListId)
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll().Where(g => g.GroceryListId == groceryListId).ToList();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            return _groceriesRepository.Add(item);
        }

        public GroceryListItem? Delete(GroceryListItem item)
        {
            throw new NotImplementedException();
        }

        public GroceryListItem? Get(int id)
        {
            return _groceriesRepository.Get(id);
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            return _groceriesRepository.Update(item);
        }

        public List<BestSellingProducts> GetBestSellingProducts(int topX = 5)
        {
            // list of bought products
            List<BoughtProducts> boughtProducts = _boughtProductsService.GetAll();
            // dictionary of product and number of times bought
            Dictionary<Product, int> salleCounts = new();
            foreach (BoughtProducts b in boughtProducts)
            {
                Product product = b.Product;
                if (salleCounts.ContainsKey(product))
                {
                    salleCounts[product]++;
                }
                else
                {
                    salleCounts[product] = 1;
                }
            }

            // sort dictionary by value and take topX keys
            salleCounts = salleCounts.OrderByDescending(x => x.Value).Take(topX).ToDictionary(x => x.Key, x => x.Value);

            List<BestSellingProducts> bestSellingProducts = new();
            foreach (KeyValuePair<Product, int> kvp in salleCounts)
            {
                bestSellingProducts.Add(new BestSellingProducts(kvp.Key.Id, kvp.Key.Name, kvp.Key.Stock, kvp.Value, bestSellingProducts.Count));
            }

            return bestSellingProducts;

        }

        private void FillService(List<GroceryListItem> groceryListItems)
        {
            foreach (GroceryListItem g in groceryListItems)
            {
                g.Product = _productRepository.Get(g.ProductId) ?? new(0, "", 0);
            }
        }
    }
}
