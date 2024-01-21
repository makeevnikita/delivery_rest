using delivery;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



namespace tests;

public class ApiWebApplicationFactory : WebApplicationFactory<Startup>
{
    protected override IWebHostBuilder CreateWebHostBuilder()
    {
        return WebHost.CreateDefaultBuilder(null)
                .UseStartup<Startup>();
    }
}

public class Tests
{

    private ApiWebApplicationFactory factory;

    private HttpClient httpClient;

    [SetUp]
    public void Setup()
    {
        factory = new ApiWebApplicationFactory();
        httpClient = factory.CreateClient();
    }

    [Test]
    public async Task Create_Product_Returns_Success_Test()
    {   
        var multipartFormContent = new MultipartFormDataContent();
        
        var fileStreamContent = new StreamContent(File.OpenRead(
            "/home/superuser/Рабочий стол/delivery/tests/1-09.png"
        ));

        fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue($"image/png");

        multipartFormContent.Add(fileStreamContent, name: "Image", fileName: "1-09.png");
        multipartFormContent.Add(new StringContent("Томат"), "Name");
        multipartFormContent.Add(new StringContent("119"), "Price");
        multipartFormContent.Add(new StringContent("Description"), "Description");
        multipartFormContent.Add(new StringContent("1"), "CategoryId");
        multipartFormContent.Add(new StringContent("true"), "IsActive");

        var httpResponse = await httpClient.PostAsync("/product/create", multipartFormContent);

        var streamReader = new StreamReader(await httpResponse.Content.ReadAsStreamAsync());
        var jsonReader = new JsonTextReader(streamReader);

        JsonSerializer serializer = new JsonSerializer();

        try
        {
            JObject json = JObject.Parse(serializer.Deserialize(jsonReader).ToString());

            StringAssert.AreEqualIgnoringCase(json["result"].ToString(), "success");
        }
        catch (JsonReaderException ex)
        {
            throw ex;
        }
    }

    [Test]
    public async Task Get_Products_Test()
    {
        var httpResponse = await httpClient.GetAsync("/product/get");

        var streamReader = new StreamReader(await httpResponse.Content.ReadAsStreamAsync());
        var jsonReader = new JsonTextReader(streamReader);

        JsonSerializer serializer = new JsonSerializer();

        try
        {
            JObject json = JObject.Parse(serializer.Deserialize(jsonReader).ToString());

            var products = json["Products"].ToList();
            Assert.That(products.Count() == Convert.ToInt32(json["PageSize"].ToString()));
            
        }
        catch (JsonReaderException ex)
        {
            throw ex;
        }
    }
}