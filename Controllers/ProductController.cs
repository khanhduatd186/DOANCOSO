using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
using WebBanThu.Areas.Admin.Models;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;
using WebBanThu.Models;

namespace WebBanThu.Controllers
{
	public class ProductController : Controller
	{
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        string domain = "https://localhost:7253/";
		HttpClient client = new HttpClient();

		public async Task<IActionResult> Index()
		{
			ViewBag.Domain = domain;
			client.BaseAddress = new Uri(domain);
			string datajson = await client.GetStringAsync("api/Product/GET_ALL");
			List<ProductModel> products = JsonConvert.DeserializeObject<List<ProductModel>>(datajson);
			
            string datajson1 = await client.GetStringAsync("/api/Category/GET_ALL");
            List<CategoryModel> categories = JsonConvert.DeserializeObject<List<CategoryModel>>(datajson1);

            ViewBag.Products = products;
			ViewBag.Categories = categories;

            return View();
		}

        public async Task<IActionResult> Create()
		{

			client.BaseAddress = new Uri(domain);
			string datajson = await client.GetStringAsync("api/Category/GET_ALL");
			List<CategoryModel> categories = JsonConvert.DeserializeObject<List<CategoryModel>>(datajson);
			ViewBag.CategoryId = new SelectList(categories, "Id", "Tittle");
			return View();

		}
		[HttpPost]
		[ValidateAntiForgeryToken]

		public async Task<IActionResult> Create(ProductModel p, IFormFile fileimage)
		{
			//client.BaseAddress = new Uri(domain);

			//ProductModel product = new ProductModel();
			//using(var http = new HttpClient())
			//{
			//    var formdata = new MultipartFormDataContent();
			//    formdata.Add(new StringContent(p.Tittle), "Tittle");
			//    formdata.Add(new StringContent(p.Description), "Description");
			//    formdata.Add(new StringContent(p.Quantity.ToString()), "Quantity");
			//    formdata.Add(new StringContent(p.Price.ToString()), "Price");
			//    formdata.Add(new StringContent(p.CategoryId.ToString()), "CategoryId");
			//    formdata.Add(new StringContent(p.Isdelete.ToString()), "Isdelete");
			//    formdata.Add(new StreamContent(file.OpenReadStream()), "Picture", file.FileName);
			//    StringContent content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
			//    using (var response = await http.PostAsync("api/Product/UploadFile", formdata))
			//    {
			//        string apiReponse = await response.Content.ReadAsStringAsync();
			//        product = JsonConvert.DeserializeObject<ProductModel>(apiReponse);
			//    }
			//}
			client.BaseAddress = new Uri(domain);
			var formdata = new MultipartFormDataContent();
			formdata.Add(new StringContent(p.Id.ToString()), "Id");
			formdata.Add(new StringContent(p.Tittle), "Tittle");
			formdata.Add(new StringContent(p.Description), "Description");
			formdata.Add(new StringContent(p.Quantity.ToString()), "Quantity");
			formdata.Add(new StringContent(p.Price.ToString()), "Price");
			formdata.Add(new StringContent(p.CategoryId.ToString()), "CategoryId");
			formdata.Add(new StringContent(p.Isdelete.ToString()), "Isdelete");
			formdata.Add(new StreamContent(fileimage.OpenReadStream()), "Picture", fileimage.FileName);
			var response = client.PostAsync("api/Product/UploadFile", formdata);
			return RedirectToAction("Index");
		}

		[HttpGet]
		public async Task<IActionResult> Edit(int id)
		{

			client.BaseAddress = new Uri(domain);
			ProductModel product = new ProductModel();
			string data = await client.GetStringAsync("api/Product/" + id);
			product = JsonConvert.DeserializeObject<ProductModel>(data);
            return View(product);

        }
		[HttpGet]
		public async Task<IActionResult> Details(int id)
		{
            ViewBag.Domain = domain;
            client.BaseAddress = new Uri(domain);
			ProductModel product = new ProductModel();
			string data = await client.GetStringAsync("api/Product/" + id);
			product = JsonConvert.DeserializeObject<ProductModel>(data);
            ViewBag.Product = product;
            ViewBag.ProductTittle = product.Tittle;
            return View();

        }
		[HttpPut]
		[ValidateAntiForgeryToken]

		public async Task<IActionResult> Edit(ProductModel p, IFormFile file)
		{
			ProductModel product = new ProductModel();
			using (var http = new HttpClient())
			{
				var formdata = new MultipartFormDataContent();
				formdata.Add(new StringContent(p.Tittle), "Tittle");
				formdata.Add(new StringContent(p.Description), "Description");
				formdata.Add(new StringContent(p.Quantity.ToString()), "Quantity");
				formdata.Add(new StringContent(p.Price.ToString()), "Price");
				formdata.Add(new StringContent(p.CategoryId.ToString()), "CategoryId");
				formdata.Add(new StringContent(p.Isdelete.ToString()), "Isdelete");
				formdata.Add(new StreamContent(file.OpenReadStream()), "Picture", file.FileName);
				StringContent content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
				using (var response = await http.PutAsync("api/Product/UploadFile", formdata))
				{
					string apiReponse = await response.Content.ReadAsStringAsync();
					product = JsonConvert.DeserializeObject<ProductModel>(apiReponse);
				}
			}
			return RedirectToAction("Index");
		}
		[HttpDelete]
		public async Task<IActionResult> Delete(int ReservationId)
		{
			using (var httpClient = new HttpClient())
			{
				using (var response = await httpClient.DeleteAsync("https://localhost:7066/api/Product/" + ReservationId))
				{
					string apiResponse = await response.Content.ReadAsStringAsync();
				}
			}

			return RedirectToAction("Index");
		}



        
    }
}
