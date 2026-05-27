using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLNhanSu_CaLam_DatXe.Models
{
    [Table("TaiKhoan")]
    public class TaiKhoan
    {
        [Key]
        public int MaTaiKhoan { get; set; }

        [Required]
        [StringLength(50)]
        public string TenDangNhap { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string MatKhau { get; set; } = null!;

        [StringLength(50)]
        public string? VaiTro { get; set; } // "Admin", "Coordinator", "CustomerCare", "Driver"

        public int? MaNhanVien { get; set; }

        // Navigation properties
        [ForeignKey("MaNhanVien")]
        public virtual NhanVien? NhanVien { get; set; }
    }
}
