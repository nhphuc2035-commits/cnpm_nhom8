using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLNhanSu_CaLam_DatXe.Models
{
    [Table("Luong")]
    public class Luong
    {
        [Key]
        public int MaLuong { get; set; }

        [Required]
        public int MaNhanVien { get; set; }

        [Required]
        public int Thang { get; set; }

        [Required]
        public int Nam { get; set; }

        public int? TongCaLam { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Thuong { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Phat { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TongLuong { get; set; }

        // Navigation properties
        [ForeignKey("MaNhanVien")]
        public virtual NhanVien NhanVien { get; set; } = null!;
    }
}
