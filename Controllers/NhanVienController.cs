using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNhanSu_CaLam_DatXe.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QLNhanSu_CaLam_DatXe.Controllers
{
    public class NhanVienController : Controller
    {
        private readonly AppDbContext _context;

        public NhanVienController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /NhanVien/GetAll
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var nvList = await _context.NhanViens
                .Include(nv => nv.ChucVu)
                .Include(nv => nv.TaiXes)
                .OrderBy(nv => nv.MaNhanVien)
                .Select(nv => new
                {
                    nv.MaNhanVien,
                    nv.HoTen,
                    nv.GioiTinh,
                    NgaySinh = nv.NgaySinh.HasValue ? nv.NgaySinh.Value.ToString("yyyy-MM-dd") : null,
                    nv.SoDienThoai,
                    nv.Email,
                    nv.DiaChi,
                    NgayVaoLam = nv.NgayVaoLam.HasValue ? nv.NgayVaoLam.Value.ToString("yyyy-MM-dd") : null,
                    nv.LuongCoBan,
                    nv.TrangThai,
                    nv.MaChucVu,
                    TenChucVu = nv.ChucVu != null ? nv.ChucVu.TenChucVu : "Chưa phân công",
                    DriverProfile = nv.TaiXes.Select(t => new
                    {
                        t.MaTaiXe,
                        t.BienSoXe,
                        t.LoaiXe,
                        t.BangLai,
                        t.TrangThai
                    }).FirstOrDefault()
                })
                .ToListAsync();

            return Json(nvList);
        }

        // GET: /NhanVien/GetRoles
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _context.ChucVus
                .Select(c => new { c.MaChucVu, c.TenChucVu, c.MoTa })
                .ToListAsync();
            return Json(roles);
        }

        // POST: /NhanVien/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NhanVienInputModel input)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var nv = new NhanVien
                {
                    HoTen = input.HoTen,
                    GioiTinh = input.GioiTinh,
                    NgaySinh = input.NgaySinh,
                    SoDienThoai = input.SoDienThoai,
                    Email = input.Email,
                    DiaChi = input.DiaChi,
                    NgayVaoLam = input.NgayVaoLam ?? DateTime.Today,
                    LuongCoBan = input.LuongCoBan,
                    TrangThai = input.TrangThai ?? "Đang làm việc",
                    MaChucVu = input.MaChucVu
                };

                _context.NhanViens.Add(nv);
                await _context.SaveChangesAsync();

                // If role is Tai Xe (MaChucVu = 3), create TaiXe record
                if (input.MaChucVu == 3)
                {
                    var tx = new TaiXe
                    {
                        MaNhanVien = nv.MaNhanVien,
                        BienSoXe = input.BienSoXe,
                        LoaiXe = input.LoaiXe,
                        BangLai = input.BangLai,
                        TrangThai = "Sẵn sàng"
                    };
                    _context.TaiXes.Add(tx);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return Json(new { success = true, message = "Thêm nhân sự mới thành công!" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        // POST: /NhanVien/Edit
        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] NhanVienInputModel input)
        {
            if (!ModelState.IsValid || input.MaNhanVien <= 0)
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var nv = await _context.NhanViens.FindAsync(input.MaNhanVien);
                if (nv == null)
                {
                    return Json(new { success = false, message = "Nhân viên không tồn tại!" });
                }

                nv.HoTen = input.HoTen;
                nv.GioiTinh = input.GioiTinh;
                nv.NgaySinh = input.NgaySinh;
                nv.SoDienThoai = input.SoDienThoai;
                nv.Email = input.Email;
                nv.DiaChi = input.DiaChi;
                nv.NgayVaoLam = input.NgayVaoLam;
                nv.LuongCoBan = input.LuongCoBan;
                nv.TrangThai = input.TrangThai;
                
                int? oldChucVu = nv.MaChucVu;
                nv.MaChucVu = input.MaChucVu;

                _context.NhanViens.Update(nv);
                await _context.SaveChangesAsync();

                // Manage TaiXe record
                if (input.MaChucVu == 3)
                {
                    // Check if TaiXe already exists
                    var tx = await _context.TaiXes.FirstOrDefaultAsync(t => t.MaNhanVien == nv.MaNhanVien);
                    if (tx == null)
                    {
                        tx = new TaiXe
                        {
                            MaNhanVien = nv.MaNhanVien,
                            BienSoXe = input.BienSoXe,
                            LoaiXe = input.LoaiXe,
                            BangLai = input.BangLai,
                            TrangThai = "Sẵn sàng"
                        };
                        _context.TaiXes.Add(tx);
                    }
                    else
                    {
                        tx.BienSoXe = input.BienSoXe;
                        tx.LoaiXe = input.LoaiXe;
                        tx.BangLai = input.BangLai;
                        _context.TaiXes.Update(tx);
                    }
                    await _context.SaveChangesAsync();
                }
                else if (oldChucVu == 3 && input.MaChucVu != 3)
                {
                    // If changed from Tai Xe to something else, remove driver profile or handle dependencies
                    var tx = await _context.TaiXes.Include(t => t.ChuyenXes).FirstOrDefaultAsync(t => t.MaNhanVien == nv.MaNhanVien);
                    if (tx != null)
                    {
                        // Clean up referencing ChuyenXe (set MaTaiXe = null)
                        foreach (var cx in tx.ChuyenXes)
                        {
                            cx.MaTaiXe = null;
                        }
                        _context.TaiXes.Remove(tx);
                        await _context.SaveChangesAsync();
                    }
                }

                await transaction.CommitAsync();
                return Json(new { success = true, message = "Cập nhật nhân sự thành công!" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        // POST: /NhanVien/Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return Json(new { success = false, message = "ID không hợp lệ!" });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var nv = await _context.NhanViens
                    .Include(n => n.TaiXes).ThenInclude(tx => tx.ChuyenXes)
                    .Include(n => n.PhanCas)
                    .Include(n => n.ChamCongs)
                    .Include(n => n.Luongs)
                    .Include(n => n.TaiKhoans)
                    .FirstOrDefaultAsync(n => n.MaNhanVien == id);

                if (nv == null)
                {
                    return Json(new { success = false, message = "Nhân viên không tồn tại!" });
                }

                // Delete associated Driver and clear references in ChuyenXe
                foreach (var tx in nv.TaiXes)
                {
                    foreach (var cx in tx.ChuyenXes)
                    {
                        cx.MaTaiXe = null;
                    }
                    _context.TaiXes.Remove(tx);
                }

                // Delete dependencies
                _context.PhanCas.RemoveRange(nv.PhanCas);
                _context.ChamCongs.RemoveRange(nv.ChamCongs);
                _context.Luongs.RemoveRange(nv.Luongs);
                _context.TaiKhoans.RemoveRange(nv.TaiKhoans);

                // Delete the employee
                _context.NhanViens.Remove(nv);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new { success = true, message = "Xóa nhân sự thành công!" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = "Lỗi khi xóa nhân viên: " + ex.Message });
            }
        }
    }

    public class NhanVienInputModel
    {
        public int MaNhanVien { get; set; }
        public string HoTen { get; set; } = null!;
        public string? GioiTinh { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string? SoDienThoai { get; set; }
        public string? Email { get; set; }
        public string? DiaChi { get; set; }
        public DateTime? NgayVaoLam { get; set; }
        public decimal? LuongCoBan { get; set; }
        public string? TrangThai { get; set; }
        public int MaChucVu { get; set; }

        // Driver details
        public string? BienSoXe { get; set; }
        public string? LoaiXe { get; set; }
        public string? BangLai { get; set; }
    }
}
