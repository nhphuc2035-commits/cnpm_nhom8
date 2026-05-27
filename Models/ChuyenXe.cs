using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLNhanSu_CaLam_DatXe.Models
{
    [Table("ChuyenXe")]
    public class ChuyenXe
    {
        [Key]
        public int MaChuyenXe { get; set; }

        [Required]
        public int MaKhachHang { get; set; }

        public int? MaTaiXe { get; set; }

        [Required]
        [StringLength(255)]
        public string DiemDon { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string DiemDen { get; set; } = null!;

        public DateTime? ThoiGianDat { get; set; }

        [StringLength(50)]
        public string? TrangThai { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? GiaTien { get; set; }

        // Navigation properties
        [ForeignKey("MaKhachHang")]
        public virtual KhachHang KhachHang { get; set; } = null!;

        [ForeignKey("MaTaiXe")]
        public virtual TaiXe? TaiXe { get; set; }
    }
}
