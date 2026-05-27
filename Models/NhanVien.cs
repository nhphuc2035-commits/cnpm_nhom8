using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLNhanSu_CaLam_DatXe.Models
{
    [Table("NhanVien")]
    public class NhanVien
    {
        [Key]
        public int MaNhanVien { get; set; }

        [Required]
        [StringLength(100)]
        public string HoTen { get; set; } = null!;

        [StringLength(10)]
        public string? GioiTinh { get; set; }

        public DateTime? NgaySinh { get; set; }

        [StringLength(15)]
        public string? SoDienThoai { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(255)]
        public string? DiaChi { get; set; }

        public DateTime? NgayVaoLam { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? LuongCoBan { get; set; }

        [StringLength(50)]
        public string? TrangThai { get; set; } // "Đang làm việc", "Nghỉ phép", "Vắng mặt", "Ngừng hoạt động"

        public int? MaChucVu { get; set; }

        // Navigation properties
        [ForeignKey("MaChucVu")]
        public virtual ChucVu? ChucVu { get; set; }

        public virtual ICollection<TaiXe> TaiXes { get; set; } = new List<TaiXe>();
        public virtual ICollection<PhanCa> PhanCas { get; set; } = new List<PhanCa>();
        public virtual ICollection<ChamCong> ChamCongs { get; set; } = new List<ChamCong>();
        public virtual ICollection<Luong> Luongs { get; set; } = new List<Luong>();
        public virtual ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();
    }
}
