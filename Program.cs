// See https://aka.ms/new-console-template for more information
using System.Text.Json;
using Coflnet.Payments.Client.Api;
using Coflnet.Payments.Client.Model;

Console.WriteLine("Applying");

var productsApi = new ProductsApi("http://" + Environment.GetEnvironmentVariable("payment_host"));
var data = JsonSerializer.Deserialize<Format>(
        File.ReadAllText("products.json"),
        new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
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
    else if (!Format.AreSame(item, existing))
    {
        // something changed, update
        await productsApi.ProductsPutAsync(item);
        Console.WriteLine("Patched " + item.Slug);
        Console.WriteLine(JsonSerializer.Serialize(item, new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine(JsonSerializer.Serialize(existing, new JsonSerializerOptions { WriteIndented = true }));
    }
}
var existingTopups = (await productsApi.ProductsTopupGetAsync(0, 10_000));

foreach (var item in data.TopUps)
{
    var existing = existingTopups?.Where(e => e.Slug == item.Slug).FirstOrDefault();
    if (existing == null)
    {
        // new 
        await productsApi.ProductsTopupPostAsync(item);
        Console.WriteLine("Added topup " + item.Slug);
    }
    else if (!Format.AreSame(item, existing))
    {
        // something changed, update
        await productsApi.ProductsTopupPutAsync(item);
        Console.WriteLine("Patched topup " + item.Slug);
    }
}

Console.WriteLine("\nApplied all, have a nice day");

public class Format
{
    public IEnumerable<Coflnet.Payments.Client.Model.PurchaseableProduct> Products { get; set; }
    public IEnumerable<Coflnet.Payments.Client.Model.TopUpProduct> TopUps { get; set; }

    public static bool AreSame(PurchaseableProduct item, PurchaseableProduct existing)
    {
        return item.OwnershipSeconds == existing.OwnershipSeconds 
                && item.Title == existing.Title 
                && item.Cost == existing.Cost
                && item.Type.GetValueOrDefault(0) == existing.Type.GetValueOrDefault(0)
                && item.Description == existing.Description;
    }

    public static bool AreSame(TopUpProduct item, TopUpProduct existing)
    {
        return item.OwnershipSeconds == existing.OwnershipSeconds 
                && item.Title == existing.Title 
                && item.Cost == existing.Cost
                && item.Type.GetValueOrDefault(0) == existing.Type.GetValueOrDefault(0)
                && item.Description == existing.Description;
    }
}

