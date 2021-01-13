using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EduLocate.Core;
using EduLocate.Server.Models;
using EduLocate.Services.ServiceInterfaces.School;

namespace EduLocate.Server.Controllers
{
    [Authorize(Policy = Policies.DataManagerPolicyName)]
    public class SchoolAdminController : Controller
    {
        private const int PageSize = 50;

        private readonly ISchoolRepository _schoolRepository;

        public SchoolAdminController(ISchoolRepository schoolRepository)
        {
            _schoolRepository = schoolRepository;
        }

        public async Task<IActionResult> Create(School school)
        {
            if (!ModelState.IsValid)
                return View(school);
            await _schoolRepository.SelectiveUpdateSchoolAsync(school);
            return RedirectToAction("Edit", new {id = school.Id});
        }

        public async Task<IActionResult> Edit(int id)
        {
            School school = await _schoolRepository.GetSchoolAsync(id);
            if (school == null)
                return NotFound();
            return View(school);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(School school)
        {
            if (!ModelState.IsValid)
                return View(school);
            await _schoolRepository.SelectiveUpdateSchoolAsync(school);
            return RedirectToAction("Edit", new {id = school.Id});
        }

        public async Task<IActionResult> Index(int page = 0, string filter = "")
        {
            ViewData["page"] = page;
            ViewData["filter"] = filter;

            IEnumerable<School> schools = await _schoolRepository.GetAllSchoolsAsync();
            if (!string.IsNullOrWhiteSpace(filter))
                schools = schools.Where(s =>
                    s.Id.ToString().Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                    (s.Name ?? "").Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                    (s.Address ?? "").Contains(filter, StringComparison.OrdinalIgnoreCase));
            return View(schools.Skip(page * PageSize).Take(PageSize));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}