using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPBL3.DTO.Statistic;
using WebPBL3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static NuGet.Packaging.PackagingConstants;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using OfficeOpenXml;
using Microsoft.AspNetCore.Authorization;
using WebPBL3.Services;

namespace WebPBL3.Controllers
{
	[Authorize(Roles = "Admin, Staff")]
	public class StatisticController : Controller
    {
        private readonly IStatisticService _statisticService;

        public StatisticController(IStatisticService statisticService)
        {
            _statisticService = statisticService;
        }

        
        public async Task<IActionResult> Index(DateOnly? _startTime = null, DateOnly? _endTime = null, string? maNV = null, string? maXe = null, string? hangXe = null)
        {
            
            ViewBag._startTime = _startTime;
            ViewBag._endTime = _endTime;
            ViewBag.maNV = maNV;
            ViewBag.maXe = maXe;
            ViewBag.hangXe = hangXe;

            ViewBag.userTotal = await _statisticService.CountUsers();
            ViewBag.carTotal = await _statisticService.CountCars();
            ViewBag.staffTotal = await _statisticService.CountStaffs();
            ViewBag.feedbackTotal = await _statisticService.CountFeedBacks();

            
            IEnumerable<StatisticMake> statisticMakes = await _statisticService.GetStatisticMakes();
            

            ViewBag.statisticMakes = statisticMakes;

            IEnumerable<StatisticRevenue> statisticRevenues = await _statisticService.GetStatisticMonths();
            

            ViewBag.statisticRevenues = statisticRevenues;

            
            IEnumerable<StatisticTable> statisticTables = await _statisticService.GetStatisticTables(_startTime, _endTime, maNV, maXe, hangXe);
            
            ViewBag.statisticTables = statisticTables;

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> SaveExcel()
        {
            // Deserialize dữ liệu từ JSON sang list StatisticTable
            //Console.WriteLine(Request.Form["data"].ToString());
            var requestData = Request.Form["data"];
            if (string.IsNullOrEmpty(requestData))
            {
                return BadRequest("Data is null or empty");
            }
            List<StatisticTable> statisticTables;
            try
            {
                statisticTables = JsonConvert.DeserializeObject<List<StatisticTable>>(requestData, new JsonSerializerSettings { MaxDepth = 10 });
            }
            catch (JsonException ex)
            {
                
                return BadRequest("Failed to deserialize data: " +  ex.Message);
            }
            try
            {
                var memoryStream = await _statisticService.CreateAndSaveExcel(statisticTables);
                var fileName = "file.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File(memoryStream, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }
    }
}