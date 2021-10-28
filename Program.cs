// See https://aka.ms/new-console-template for more information
using System.Text.Json;
using Coflnet.Payments.Client.Api;

Console.WriteLine("Applying");

var productsApi = new ProductsApi("http://" + Environment.GetEnvironmentVariable("payment_host"));
var products = JsonSerializer.Deserialize<IEnumerable<Coflnet.Payments.Client.Model.PurchaseableProduct>>(
        File.ReadAllText("products.json"),
        new JsonSerializerOptions(){PropertyNameCaseInsensitive=true});
Console.Write(JsonSerializer.Serialize(products));
if (products == null)
    throw new Exception("products.json is invalid");
foreach (var item in products)
{
    var existing = await productsApi.ProductsPProductSlugGetAsync(item.Slug);
    if (existing == null)
    {
        // new 
        await productsApi.ProductsPostAsync(item);
        Console.WriteLine("Added " + item.Slug);
    }
    else if (!item.Equals(existing))
    {
        // something changed, update
        await productsApi.ProductsPutAsync(item);
        Console.WriteLine("Patched " + item.Slug);
    }
}
Console.WriteLine("Applied all, have a nice day");