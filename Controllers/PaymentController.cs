using BitlyAPI;
using Bitrix.Iyzipay.Models.Callback;
using Bitrix.Iyzipay.Models.Contact;
using Bitrix.Iyzipay.Models.Invoice;
using Bitrix.Iyzipay.Models.Update;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using HttpClient = System.Net.Http.HttpClient;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Bitrix.Iyzipay.Controllers;

[ApiController]
[Route("api/[controller]")]

public class PaymentController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public PaymentController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet("start-payment")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> StartPayment([FromForm] IFormCollection? data)
    {
        if (data == null) return BadRequest("Webhook Data is Null");
        if (data.ToList()[8].Value != _configuration.GetSection("BitrixAuthKey").Value) return BadRequest("Invalid AuthKey");
        //Your bitrix outbound webhook auth key

        var invoiceId = data.ToList()[1].Value;
        //invoiceId data allways comes from the first index of webhook data
        var entityId = data.ToList()[2].Value;

        var client = new HttpClient();
        client.BaseAddress = new Uri("https://yourdomain.bitrix24.com/rest");
        //the base address of your bitrix inbound webhooks

        var invoiceResponse = await client.GetAsync(client.BaseAddress +
                                                        "/00/000000000000000/crm.item.get?entityTypeId=31&id=" +
                                                        invoiceId);

        if (!invoiceResponse.IsSuccessStatusCode) return BadRequest("Invoice response data is null");
        var apiInvioiceResponse = invoiceResponse.Content.ReadAsStringAsync().Result;
        var invoiceResult = JsonConvert.DeserializeObject<InvoiceViewModel>(apiInvioiceResponse);
        //Now we have the invoice data as an object (InvoiceViewModel)

        if (invoiceResult?.Result.Item.StageId != "YourStageId") return BadRequest("Invoice Stage is wrong");
        //Create the stages for the payment url

        var contactId = invoiceResult.Result.Item.ContactId;
        var contactResponse =
            await client.GetAsync(client.BaseAddress + "/00/000000000000000/crm.contact.get?id=" + contactId);

        if (!contactResponse.IsSuccessStatusCode) return BadRequest("Contact response couldn't fetch");
        var apiContactResponse = contactResponse.Content.ReadAsStringAsync().Result;
        var contactResult = JsonConvert.DeserializeObject<ContactViewModel>(apiContactResponse);
        //Now we have the contact data as an object (ContactViewModel). We will use these data for iyzipay api

        if (contactResult == null) return BadRequest("Contact result data is null");

        //from now on we start to create the iyzipay options data
        var options = new Options
        {
            ApiKey = _configuration.GetSection("ApiKey").Value,
            SecretKey = _configuration.GetSection("SecretKey").Value,
            BaseUrl = _configuration.GetSection("BaseUrl").Value
        };
        var request = new CreateCheckoutFormInitializeRequest
        {
            Price = invoiceResult.Result.Item.Opportunity.ToString(),
            PaidPrice = invoiceResult.Result.Item.Opportunity.ToString(),
            Currency = Currency.TRY.ToString(),
            ConversationId = invoiceId.ToString(),
            CallbackUrl = _configuration.GetSection("CallBackUrl").Value
        };

        var enabledInstallments = new List<int> { 1 };
        request.EnabledInstallments = enabledInstallments;

        if (contactResult.Result.EMAIL == null)
        {
            contactResult.Result.EMAIL = new List<EMAIL>();
            var email = new EMAIL
            {
                ID = "1",
                VALUE = "mail@yourdomain.com"
            };
            contactResult.Result.EMAIL.Add(email);
        }

        if (contactResult.Result.PHONE == null)
        {
            contactResult.Result.PHONE = new List<PHONE>();
            var phone = new PHONE
            {
                ID = "1",
                VALUE = "00000000000"
            };
            contactResult.Result.PHONE.Add(phone);
        }

        var buyer = new Buyer
        {
            Id = contactResult.Result.ID,
            Name = contactResult.Result.NAME,
            Surname = contactResult.Result.LASTNAME,
            Email = contactResult.Result.EMAIL.FirstOrDefault()?.VALUE,
            IdentityNumber = "11223334455",
            RegistrationAddress = contactResult.Result.ADDRESS?.ToString(),
            Ip = "127.0.0.1",
            City = contactResult.Result.ADDRESSCITY?.ToString(),
            Country = contactResult.Result.ADDRESSCOUNTRY?.ToString(),
            GsmNumber = contactResult.Result.PHONE.FirstOrDefault()?.VALUE
        };
        request.Buyer = buyer;

        var shippingAddress = new Address
        {
            ContactName = contactResult.Result.NAME + " " + contactResult.Result.LASTNAME,
            City = contactResult.Result.ADDRESSCITY?.ToString(),
            Country = contactResult.Result.ADDRESSCOUNTRY?.ToString(),
            Description = contactResult.Result.ADDRESS?.ToString()
        };
        request.ShippingAddress = shippingAddress;

        var billingAddress = new Address
        {
            ContactName = contactResult.Result.NAME + " " + contactResult.Result.LASTNAME,
            City = contactResult.Result.ADDRESSCITY?.ToString(),
            Country = contactResult.Result.ADDRESSCOUNTRY?.ToString(),
            Description = contactResult.Result.ADDRESS?.ToString()
        };
        request.BillingAddress = billingAddress;

        var basketItems = new List<BasketItem>();
        var firstBasketItem = new BasketItem
        {
            Id = invoiceResult?.Result.Item.Id.ToString(),
            Name = invoiceResult?.Result.Item.Title,
            Category1 = invoiceResult?.Result.Item.Title,
            ItemType = BasketItemType.PHYSICAL.ToString(),
            Price = invoiceResult?.Result.Item.Opportunity.ToString()
        };
        basketItems.Add(firstBasketItem);

        request.BasketItems = basketItems;

        var checkoutFormInitialize = CheckoutFormInitialize.Create(request, options);

        if (checkoutFormInitialize.Status != "success") return BadRequest("checkoutFormInitialize data is null");

        var bitly = new Bitly(_configuration.GetSection("BitlyAuth").Value);
        var linkResponse = await bitly.PostShorten(checkoutFormInitialize.PaymentPageUrl);
        //Bonus, we used an urlshortener

        var postData = new UpdateViewModel
        {
            fields = new Models.Update.Fields
            {
                UfCrmSmartInvoice1681886423 = linkResponse.Link
            }
        };

        var json = JsonSerializer.Serialize(postData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var result =
            await client.PostAsync(
                client.BaseAddress + "/00/000000000000000/crm.item.update?entityTypeId=31&id=" + invoiceId,
                content);
        //And finish, we posted the payment url data to the bitrix invoice stage. Now you can sent this url to your customer. For 3ds payment we need a callback endpoint also.
        return Ok(result);
    }

    [HttpPost("result")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> Callback([FromForm] IFormCollection? data)
    {
        if (data == null) return BadRequest("Iyzico return data is null");
        var token = data.ToList()[0].Value;
        //Iyzipay return token is required

        var options = new Options();
        options.ApiKey = _configuration.GetSection("ApiKey").Value;
        options.SecretKey = _configuration.GetSection("SecretKey").Value;
        options.BaseUrl = _configuration.GetSection("BaseUrl").Value;

        RetrieveCheckoutFormRequest request = new RetrieveCheckoutFormRequest();
        request.Token = token;

        CheckoutForm checkoutForm = CheckoutForm.Retrieve(request, options);
        var itemId = checkoutForm.PaymentItems[0].ItemId;
        if (checkoutForm.Status == "success")
        {
            var client = new HttpClient();
            client.BaseAddress =
                new Uri(
                    "https://yourdomain.bitrix24.com/rest/00/000000000000/crm.item.update?entityTypeId=31&id=");

            var postData = new CallbackVm()
            {
                fields = new Models.Callback.Fields()
                {
                    stageId = "YourPaidStageId"
                }
            };
            //with this post data we mark your invoice as paid

            var json = JsonSerializer.Serialize(postData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var result = await client.PostAsync(client.BaseAddress + itemId, content);

            return Redirect(result.IsSuccessStatusCode
                ? "your success url"
                : "your failure url");
        }
        else
        {
            var client = new HttpClient();
            client.BaseAddress =
                new Uri(
                    "https://yourdomain.bitrix24.com/rest/00/000000000000/crm.item.update?entityTypeId=31&id=");
            var postData = new CallbackVm()
            {
                fields = new Models.Callback.Fields()
                {
                    stageId = "YourUnpaidStageId"
                }
            };
            //with this post data we mark your invoice as unpaid
            var json = JsonSerializer.Serialize(postData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await client.PostAsync(client.BaseAddress + request.ConversationId, content);

            return Redirect("your failure url");
        }
    }
}

