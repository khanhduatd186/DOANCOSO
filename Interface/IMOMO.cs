using Microsoft.AspNetCore.Mvc;
using WebBanThu.Models;

namespace WebBanThu.Interface
{
    public interface IMOMO
    {
        Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderInfoModel model,  double price, string name);
        MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
    }
}

