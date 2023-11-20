using HienlthOnline.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using WebBanThu.Areas.Admin.Models;
using WebBanThu.Models;
using System.Text;
using System.Diagnostics;
using System.Reflection;

using RestSharp;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using WebBanThu.Interface;
using WebBanThu.Helpers;

namespace WebBanThu.Controllers
{
    [Authorize(Roles = "CUTOMER")]
    public class ShoppingCartController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _clientId;
        private readonly string _secretKey;
        private readonly IOptions<MomoOptionModel> _options;
        private readonly IEmailService _email;
     

        public ShoppingCartController(IHttpContextAccessor httpContextAccessor,IConfiguration config, IOptions<MomoOptionModel> options, IEmailService emailService)
        {
            _httpContextAccessor = httpContextAccessor;
            _clientId = config["PaypalSettings:ClientId"];
            _secretKey = config["PaypalSettings:SecretKey"];
            _options = options;
            _email = emailService;
         
        }
        string domain = "https://localhost:7253/";
        HttpClient client = new HttpClient();
        public const string CARTKEY = "cart";

        // Lấy cart từ Session (danh sách CartItem)
        List<CartItem> GetCartItems()
        {
            ViewBag.Domain = domain;
            var session = HttpContext.Session;
            string jsoncart = session.GetString(CARTKEY);
            if (jsoncart != null)
            {
                return JsonConvert.DeserializeObject<List<CartItem>>(jsoncart);
            }
            return new List<CartItem>();
        }
        void ClearCart()
        {
            ViewBag.Domain = domain;
            var session = HttpContext.Session;
            session.Remove(CARTKEY);
        }

        // Lưu Cart (Danh sách CartItem) vào session
        void SaveCartSession(List<CartItem> ls)
        {
            ViewBag.Domain = domain;
            var session = HttpContext.Session;
            string jsoncart = JsonConvert.SerializeObject(ls);
            session.SetString(CARTKEY, jsoncart);
        }
        //[Route("addcart/{productid:int}", Name = "addcart")]
        public async Task<IActionResult> AddToCart([FromRoute] int id)
        {

            ViewBag.Domain = domain;
            client.BaseAddress = new Uri(domain);
            ProductModel product = new ProductModel();
            string data = await client.GetStringAsync("api/Product/" + id);
            product = JsonConvert.DeserializeObject<ProductModel>(data);
            if (product == null)
                return NotFound("Không có sản phẩm");

            // Xử lý đưa vào Cart ...
            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.product.Id == id);
            if (cartitem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cartitem.quantity++;
            }
            else
            {
                //  Thêm mới
                cart.Add(new CartItem() { quantity = 1, product = product });
            }

