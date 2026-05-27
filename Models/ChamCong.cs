using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLNhanSu_CaLam_DatXe.Models
{
    [Table("ChamCong")]
    public class ChamCong
    {
        [Key]
        public int MaChamCong { get; set; }

        [Required]
        public int MaNhanVien { get; set; }

        [Required]
        public DateTime NgayChamCong { get; set; }

        public TimeSpan? GioCheckIn { get; set; }

        public TimeSpan? GioCheckOut { get; set; }

        [StringLength(50)]
        public string? TrangThai { get; set; }

        // Navigation properties
        [ForeignKey("MaNhanVien")]
        public virtual NhanVien NhanVien { get; set; } = null!;
    }
}
