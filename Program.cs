// See https://aka.ms/new-console-template for more information
using System.Text.Json;
using Coflnet.Payments.Client.Api;
using Coflnet.Payments.Client.Model;
using Newtonsoft.Json;

Console.WriteLine("Applying");

var applyApi = new ApplyApi("http://" + Environment.GetEnvironmentVariable("PAYMENT_HOST"));
Console.WriteLine("Using instance " + applyApi.Configuration.BasePath);
var data = JsonConvert.DeserializeObject<SystemState>(
        File.ReadAllText("products.json"));
Console.Write(JsonConvert.SerializeObject(data, Formatting.Indented));
if (data == null)
    throw new Exception("products.json is invalid");

await applyApi.ApplyPostAsync(data);
Console.WriteLine("Done");

