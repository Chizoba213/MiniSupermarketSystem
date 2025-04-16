using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using MiniSupermarketSystem.Application.Order.Dtos;
using MiniSupermarketSystem.Application.Product.Dtos;
using MiniSupermarketSystem.ConsoleApp;

class Program
{
    private static IMemoryCache _cache;
    private static HttpClient _httpClient;
    private static ConsoleAuthHandler _authHandler;

    static async Task Main(string[] args)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7235/api/")
        };
        _cache = new MemoryCache(new MemoryCacheOptions());
        _authHandler = new ConsoleAuthHandler(_httpClient, _cache);

        if (!await InitializeAuthentication())
        {
            Console.WriteLine("Exiting application...");
            return;
        }

        await MainMenu();
    }

    private static async Task<bool> InitializeAuthentication()
    {
        if (!await _authHandler.AuthenticateUser())
        {
            Console.WriteLine("Authentication failed.");
            return false;
        }

        if (!_cache.TryGetValue("auth_token", out string token) || string.IsNullOrEmpty(token))
        {
            Console.WriteLine("Invalid token. Please login again.");
            return false;
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return true;
    }


    private static async Task MainMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Mini Supermarket System");
            Console.WriteLine("1. View Products");
            Console.WriteLine("2. Make Purchase");
            Console.WriteLine("3. Check Order Status");
            Console.WriteLine("4. Exit");
            Console.Write("Select an option: ");

            var option = Console.ReadLine();

            try
            {
                switch (option)
                {
                    case "1":
                        await EnsureAuthenticatedAndExecute(ViewProducts);
                        break;
                    case "2":
                        await EnsureAuthenticatedAndExecute(MakePurchase);
                        break;
                    case "3":
                        await EnsureAuthenticatedAndExecute(CheckOrderStatus);
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Operation cancelled due to authentication failure.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }

    private static async Task EnsureAuthenticatedAndExecute(Func<Task> action)
    {
        if (_httpClient.DefaultRequestHeaders.Authorization == null)
        {
            if (!await InitializeAuthentication())
            {
                throw new UnauthorizedAccessException();
            }
        }

        try
        {
            await action();
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            Console.WriteLine("Session expired. Attempting to re-authenticate...");
            _cache.Remove("auth_token");
            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!await InitializeAuthentication())
            {
                throw new UnauthorizedAccessException();
            }

            await action(); // Retry the operation
        }
    }

    private static async Task ViewProducts()
    {
        var response = await _httpClient.GetAsync("products");
        response.EnsureSuccessStatusCode();

        var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();

        Console.WriteLine("\nAvailable Products:");
        Console.WriteLine("ID\tName\t\tPrice\tStock\tDescription");
        foreach (var product in products)
        {
            Console.WriteLine($"{product.Id}\t{product.Name}\t{product.Price:C}\t{product.QuantityInStock}\t{product.Description}");
        }
    }

    private static async Task MakePurchase()
    {
        await ViewProducts();

        Console.WriteLine("\nEnter product ID and quantity (format: id,quantity). Enter 'done' when finished:");

        var items = new List<OrderItemDto>();
        while (true)
        {
            Console.Write("Add item: ");
            var input = Console.ReadLine();
            if (input?.ToLower() == "done") break;

            var parts = input?.Split(',');
            if (parts?.Length != 2 || !int.TryParse(parts[0], out int id) || !int.TryParse(parts[1], out int qty))
            {
                Console.WriteLine("Invalid format. Use 'id,quantity'");
                continue;
            }

            items.Add(new OrderItemDto { ProductId = id, Quantity = qty });
        }

        if (!items.Any())
        {
            Console.WriteLine("No items added to order");
            return;
        }

        var orderDto = new CreateOrderDto { Items = items };
        var response = await _httpClient.PostAsJsonAsync("orders", orderDto);
        response.EnsureSuccessStatusCode();

        var order = await response.Content.ReadFromJsonAsync<OrderDto>();

        Console.WriteLine("\nOrder Created Successfully!");
        Console.WriteLine($"Transaction Reference: {order.TransactionReference}");
        Console.WriteLine($"Total Amount: {order.TotalAmount:C}");
        Console.WriteLine($"Bank Account for Payment: {order.BankAccountNumber}");
        Console.WriteLine($"Bank Account Name: {order.BankAccountName}");
        Console.WriteLine($"Bank Name: {order.BankName}");
        Console.WriteLine("\nPlease transfer the exact amount to the provided bank account.");
        Console.WriteLine("Your order will be processed after payment is confirmed.");
    }

    private static async Task CheckOrderStatus()
    {
        Console.Write("Enter transaction reference: ");
        var reference = Console.ReadLine();

        var response = await _httpClient.GetAsync($"orders/{reference}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            Console.WriteLine("Order not found");
            return;
        }

        response.EnsureSuccessStatusCode();
        var order = await response.Content.ReadFromJsonAsync<OrderDto>();

        Console.WriteLine("\nOrder Details:");
        Console.WriteLine($"Reference: {order.TransactionReference}");
        Console.WriteLine($"Date: {order.OrderDate}");
        Console.WriteLine($"Status: {order.PaymentStatus}");
        Console.WriteLine($"Total: {order.TotalAmount:C}");
        Console.WriteLine($"PaymnetDate: {order.PaymentDate}");
        Console.WriteLine($"Amount Paid: {order.AmountPaid:C}");


        Console.WriteLine("\nItems:");
        foreach (var item in order.OrderDetails)
        {
            Console.WriteLine($"{item.ProductName} - {item.Quantity} x {item.UnitPrice:C} = {item.Quantity * item.UnitPrice:C}");
        }
    }
}