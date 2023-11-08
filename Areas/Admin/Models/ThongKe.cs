
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebBanThu.Areas.Admin.Models
{
    public class ThongKe
    {
       
        public int SoLuongBil { get; set; }
        public double TongDoanhThu { get; set; }
        public int SoLuongTaiKhoan { get; set; }
        public int TongSanPhamDaBan { get; set; }
    }
}
