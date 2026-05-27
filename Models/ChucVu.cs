using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLNhanSu_CaLam_DatXe.Models
{
    [Table("ChucVu")]
    public class ChucVu
    {
        [Key]
        public int MaChucVu { get; set; }

        [Required]
        [StringLength(100)]
        public string TenChucVu { get; set; } = null!;

        [StringLength(255)]
        public string? MoTa { get; set; }

        // Navigation properties
        public virtual ICollection<NhanVien> NhanViens { get; set; } = new List<NhanVien>();
    }
}
