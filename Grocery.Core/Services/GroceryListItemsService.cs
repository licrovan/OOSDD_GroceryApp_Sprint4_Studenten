using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class GroceryListItemsService : IGroceryListItemsService
    {
        private readonly IGroceryListItemsRepository _groceriesRepository;
        private readonly IProductRepository _productRepository;

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
            // list of bought products from grocerylistimtesrepo
            List<GroceryListItem> boughtItems = _groceriesRepository.GetAll();

            // dictionary of product and number of times bought
            Dictionary<int, int> saleCounts = new();

            foreach (GroceryListItem item in boughtItems)
            {

                int id = item.ProductId;
                int sales = item.Amount;

                if (saleCounts.ContainsKey(id))
                {
                    saleCounts[id] = saleCounts[id] + sales;
                }
                else
                {
                    saleCounts.Add(id, sales);
                }

            }

            return sortPickSales(topX, ref saleCounts);

        }

        private List<BestSellingProducts> sortPickSales(int topX, ref Dictionary<int, int> saleCounts)
        {
            saleCounts = saleCounts.OrderByDescending(x => x.Value).Take(topX).ToDictionary(x => x.Key, x => x.Value);

            // create list of best selling products to return
            List<BestSellingProducts> bestSelling = new();

            foreach (KeyValuePair<int, int> kvp in saleCounts)
            {
                // find product asocciated with product id (key)
                Product? product = _productRepository.Get(kvp.Key);

                BestSellingProducts bestProduct = new(kvp.Key, product.Name, product.Stock, kvp.Value, bestSelling.Count + 1);

                bestSelling.Add(bestProduct);
            }

            return bestSelling;
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
