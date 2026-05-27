using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLNhanSu_CaLam_DatXe.Models
{
    [Table("TaiXe")]
    public class TaiXe
    {
        [Key]
        public int MaTaiXe { get; set; }

        public int? MaNhanVien { get; set; }

        [StringLength(20)]
        public string? BienSoXe { get; set; }

        [StringLength(100)]
        public string? LoaiXe { get; set; }

        [StringLength(50)]
        public string? BangLai { get; set; }

        [StringLength(50)]
        public string? TrangThai { get; set; } // "Sẵn sàng", "Đang bận", "Nghỉ"

        // Navigation properties
        [ForeignKey("MaNhanVien")]
        public virtual NhanVien? NhanVien { get; set; }

        public virtual ICollection<ChuyenXe> ChuyenXes { get; set; } = new List<ChuyenXe>();
    }
}
