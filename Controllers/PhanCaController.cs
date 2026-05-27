using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNhanSu_CaLam_DatXe.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QLNhanSu_CaLam_DatXe.Controllers
{
    public class PhanCaController : Controller
    {
        private readonly AppDbContext _context;

        public PhanCaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /PhanCa/GetShiftsForDate
        [HttpGet]
        public async Task<IActionResult> GetShiftsForDate(string date)
        {
            if (!DateTime.TryParse(date, out DateTime queryDate))
            {
                queryDate = DateTime.Today;
            }

            var shifts = await _context.CaLams.ToListAsync();
            var assignments = await _context.PhanCas
                .Include(p => p.NhanVien)
                .ThenInclude(n => n.ChucVu)
                .Where(p => p.NgayLam.Date == queryDate.Date)
                .ToListAsync();

            var result = shifts.Select(s => {
                var shiftAssignments = assignments.Where(a => a.MaCa == s.MaCa).ToList();
                return new
                {
                    s.MaCa,
                    s.TenCa,
                    GioBatDau = s.GioBatDau.ToString(@"hh\:mm"),
                    GioKetThuc = s.GioKetThuc.ToString(@"hh\:mm"),
                    s.MoTa,
                    AssignedPersonnel = shiftAssignments.Select(a => new
                    {
                        a.MaPhanCa,
                        a.MaNhanVien,
                        a.NhanVien.HoTen,
                        a.NhanVien.SoDienThoai,
                        a.NhanVien.TrangThai,
                        TenChucVu = a.NhanVien.ChucVu != null ? a.NhanVien.ChucVu.TenChucVu : "N/A",
                        a.GhiChu
                    }).ToList(),
                    PersonnelCount = shiftAssignments.Count
                };
            }).ToList();

            return Json(result);
        }

        // GET: /PhanCa/GetAllShifts
        [HttpGet]
        public async Task<IActionResult> GetAllShifts()
        {
            var shifts = await _context.CaLams
                .OrderBy(s => s.GioBatDau)
                .Select(s => new
                {
                    s.MaCa,
                    s.TenCa,
                    GioBatDau = s.GioBatDau.ToString(@"hh\:mm"),
                    GioKetThuc = s.GioKetThuc.ToString(@"hh\:mm"),
                    s.MoTa
                })
                .ToListAsync();
            return Json(shifts);
        }

        // GET: /PhanCa/GetAvailablePersonnelForDate
        [HttpGet]
        public async Task<IActionResult> GetAvailablePersonnelForDate(string date, int shiftId)
        {
            if (!DateTime.TryParse(date, out DateTime queryDate))
            {
                queryDate = DateTime.Today;
            }

            // Get IDs of personnel already assigned on this day for this shift
            var assignedIds = await _context.PhanCas
                .Where(p => p.NgayLam.Date == queryDate.Date && p.MaCa == shiftId)
                .Select(p => p.MaNhanVien)
                .ToListAsync();

            // Find all active employees who are NOT assigned to this shift yet
            var available = await _context.NhanViens
                .Include(nv => nv.ChucVu)
                .Where(nv => nv.TrangThai == "Đang làm việc" && !assignedIds.Contains(nv.MaNhanVien))
                .Select(nv => new
                {
                    nv.MaNhanVien,
                    nv.HoTen,
                    TenChucVu = nv.ChucVu != null ? nv.ChucVu.TenChucVu : "N/A",
                    nv.SoDienThoai
                })
                .ToListAsync();

            return Json(available);
        }

        // POST: /PhanCa/Assign
        [HttpPost]
        public async Task<IActionResult> Assign([FromBody] AssignInputModel input)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
            }

            try
            {
                // Check if already assigned
                var exists = await _context.PhanCas
                    .AnyAsync(p => p.NgayLam.Date == input.NgayLam.Date && p.MaNhanVien == input.MaNhanVien && p.MaCa == input.MaCa);
                
                if (exists)
                {
                    return Json(new { success = false, message = "Nhân viên đã được phân vào ca này trước đó!" });
                }

                var phanCa = new PhanCa
                {
                    MaNhanVien = input.MaNhanVien,
                    MaCa = input.MaCa,
                    NgayLam = input.NgayLam.Date,
                    GhiChu = input.GhiChu
                };

                _context.PhanCas.Add(phanCa);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Phân công ca làm thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        // POST: /PhanCa/RemoveAssignment
        [HttpPost]
        public async Task<IActionResult> RemoveAssignment(int id)
        {
            if (id <= 0)
            {
                return Json(new { success = false, message = "ID không hợp lệ!" });
            }

            try
            {
                var assignment = await _context.PhanCas.FindAsync(id);
                if (assignment == null)
                {
                    return Json(new { success = false, message = "Bản phân ca không tồn tại!" });
                }

                _context.PhanCas.Remove(assignment);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Hủy phân công ca thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        // POST: /PhanCa/CreateShift
        [HttpPost]
        public async Task<IActionResult> CreateShift([FromBody] ShiftInputModel input)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
            }

            try
            {
                if (!TimeSpan.TryParse(input.GioBatDau, out TimeSpan start) || !TimeSpan.TryParse(input.GioKetThuc, out TimeSpan end))
                {
                    return Json(new { success = false, message = "Định dạng thời gian không đúng!" });
                }

                var cl = new CaLam
                {
                    TenCa = input.TenCa,
                    GioBatDau = start,
                    GioKetThuc = end,
                    MoTa = input.MoTa
                };

                _context.CaLams.Add(cl);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Tạo ca làm mới thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }

    public class AssignInputModel
    {
        public int MaNhanVien { get; set; }
        public int MaCa { get; set; }
        public DateTime NgayLam { get; set; }
        public string? GhiChu { get; set; }
    }

    public class ShiftInputModel
    {
        public string TenCa { get; set; } = null!;
        public string GioBatDau { get; set; } = null!;
        public string GioKetThuc { get; set; } = null!;
        public string? MoTa { get; set; }
    }
}
