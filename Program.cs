// See https://aka.ms/new-console-template for more information
using System.Text.Json;
using Coflnet.Payments.Client.Api;

Console.WriteLine("Applying");

var productsApi = new ProductsApi("http://" + Environment.GetEnvironmentVariable("payment_host"));
var data = JsonSerializer.Deserialize<Format>(
        File.ReadAllText("products.json"),
        new JsonSerializerOptions(){PropertyNameCaseInsensitive=true});
Console.Write(JsonSerializer.Serialize(data));
if (data == null)
    throw new Exception("products.json is invalid");
foreach (var item in data.Products)
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
var existingTopups = (await productsApi.ProductsTopupGetAsync(0,10_000));

foreach (var item in data.TopUps)
{
    var existing = existingTopups.Where(e=>e.Slug == item.Slug).FirstOrDefault();
    if (existing == null)
    {
        // new 
        await productsApi.ProductsTopupPostAsync(item);
        Console.WriteLine("Added topup " + item.Slug);
    }
    else if (!item.Equals(existing))
    {
        // something changed, update
        await productsApi.ProductsTopupPutAsync(item);
        Console.WriteLine("Patched topup " + item.Slug);
    }
}

Console.WriteLine("Applied all, have a nice day");

public class Format
{
    public IEnumerable<Coflnet.Payments.Client.Model.PurchaseableProduct> Products {get;set;}
    public IEnumerable<Coflnet.Payments.Client.Model.TopUpProduct> TopUps {get;set;}
}