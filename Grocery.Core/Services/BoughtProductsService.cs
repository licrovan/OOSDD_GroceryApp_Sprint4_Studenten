
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class BoughtProductsService : IBoughtProductsService
    {
        private readonly IGroceryListItemsRepository _groceryListItemsRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IProductRepository _productRepository;
        private readonly IGroceryListRepository _groceryListRepository;
        public BoughtProductsService(IGroceryListItemsRepository groceryListItemsRepository, IGroceryListRepository groceryListRepository, IClientRepository clientRepository, IProductRepository productRepository)
        {
            _groceryListItemsRepository=groceryListItemsRepository;
            _groceryListRepository=groceryListRepository;
            _clientRepository=clientRepository;
            _productRepository=productRepository;
        }
        public List<BoughtProducts> Get(int productId)
        {
            List<BoughtProducts> boughtProducts = new();
            Product product = _productRepository.Get(productId);

            // get grocerylists off all users
            List<GroceryList> groceryLists = _groceryListRepository.GetAll();
            // get all grocerylistitems
            List<GroceryListItem> groceryListItems = _groceryListItemsRepository.GetAll();

            foreach (GroceryListItem gli in groceryListItems)
            {
                if (gli.ProductId == productId)
                {
                    // get grocerylist
                    GroceryList gl = groceryLists.Find(g => g.Id == gli.GroceryListId);
                    if (gl != null)
                    {
                        // get client
                        Client client = _clientRepository.Get(gl.ClientId);
                        if (client != null)
                        {
                            BoughtProducts bp = new(client, gl, product);
                            boughtProducts.Add(bp);
                        }
                    }
                }
            }

            return boughtProducts;

        }

    }
}
