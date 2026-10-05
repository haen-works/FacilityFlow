using FacilityFlow.Api.Data;
using FacilityFlow.Api.Dtos;
using FacilityFlow.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FacilityFlow.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/faultreports")]
    public class FaultReportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FaultReportsController(AppDbContext context)
        {
            _context = context;
        }

        // Tüm arızaları listeler ve isteğe göre filtreler.
        [HttpGet]
        public async Task<ActionResult> Get(
            [FromQuery] string? status,
            [FromQuery] string? priority,
            [FromQuery] int? reportedById,
            [FromQuery] int? assignedTechnicianId,
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = _context.FaultReports.AsNoTracking();

            var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if(!int.TryParse(userIdText, out var userId))
            {
                return Unauthorized();
            }

            if(role == "Employee")
            {
                query = query.Where(report => report.ReportedById == userId);
            }else if(role == "Technician")
            {
                query = query.Where(report => report.AssignedTechnicianId == userId);
            }else if(role !="Admin")
            {
                return Forbid();
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(report => report.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(priority))
            {
                query = query.Where(report => report.Priority == priority);
            }

            if (reportedById.HasValue)
            {
                query = query.Where(report => report.ReportedById == reportedById.Value);
            }

            if (assignedTechnicianId.HasValue)
            {
                query = query.Where(report => report.AssignedTechnicianId == assignedTechnicianId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(report =>
                    report.Title.Contains(search) ||
                    report.Description.Contains(search));
            }

            if (page < 1 || pageSize < 1 || pageSize > 100)
            {
                return BadRequest(new {
                    message = "Page en az 1 olmalı ve pageSize 1 ile 100 arasında olmalı."
                });
            }

            var totalCount = await query.CountAsync();

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            query = query.OrderByDescending(report => report.CreatedAt);

            var skipCount = (page - 1) * pageSize;

            var reports = await query

                    .Skip(skipCount)
                    .Take(pageSize)
                    .Select(report => new FaultReportResponseDto
                    {
                        Id = report.Id,
                        Title = report.Title,
                        Description = report.Description,
                        Location = report.Location,
                        Status = report.Status,
                        Priority = report.Priority,
                        AssignedTechnicianId = report.AssignedTechnicianId,
                        ReportedById = report.ReportedById,
                        CreatedAt = report.CreatedAt
                    })
                    .ToListAsync();

            return Ok(new
            {
                currentPage = page,
                pageSize = pageSize,
                totalCount = totalCount,
                totalPages = totalPages,
                data = reports
            });
        }

        // ID numarasına göre tek bir arıza getirir.
        [HttpGet("{id:int}")]
        public async Task<ActionResult<FaultReportResponseDto>> GetById(int id)
        {
            var report = await _context.FaultReports
                .AsNoTracking()
                .FirstOrDefaultAsync(report => report.Id == id);

            if (report == null)
            {
                return NotFound(new {
                message = "Arıza bulunamadı."
                });
            }

            var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if(!int.TryParse(userIdText, out var userId))
            {
                return Unauthorized();
            }

            if(role == "Employee" && report.ReportedById!= userId)
            {
                return Forbid();
            }

            if(role == "Technician" && report.AssignedTechnicianId != userId)
            {
                return Forbid();
            }

            if(role !="Employee"&&
               role !="Technician"&&
               role !="Admin"
            ){
                return Forbid();
            }

            return Ok(ToResponse(report));
        }

        // Bir arızanın yalnızca durumunu günceller.
        [HttpPatch("{id:int}/status")]
        [Authorize(Roles = "Technician,Admin")]
        public async Task<ActionResult<FaultReportResponseDto>> UpdateStatus(
            int id,
            UpdateFaultReportStatusDto dto)
        {
            var report = await _context.FaultReports.FindAsync(id);

            if (report == null)
            {
                return NotFound(new{
                message = "Arıza bulunamadı."
                });
            }

            var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var role = User.FindFirstValue(ClaimTypes.Role);

            if(!int.TryParse(userIdText, out var userId))
            {
                return  Unauthorized();
            }

            if(role == "Technician" && report.AssignedTechnicianId !=userId)
            {
                return Forbid();
            }

            report.Status = dto.Status;
            await _context.SaveChangesAsync();

            return Ok(ToResponse(report));
        }

        [HttpPatch("{id:int}/assignment")]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<FaultReportResponseDto>> AssignTechnician(
            int id,
            AssingTechnicianDto dto)
        {
            var report = await _context.FaultReports.FindAsync(id);

            if (report == null)
            {
                return NotFound(new { message = "Arıza bulunamadı ." });
            }

            var technician = await _context.Users.FindAsync(dto.TechnicianId);

            if (technician == null)
            {
                return NotFound(new { message = "Kullanıcı bulunamadı." });
            }

            if (technician.Role != "Technician")
            {
                return BadRequest(new
                {
                    message = "Arıza yalnızca teknik personele atanabilir."
                });
            }

            report.AssignedTechnicianId = technician.Id;
            await _context.SaveChangesAsync();

            return Ok(ToResponse(report));
        }

        [HttpPost]
        [Authorize(Roles ="Employee,Admin")]
        public async Task<ActionResult<FaultReportResponseDto>> Create(
            CreateFaultReportDto dto)
        {
            var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(!int.TryParse(userIdText, out var userId))
            {
                return Unauthorized();
            }

            var report = new FaultReport
            {
                Title = dto.Title.Trim(),
                Description = dto.Description.Trim(),
                Location = dto.Location.Trim(),
                Priority = dto.Priority,
                ReportedById = userId
            };

            _context.FaultReports.Add(report);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = report.Id },
                ToResponse(report));
        }

        [HttpPatch ("{id:int}/unassign")]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<FaultReportResponseDto>> UnassignTechnician(int id)
        {
            var report = await _context.FaultReports.FindAsync(id);

            if(report == null)
            {
                return NotFound(new
                {
                    message = "Arıza bulunamadı."
                });
            }

            report.AssignedTechnicianId = null;

            await _context.SaveChangesAsync();

            return Ok(ToResponse(report));
        }

        private static FaultReportResponseDto ToResponse(FaultReport report)
        {
            return new FaultReportResponseDto
            {
                Id = report.Id,
                Title = report.Title,
                Description = report.Description,
                Location = report.Location,
                Status = report.Status,
                Priority = report.Priority,
                AssignedTechnicianId = report.AssignedTechnicianId,
                ReportedById = report.ReportedById,
                CreatedAt = report.CreatedAt
            };
        }
    }
}