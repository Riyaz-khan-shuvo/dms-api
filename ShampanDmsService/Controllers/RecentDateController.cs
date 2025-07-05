using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ShampanDms.Service.Service;
using ShampanDms.Service.ViewModel;
using ShampanDms.Service.ViewModel.CommonVMs;
using ShampanDms.Service.ViewModel.KendoCommon;
using ShampanDms.Service.ViewModel.Utility;
using ShampanDmsService.Configuration;
using System;
using System.Data;
using System.Threading.Tasks;
using static ShampanDmsService.Configuration.HttpRequestHelper;

namespace ShampanDmsService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecentDateController : ControllerBase
    {
        private readonly IRecentDateService _recentDateService;

        public RecentDateController(IRecentDateService recentDateService)
        {
            _recentDateService = recentDateService;
        }

        [HttpGet]
        public ActionResult<RecentDateVM> GetRecentDate()
        {
            try
            {
                var recentDate = _recentDateService.GetRecentDate();
                return Ok(recentDate);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
    }
}
