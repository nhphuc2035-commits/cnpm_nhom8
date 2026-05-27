using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLNhanSu_CaLam_DatXe.Models
{
    [Table("CaLam")]
    public class CaLam
    {
        [Key]
        public int MaCa { get; set; }

        [Required]
        [StringLength(100)]
        public string TenCa { get; set; } = null!;

        [Required]
        public TimeSpan GioBatDau { get; set; }

        [Required]
        public TimeSpan GioKetThuc { get; set; }

        [StringLength(255)]
        public string? MoTa { get; set; }

        // Navigation properties
        public virtual ICollection<PhanCa> PhanCas { get; set; } = new List<PhanCa>();
    }
}
