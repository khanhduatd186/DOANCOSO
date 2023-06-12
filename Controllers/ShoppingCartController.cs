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


namespace WebBanThu.Controllers
{
    [Authorize(Roles = "CUTOMER")]
    public class ShoppingCartController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _clientId;
        private readonly string _secretKey;


        public ShoppingCartController(IHttpContextAccessor httpContextAccessor,IConfiguration config)
        {
            _httpContextAccessor = httpContextAccessor;
            _clientId = config["PaypalSettings:ClientId"];
            _secretKey = config["PaypalSettings:SecretKey"];
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
        [Route("/cart", Name = "cart")]
        public IActionResult Cart()
        {
            ViewBag.Domain = domain;
            return View(GetCartItems());
        }
        [Route("/updatecart", Name = "updatecart")]
        [HttpPost]
        public IActionResult UpdateCart([FromForm] int productid, [FromForm] int quantity)
        {
            // Cập nhật Cart thay đổi số lượng quantity ...
            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.product.Id == productid);
            if (cartitem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cartitem.quantity = quantity;
            }
            SaveCartSession(cart);
            // Trả về mã thành công (không có nội dung gì - chỉ để Ajax gọi)
            return Ok();
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

        //[Authorize, HttpGet]
        //public IActionResult ThanhToan()
        //{
        //    return View();
        //}
    
        [HttpPost, ActionName("ThanhToan")]
        public async Task<IActionResult> ThanhToan(double price)
        {
            if (ModelState.IsValid)
            {
               
                string email = User.Identity.Name;
                ViewBag.Domain = domain;
                client.BaseAddress = new Uri(domain);
                SignUpModel user = new SignUpModel();
                string data = await client.GetStringAsync("api/Account/GetUseByEmail/" + email);
                int id = 0;
                user = JsonConvert.DeserializeObject<SignUpModel>(data);
               
                    try
                    {
                        BillModel donHang = new BillModel
                        {
                            
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

                            foreach(var item in bill)
                            {
                                if(item.Price == 0)
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
                                    }
                                    BillModel bill1 = new BillModel
                                    {
                                        Id =  item.Id,
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
                                        return RedirectToAction("Cart");
                                    }
                            }

                        }

                            

        
                        }
                        
                    }
                
                    catch (Exception ex)
                    {
                        //log

                        return View();
                    }
          
            }
            return View();
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
