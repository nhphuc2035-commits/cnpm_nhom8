using Microsoft.EntityFrameworkCore;

namespace QLNhanSu_CaLam_DatXe.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ChucVu> ChucVus { get; set; } = null!;
        public DbSet<CaLam> CaLams { get; set; } = null!;
        public DbSet<NhanVien> NhanViens { get; set; } = null!;
        public DbSet<TaiXe> TaiXes { get; set; } = null!;
        public DbSet<PhanCa> PhanCas { get; set; } = null!;
        public DbSet<KhachHang> KhachHangs { get; set; } = null!;
        public DbSet<ChuyenXe> ChuyenXes { get; set; } = null!;
        public DbSet<ChamCong> ChamCongs { get; set; } = null!;
        public DbSet<Luong> Luongs { get; set; } = null!;
        public DbSet<TaiKhoan> TaiKhoans { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure decimal precision
            modelBuilder.Entity<NhanVien>()
                .Property(n => n.LuongCoBan)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ChuyenXe>()
                .Property(c => c.GiaTien)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Luong>()
                .Property(l => l.Thuong)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Luong>()
                .Property(l => l.Phat)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Luong>()
                .Property(l => l.TongLuong)
                .HasPrecision(18, 2);
        }
    }
}
