using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLNhanSu_CaLam_DatXe.Models
{
    [Table("PhanCa")]
    public class PhanCa
    {
        [Key]
        public int MaPhanCa { get; set; }

        [Required]
        public int MaNhanVien { get; set; }

        [Required]
        public int MaCa { get; set; }

        [Required]
        public DateTime NgayLam { get; set; }

        [StringLength(255)]
        public string? GhiChu { get; set; }

        // Navigation properties
        [ForeignKey("MaNhanVien")]
        public virtual NhanVien NhanVien { get; set; } = null!;

        [ForeignKey("MaCa")]
        public virtual CaLam CaLam { get; set; } = null!;
    }
}