            // Lưu cart vào Session
            SaveCartSession(cart);
            // Chuyển đến trang hiện thị Cart
            return RedirectToAction(nameof(Cart));
        }
        // Hiện thị giỏ hàng
       
        public IActionResult Cart()
        {
            ViewBag.Domain = domain;
            return View(GetCartItems());
        }
        [Route("/updatecart", Name = "updatecart")]
        [HttpPost]
        public  IActionResult UpdateCart([FromForm] int productid, [FromForm] int quantity)
        {
            try
            {
                client.BaseAddress = new Uri(domain);
                var cart = GetCartItems();
                var cartitem = cart.Find(p => p.product.Id == productid);
                HttpResponseMessage datajson1 = client.GetAsync("api/Product/" + productid).Result;
                string data5 = datajson1.Content.ReadAsStringAsync().Result;
                ProductModel productup = JsonConvert.DeserializeObject<ProductModel>(data5);
                if (cartitem != null && productup.Quantity > quantity)
                {
                    // Đã tồn tại, tăng thêm 1
                    cartitem.quantity = quantity;
                }
               
                SaveCartSession(cart);
                // Trả về mã thành công (không có nội dung gì - chỉ để Ajax gọi)
                return Ok();
            }
            catch
            {
                return View();
            }
            // Cập nhật Cart thay đổi số lượng quantity ...
           
        }
        [Route("/removecart/{productid:int}", Name = "removecart")]
        public IActionResult RemoveCart([FromRoute] int productid)
        {
            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.product.Id == productid);
            if (cartitem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cart.Remove(cartitem);
            }

            SaveCartSession(cart);
            return RedirectToAction(nameof(Cart));
        }
       

        [HttpPost, ActionName("ThanhToanMoMo")]
        public async Task<IActionResult> ThanhToanMoMo(OrderInfoModel model,string total)
        {
            List<string> ProductImages = new List<string>();    
            string email = User.Identity.Name;
            double price = double.Parse(total);
            ViewBag.Domain = domain;
            client.BaseAddress = new Uri(domain);
            SignUpModel user = new SignUpModel();
            string data = await client.GetStringAsync("api/Account/GetUseByEmail/" + email);
            user = JsonConvert.DeserializeObject<SignUpModel>(data);

            model.OrderId = DateTime.UtcNow.Ticks.ToString();
            model.OrderInfo = "Khách hàng: " + user.Name + ". Nội dung: " + "thanh toan don hang";
            var rawData =
                $"partnerCode={_options.Value.PartnerCode}&accessKey={_options.Value.AccessKey}&requestId={model.OrderId}&amount={price}&orderId={model.OrderId}&orderInfo={model.OrderInfo}&returnUrl={_options.Value.ReturnUrl}&notifyUrl={_options.Value.NotifyUrl}&extraData=";

            var signature = ComputeHmacSha256(rawData, _options.Value.SecretKey);

            var client1 = new RestClient(_options.Value.MomoApiUrl);
            var request = new RestRequest() { Method = Method.Post };
            request.AddHeader("Content-Type", "application/json; charset=UTF-8");

            // Create an object representing the request data
            var requestData = new
            {
                accessKey = _options.Value.AccessKey,
                partnerCode = _options.Value.PartnerCode,
                requestType = _options.Value.RequestType,
                notifyUrl = _options.Value.NotifyUrl,
                returnUrl = _options.Value.ReturnUrl,
                orderId = model.OrderId,
                amount = (price).ToString(),
                orderInfo = model.OrderInfo,
                requestId = model.OrderId,
                extraData = "",
                signature = signature
            };
            request.AddParameter("application/json", JsonConvert.SerializeObject(requestData), ParameterType.RequestBody);
            var response = await client1.ExecuteAsync(request);
            var kq = JsonConvert.DeserializeObject<MomoCreatePaymentResponseModel>(response.Content);
            return Redirect(kq.PayUrl);
            //if (response.IsSuccessStatusCode)
            //{
            //    BillModel donHang = new BillModel
            //    {
            //        Status = 0,
            //        IdUser = user.id,
            //        dateTime = DateTime.UtcNow,
            //        Price = 0
            //    };
            //    string data1 = JsonConvert.SerializeObject(donHang);
            //    StringContent content = new StringContent(data1, Encoding.UTF8, "application/json");
            //    HttpResponseMessage responseMessage = client.PostAsync("api/Bill", content).Result;
            //    if (responseMessage.IsSuccessStatusCode)
            //    {
            //        HttpResponseMessage datajson = client.GetAsync("api/Bill").Result;
            //        string data3 = datajson.Content.ReadAsStringAsync().Result;
            //        List<BillModel> bill = JsonConvert.DeserializeObject<List<BillModel>>(data3);

            //        foreach (var item in bill)
            //        {
            //            if (item.Price == 0)
            //            {
            //                foreach (var item1 in GetCartItems())
            //                {
            //                    var chitietdonhang = new Product_BillModel
            //                    {
            //                        IdBill = item.Id,
            //                        IdProduct = item1.product.Id,
            //                        Quantity = item1.quantity,
            //                        Price = item1.product.Price
            //                    };
            //                    string data2 = JsonConvert.SerializeObject(chitietdonhang);
            //                    StringContent content1 = new StringContent(data2, Encoding.UTF8, "application/json");
            //                    HttpResponseMessage responseMessage1 = client.PostAsync("api/Product_Bill", content1).Result;

            //                    HttpResponseMessage datajson1 = client.GetAsync("api/Product/" + item1.product.Id).Result;
            //                    string data5 = datajson1.Content.ReadAsStringAsync().Result;
            //                    ProductModel productup = JsonConvert.DeserializeObject<ProductModel>(data5);

            //                    ProductModel productup1 = new ProductModel
            //                    {
            //                        Id = productup.Id,
            //                        Image = productup.Image,
            //                        Price = productup.Price,
            //                        CategoryId = productup.CategoryId,
            //                        Quantity = productup.Quantity - item1.quantity,
            //                        Description = productup.Description,
            //                        Isdelete = productup.Isdelete,
            //                        Tittle = productup.Tittle
            //                    };
            //                    ProductImages.Add(productup1.Image);
            //                    string data6 = JsonConvert.SerializeObject(productup1);
            //                    StringContent content5 = new StringContent(data6, Encoding.UTF8, "application/json");
            //                    HttpResponseMessage responseMessage5 = client.PutAsync("api/Product/" + productup.Id, content5).Result;

            //                }
            //                BillModel bill1 = new BillModel
            //                {
            //                    Status = item.Status,
            //                    Id = item.Id,
            //                    IdUser = item.IdUser,
            //                    dateTime = item.dateTime,
            //                    Price = price
            //                };
            //                string data4 = JsonConvert.SerializeObject(bill1);
            //                StringContent content4 = new StringContent(data4, Encoding.UTF8, "application/json");
            //                HttpResponseMessage responseMessage4 = client.PutAsync("api/Bill/" + item.Id, content4).Result;
            //                if (responseMessage4.IsSuccessStatusCode)
            //                {
            //                    HttpContext.Session.Remove(CARTKEY);
            //                    //var kq = JsonConvert.DeserializeObject<MomoCreatePaymentResponseModel>(response.Content);
            //                    string to = email;
            //                    string subject = "Thanh toán thành công";
            //                    string body = "Thanh toán của bạn đã thành công.";
            //                    //_email.SendPaymentConfirmationEmail(to, subject, body,ProductImages);
            //                    return Redirect(kq.PayUrl);
            //                }

            //            }
            //        }

            //    }

            //}
            //else
            //{
            //    return RedirectToAction(nameof(Cart));
            //}

            return null;


        }
        public MomoExecuteResponseModel PaymentExecuteAsync([FromServices] IQueryCollection collection)
        {
            var amount = collection.First(s => s.Key == "amount").Value;
            var orderInfo = collection.First(s => s.Key == "orderInfo").Value;
            var orderId = collection.First(s => s.Key == "orderId").Value;
            int errorcode =int.Parse(collection.First(s => s.Key == "errorCode").Value);
            return new MomoExecuteResponseModel()
            {
                Amount = amount,
                OrderId = orderId,
                OrderInfo = orderInfo,
                ErrorCode = errorcode
                
            };
        }
      
        public async Task<IActionResult> PaymentcallBackAsync()
        {
            try
            {
                var response = HttpContext.Request.Query["errorCode"];
                
                if (!response.Equals("0"))
                {
                    return RedirectToAction(nameof(Cart));
                }
                else
                {
                    List<string> ProductImages = new List<string>();
                    string email = User.Identity.Name;
                    ViewBag.Domain = domain;
                    client.BaseAddress = new Uri(domain);
                    SignUpModel user = new SignUpModel();
                    string data = await client.GetStringAsync("api/Account/GetUseByEmail/" + email);
                    user = JsonConvert.DeserializeObject<SignUpModel>(data);
                    int price =int.Parse( HttpContext.Request.Query["amount"]);
                    BillModel donHang = new BillModel
                    {
                        Status = 0,
                        IdUser = user.id,
                        dateTime = DateTime.UtcNow,
                        Price = 0
                    };
                    string data1 = JsonConvert.SerializeObject(donHang);
                    StringContent content = new StringContent(data1, Encoding.UTF8, "application/json");
                    HttpResponseMessage responseMessage = client.PostAsync("api/Bill", content).Result;
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        HttpResponseMessage datajson = client.GetAsync("api/Bill").Result;
                        string data3 = datajson.Content.ReadAsStringAsync().Result;
                        List<BillModel> bill = JsonConvert.DeserializeObject<List<BillModel>>(data3);

                        foreach (var item in bill)
                        {
                            if (item.Price == 0)
                            {
                                foreach (var item1 in GetCartItems())
                                {
                                    var chitietdonhang = new Product_BillModel
                                    {
                                        IdBill = item.Id,
                                        IdProduct = item1.product.Id,
                                        Quantity = item1.quantity,
                                        Price = item1.product.Price
                                    };
                                    string data2 = JsonConvert.SerializeObject(chitietdonhang);
                                    StringContent content1 = new StringContent(data2, Encoding.UTF8, "application/json");
                                    HttpResponseMessage responseMessage1 = client.PostAsync("api/Product_Bill", content1).Result;

                                    HttpResponseMessage datajson1 = client.GetAsync("api/Product/" + item1.product.Id).Result;
                                    string data5 = datajson1.Content.ReadAsStringAsync().Result;
                                    ProductModel productup = JsonConvert.DeserializeObject<ProductModel>(data5);

                                    ProductModel productup1 = new ProductModel
                                    {
                                        Id = productup.Id,
                                        Image = productup.Image,
                                        Price = productup.Price,
                                        CategoryId = productup.CategoryId,
                                        Quantity = productup.Quantity - item1.quantity,
                                        Description = productup.Description,
                                        Isdelete = productup.Isdelete,
                                        Tittle = productup.Tittle
                                    };
                                    ProductImages.Add(productup1.Image);
                                    string data6 = JsonConvert.SerializeObject(productup1);
                                    StringContent content5 = new StringContent(data6, Encoding.UTF8, "application/json");
                                    HttpResponseMessage responseMessage5 = client.PutAsync("api/Product/" + productup.Id, content5).Result;

                                }

                                BillModel bill1 = new BillModel
                                {
                                    Status = 1,
                                    Id = item.Id,
                                    IdUser = item.IdUser,
                                    dateTime = item.dateTime,
                                    Price = price
                                };
                                string data4 = JsonConvert.SerializeObject(bill1);
                                StringContent content4 = new StringContent(data4, Encoding.UTF8, "application/json");
                                HttpResponseMessage responseMessage4 = client.PutAsync("api/Bill/" + item.Id, content4).Result;
                                if (responseMessage4.IsSuccessStatusCode)
                                {
                                    HttpContext.Session.Remove(CARTKEY);
                                    //var kq = JsonConvert.DeserializeObject<MomoCreatePaymentResponseModel>(response.Content);
                                    string to = email;
                                    string subject = "Thanh toán thành công";
                                    string body = "Thanh toán của bạn đã thành công.";
                                    //_email.SendPaymentConfirmationEmail(to, subject, body,ProductImages);
                                    return RedirectToAction(nameof(Cart));

                                }
                                
                            }
                        }

                    }
                }
                return BadRequest();
            }
            catch
            {
                return BadRequest();
            }


        }
        [HttpPost, ActionName("ThanhToan")]
        public async Task<IActionResult> ThanhToan(string total)
        {
            List<string> ProductImages = new List<string>();
            string email = User.Identity.Name;
            ViewBag.Domain = domain;
            client.BaseAddress = new Uri(domain);
            SignUpModel user = new SignUpModel();
            string data = await client.GetStringAsync("api/Account/GetUseByEmail/" + email);
            user = JsonConvert.DeserializeObject<SignUpModel>(data);
           
            BillModel donHang = new BillModel
            {
                Status = 0,
                IdUser = user.id,
                dateTime = DateTime.UtcNow,
                Price = 0
            };
            string data1 = JsonConvert.SerializeObject(donHang);
            StringContent content = new StringContent(data1, Encoding.UTF8, "application/json");
            HttpResponseMessage responseMessage = client.PostAsync("api/Bill", content).Result;
            if (responseMessage.IsSuccessStatusCode)
            {
                HttpResponseMessage datajson = client.GetAsync("api/Bill").Result;
                string data3 = datajson.Content.ReadAsStringAsync().Result;
                List<BillModel> bill = JsonConvert.DeserializeObject<List<BillModel>>(data3);

                foreach (var item in bill)
                {
                    if (item.Price == 0)
                    {
                        foreach (var item1 in GetCartItems())
                        {
                            var chitietdonhang = new Product_BillModel
                            {
                                IdBill = item.Id,
                                IdProduct = item1.product.Id,
                                Quantity = item1.quantity,
                                Price = item1.product.Price
                            };
                            string data2 = JsonConvert.SerializeObject(chitietdonhang);
                            StringContent content1 = new StringContent(data2, Encoding.UTF8, "application/json");
                            HttpResponseMessage responseMessage1 = client.PostAsync("api/Product_Bill", content1).Result;

                            HttpResponseMessage datajson1 = client.GetAsync("api/Product/" + item1.product.Id).Result;
                            string data5 = datajson1.Content.ReadAsStringAsync().Result;
                            ProductModel productup = JsonConvert.DeserializeObject<ProductModel>(data5);

                            ProductModel productup1 = new ProductModel
                            {
                                Id = productup.Id,
                                Image = productup.Image,
                                Price = productup.Price,
                                CategoryId = productup.CategoryId,
                                Quantity = productup.Quantity - item1.quantity,
                                Description = productup.Description,
                                Isdelete = productup.Isdelete,
                                Tittle = productup.Tittle
                            };
                            ProductImages.Add(productup1.Image);
                            string data6 = JsonConvert.SerializeObject(productup1);
                            StringContent content5 = new StringContent(data6, Encoding.UTF8, "application/json");
                            HttpResponseMessage responseMessage5 = client.PutAsync("api/Product/" + productup.Id, content5).Result;

                        }

                        BillModel bill1 = new BillModel
                        {
                            Status = item.Status,
                            Id = item.Id,
                            IdUser = item.IdUser,
                            dateTime = item.dateTime,
                            Price =double.Parse(total)
                        };
                        string data4 = JsonConvert.SerializeObject(bill1);
                        StringContent content4 = new StringContent(data4, Encoding.UTF8, "application/json");
                        HttpResponseMessage responseMessage4 = client.PutAsync("api/Bill/" + item.Id, content4).Result;
                        if (responseMessage4.IsSuccessStatusCode)
                        {
                            HttpContext.Session.Remove(CARTKEY);
                            //var kq = JsonConvert.DeserializeObject<MomoCreatePaymentResponseModel>(response.Content);
                            string to = email;
                            string subject = "Thanh toán thành công";
                            string body = "Thanh toán của bạn đã thành công.";
                            //_email.SendPaymentConfirmationEmail(to, subject, body,ProductImages);

                        }
                        
                    }
                }

            }
            return RedirectToAction(nameof(Cart));
        }
        private string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            byte[] hashBytes;

            using (var hmac = new HMACSHA256(keyBytes))
            {
                hashBytes = hmac.ComputeHash(messageBytes);
            }

            var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return hashString;
        }
        public double TyGiaUSD = 23300;//store in Database
        //[Authorize]
        //public async System.Threading.Tasks.Task<IActionResult> PaypalCheckout()
        //{
        //    var environment = new SandboxEnvironment(_clientId, _secretKey);
        //    var client = new PayPalHttpClient(environment);

        //    #region Create Paypal Order
        //    var itemList = new ItemList()
        //    {
        //        items = new List<Item>()
        //    };
        //    List<CartItem> carts = GetCartItems();
        //    var total = Math.Round(carts.Sum(p => (p.product.Price*p.quantity)) / TyGiaUSD, 2);
        //    foreach (var item in carts)
        //    {
        //        itemList.items.Add(new Item()
        //        {
        //            name = item.product.Tittle,
        //            currency = "USD",
        //            price = Math.Round(item.product.Price / TyGiaUSD, 2).ToString(),
        //            quantity = item.quantity.ToString(),
        //            sku = "sku",
        //            tax = "0"
        //        });
        //    }
        //    #endregion

        //    var paypalOrderId = DateTime.Now.Ticks;
        //    var hostname = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
        //    var payment = new Payment()
        //    {
        //        intent = "sale",
        //        transactions = new List<Transaction>()
        //        {
        //            new Transaction()
        //            {
        //                amount = new Amount()
        //                {
        //                    total = total.ToString(),
        //                    currency = "USD",
        //                    details = new Details
        //                    {
        //                        tax = "0",
        //                        shipping = "0",
        //                        subtotal = total.ToString()
        //                    }
                            
        //                },
        //                item_list = itemList,
        //                description = $"Invoice #{paypalOrderId}",
        //                invoice_number = paypalOrderId.ToString()
        //            }
        //        },
        //        redirect_urls = new RedirectUrls()
        //        {
        //            cancel_url = $"{hostname}/ShoppingCart/CheckoutFail",
        //            return_url = $"{hostname}/ShoppingCart/CheckoutSuccess"
        //        },
        //        payer = new Payer()
        //        {
        //          payment_method  = "paypal"
        //        }
        //    };

        //    OrdersCreateRequest request = new OrdersCreateRequest();
        //    request.RequestBody(payment);

        //    try
        //    {
        //        var response = await client.Execute(request);
        //        var statusCode = response.StatusCode;
        //        Payment result = response.Result<Payment>();

        //        var links = result.links.GetEnumerator();
        //        string paypalRedirectUrl = null;
        //        while (links.MoveNext())
        //        {
        //            LinkDescriptionObject lnk = links.Current;
        //            if (lnk.Rel.ToLower().Trim().Equals("approval_url"))
        //            {
        //                //saving the payapalredirect URL to which user will be redirected for payment  
        //                paypalRedirectUrl = lnk.Href;
        //            }
        //        }

        //        return Redirect(paypalRedirectUrl);
        //    }
        //    catch (HttpException httpException)
        //    {
        //        var statusCode = httpException.StatusCode;
        //        var debugId = httpException.Headers.GetValues("PayPal-Debug-Id").FirstOrDefault();

        //        //Process when Checkout with Paypal fails
        //        return Redirect("/ShoppingCart/CheckoutFail");
        //    }
        //}

        public IActionResult CheckoutFail()
        {
            //Tạo đơn hàng trong database với trạng thái thanh toán là "Chưa thanh toán"
            //Xóa session
            return View();
        }

        public IActionResult CheckoutSuccess()
        {
            //Tạo đơn hàng trong database với trạng thái thanh toán là "Paypal" và thành công
            //Xóa session
            return View();
        }
        //public IActionResult Index()
        //{
        //    ViewBag.Domain = domain;
        //    return View(Carts);
        //}
        //public List<CartItem> Carts
        //{
        //    get
        //    {
        //        var data = HttpContext.Session.Get<List<CartItem>>("GioHang");
        //        if (data == null)
        //        {
        //            data = new List<CartItem>();
        //        }
        //        return data;
        //    }
        //}
        //public async Task<IActionResult> AddToCart(int id, int SoLuong, string type = "Normal")
        //{
        //    List<CartItem> myCart = Carts;
        //    var item = myCart.SingleOrDefault(p => p.Id == id);

        //    if (item == null)//chưa có
        //    {
        //        ViewBag.Domain = domain;
        //        client.BaseAddress = new Uri(domain);
        //        ProductModel product = new ProductModel();
        //        string data = await client.GetStringAsync("api/Product/" + id);
        //        product = JsonConvert.DeserializeObject<ProductModel>(data);
        //        item = new CartItem
        //        {
        //            Id = product.Id,
        //            Tittle = product.Tittle,
        //            Price = product.Price,
        //            Quantity = SoLuong,
        //            Image = product.Image
        //        };
        //        myCart.Add(item);
        //    }
        //    else
        //    {
        //        item.Quantity += SoLuong;
        //    }
        //    HttpContext.Session.Set("GioHang", myCart);
        //    return RedirectToAction("Index");
        //}


        //[HttpGet]
        //public IActionResult GetListItems()
        //{
        //    ViewBag.Domain = domain;
        //    var session = HttpContext.Session.GetString("cart");
        //    List<CartItem> currentCart = new List<CartItem>();
        //    if (session != null)
        //        currentCart = JsonConvert.DeserializeObject<List<CartItem>>(session);
        //    return Ok(currentCart);
        //}

        //public async Task<IActionResult> AddToCart(int id)
        //{
        //    ViewBag.Domain = domain;
        //    client.BaseAddress = new Uri(domain);
        //    ProductModel product = new ProductModel();
        //    string data = await client.GetStringAsync("api/Product/" + id);
        //    product = JsonConvert.DeserializeObject<ProductModel>(data);

        //    var session = HttpContext.Session.GetString("cart");
        //    List<CartItem> currentCart = new List<CartItem>();
        //    if (session != null)
        //        currentCart = JsonConvert.DeserializeObject<List<CartItem>>(session);

        //    int quantity = 1;
        //    if (currentCart.Any(x => x.Product.Id == id))
        //    {
        //        quantity = currentCart.First(x => x.Product.Id == id).Quantity + 1;
        //    }

        //    var cartItem = new CartItem()
        //    {
        //        Product = product,
        //        Quantity = quantity
        //    };

        //    currentCart.Add(cartItem);

        //    HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(currentCart));
        //    return Ok(currentCart);
        //}

        //public IActionResult UpdateCart(int id, int quantity)
        //{
        //    ViewBag.Domain = domain;
        //    var session = HttpContext.Session.GetString("cart");
        //    List<CartItem> currentCart = new List<CartItem>();
        //    if (session != null)
        //        currentCart = JsonConvert.DeserializeObject<List<CartItem>>(session);

        //    foreach (var item in currentCart)
        //    {
        //        if (item.Product.Id == id)
        //        {
        //            if (quantity == 0)
        //            {
        //                currentCart.Remove(item);
        //                break;
        //            }
        //            item.Quantity = quantity;
        //        }
        //    }

        //    HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(currentCart));
        //    return Ok(currentCart);
        //}
    }
}
