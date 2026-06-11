using System.Net;
using System.Net.Http.Json;
using HelperZaOptimalnuKupnju.DTOs;
using HelperZaOptimalnuKupnju.Models;
using Xunit;

namespace HelperZaOptimalnuKupnju.Tests;

public sealed class ApiCrudTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient _client;

    public ApiCrudTests(ApiTestFactory factory)
    {
        _client = factory.CreateClient();
    }

    public static IEnumerable<object[]> SuccessCases()
    {
        yield return new object[] { "api/ApiProducts", NewProduct(), NewProductEdit(), (Action<object, int>)((dto, id) => ((ProductEditDTO)dto).Id = id) };
        yield return new object[] { "api/ApiStores", NewStore(), NewStoreEdit(), (Action<object, int>)((dto, id) => ((StoreEditDTO)dto).Id = id) };
        yield return new object[] { "api/ApiUsers", NewUser(), NewUserEdit(), (Action<object, int>)((dto, id) => ((UserEditDTO)dto).Id = id) };
        yield return new object[] { "api/ApiOrders", NewOrder(), NewOrderEdit(), (Action<object, int>)((dto, id) => ((OrderEditDTO)dto).Id = id) };
        yield return new object[] { "api/ApiOrderItems", NewOrderItem(), NewOrderItemEdit(), (Action<object, int>)((dto, id) => ((OrderItemEditDTO)dto).Id = id) };
    }

    public static IEnumerable<object[]> MissingIdCases()
    {
        yield return new object[] { "api/ApiProducts", NewProductEdit(), (Action<object, int>)((dto, id) => ((ProductEditDTO)dto).Id = id) };
        yield return new object[] { "api/ApiStores", NewStoreEdit(), (Action<object, int>)((dto, id) => ((StoreEditDTO)dto).Id = id) };
        yield return new object[] { "api/ApiUsers", NewUserEdit(), (Action<object, int>)((dto, id) => ((UserEditDTO)dto).Id = id) };
        yield return new object[] { "api/ApiOrders", NewOrderEdit(), (Action<object, int>)((dto, id) => ((OrderEditDTO)dto).Id = id) };
        yield return new object[] { "api/ApiOrderItems", NewOrderItemEdit(), (Action<object, int>)((dto, id) => ((OrderItemEditDTO)dto).Id = id) };
    }

    public static IEnumerable<object[]> BadRequestCases()
    {
        yield return new object[] { "api/ApiProducts", new ProductCreateDTO { Name = "", Description = "Description", UnitPrice = 1m }, NewProductEdit(), (Action<object, int>)((dto, id) => ((ProductEditDTO)dto).Id = id) };
        yield return new object[] { "api/ApiStores", new StoreCreateDTO { Name = "", Brand = "Brand", Address = "Address", City = "Zagreb", Country = "HR", OpeningHours = "08-16" }, NewStoreEdit(), (Action<object, int>)((dto, id) => ((StoreEditDTO)dto).Id = id) };
        yield return new object[] { "api/ApiUsers", new UserCreateDTO { FirstName = "Duplicate", LastName = "User", Email = "ivan.horvat@email.com", Role = UserRole.Buyer, Phone = "+38593333333" }, NewUserEdit(), (Action<object, int>)((dto, id) => ((UserEditDTO)dto).Id = id) };
        yield return new object[] { "api/ApiOrders", new OrderCreateDTO { BuyerId = 999999, ExpectedDeliveryDateTime = DateTime.UtcNow.AddDays(2) }, NewOrderEdit(), (Action<object, int>)((dto, id) => ((OrderEditDTO)dto).Id = id) };
        yield return new object[] { "api/ApiOrderItems", new OrderItemCreateDTO { OrderId = 999999, ProductId = 1, Quantity = 1, UnitPrice = 1m }, NewOrderItemEdit(), (Action<object, int>)((dto, id) => ((OrderItemEditDTO)dto).Id = id) };
    }

    [Theory]
    [MemberData(nameof(SuccessCases))]
    public async Task Crud_success_scenarios_work(string route, object createDto, object editDto, Action<object, int> setEditId)
    {
        Assert.Equal(HttpStatusCode.OK, (await _client.GetAsync(route)).StatusCode);

        var createResponse = await _client.PostAsJsonAsync(route, createDto);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<IdResponse>();
        Assert.NotNull(created);
        Assert.True(created.Id > 0);

        Assert.Equal(HttpStatusCode.OK, (await _client.GetAsync($"{route}/{created.Id}")).StatusCode);

        setEditId(editDto, created.Id);
        Assert.Equal(HttpStatusCode.OK, (await _client.PutAsJsonAsync($"{route}/{created.Id}", editDto)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await _client.DeleteAsync($"{route}/{created.Id}")).StatusCode);
    }

    [Theory]
    [MemberData(nameof(MissingIdCases))]
    public async Task Crud_returns_not_found_for_missing_ids(string route, object editDto, Action<object, int> setEditId)
    {
        const int missingId = 999999;

        Assert.Equal(HttpStatusCode.NotFound, (await _client.GetAsync($"{route}/{missingId}")).StatusCode);

        setEditId(editDto, missingId);
        Assert.Equal(HttpStatusCode.NotFound, (await _client.PutAsJsonAsync($"{route}/{missingId}", editDto)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await _client.DeleteAsync($"{route}/{missingId}")).StatusCode);
    }

    [Theory]
    [MemberData(nameof(BadRequestCases))]
    public async Task Crud_returns_bad_request_for_validation_or_business_errors(string route, object invalidCreateDto, object editDto, Action<object, int> setEditId)
    {
        Assert.Equal(HttpStatusCode.BadRequest, (await _client.PostAsJsonAsync(route, invalidCreateDto)).StatusCode);

        setEditId(editDto, 1);
        Assert.Equal(HttpStatusCode.BadRequest, (await _client.PutAsJsonAsync($"{route}/999999", editDto)).StatusCode);
    }

    [Fact]
    public async Task Complaints_crud_success_and_error_scenarios_work()
    {
        Assert.Equal(HttpStatusCode.OK, (await _client.GetAsync("api/complaints")).StatusCode);

        using var form = new MultipartFormDataContent
        {
            { new StringContent("Test complaint"), "title" },
            { new StringContent("Complaint description"), "description" }
        };

        var createResponse = await _client.PostAsync("api/complaints", form);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<IdResponse>();
        Assert.NotNull(created);
        Assert.True(created.Id > 0);

        Assert.Equal(HttpStatusCode.OK, (await _client.DeleteAsync($"api/complaints/{created.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await _client.DeleteAsync("api/complaints/999999")).StatusCode);

        using var invalidForm = new MultipartFormDataContent
        {
            { new StringContent(""), "title" },
            { new StringContent("Complaint description"), "description" }
        };

        Assert.Equal(HttpStatusCode.BadRequest, (await _client.PostAsync("api/complaints", invalidForm)).StatusCode);
    }

    private static ProductCreateDTO NewProduct() =>
        new() { Name = "Test product", Description = "Description", UnitPrice = 12.5m };

    private static ProductEditDTO NewProductEdit() =>
        new() { Name = "Updated product", Description = "Updated", UnitPrice = 20m };

    private static StoreCreateDTO NewStore() =>
        new() { Name = "Test store", Brand = "Brand", Address = "Address", City = "Zagreb", Country = "HR", OpeningHours = "08-16" };

    private static StoreEditDTO NewStoreEdit() =>
        new() { Name = "Updated store", Brand = "Brand 2", Address = "Address 2", City = "Split", Country = "HR", OpeningHours = "09-17" };

    private static UserCreateDTO NewUser() =>
        new() { FirstName = "Test", LastName = "Buyer", Email = "crud-user@example.com", Role = UserRole.Buyer, Phone = "+38591111111" };

    private static UserEditDTO NewUserEdit() =>
        new() { FirstName = "Updated", LastName = "Buyer", Email = "crud-user-updated@example.com", Role = UserRole.Buyer, Phone = "+38592222222", IsActive = true };

    private static OrderCreateDTO NewOrder() =>
        new()
        {
            BuyerId = 1,
            ExpectedDeliveryDateTime = DateTime.UtcNow.AddDays(2),
            Items = new List<OrderItemCreateDTO> { new() { ProductId = 1, Quantity = 2, UnitPrice = 0 } }
        };

    private static OrderEditDTO NewOrderEdit() =>
        new() { ExpectedDeliveryDateTime = DateTime.UtcNow.AddDays(3), Status = OrderStatus.Confirmed };

    private static OrderItemCreateDTO NewOrderItem() =>
        new() { OrderId = 1, ProductId = 1, Quantity = 3, UnitPrice = 2.5m };

    private static OrderItemEditDTO NewOrderItemEdit() =>
        new() { Quantity = 4, UnitPrice = 3.5m };

    private sealed class IdResponse
    {
        public int Id { get; set; }
    }
}
