using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShampanDms.Service.Service;
using ShampanDms.Service.ViewModel;
using ShampanDms.Service.ViewModel.CommonVMs;

namespace ShampanDmsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MISReportController : ControllerBase
    {
        // POST: api/MISReport/ExportSaleAndPayment
        [HttpPost("ExportSaleAndPayment")]
        public async Task<ResultVM> ExportSaleAndPayment(SalePaymentVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                MISReportService _service = new MISReportService();
                resultVM = await _service.ExportSaleAndPayment(vm);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = vm
                };
            }
        }


        [HttpPost("ExportSaleAndPaymentSummary")]
        public async Task<ResultVM> ExportSaleAndPaymentSummary(SalePaymentVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                MISReportService _service = new MISReportService();
                resultVM = await _service.ExportSaleAndPaymentSummary(vm);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = vm
                };
            }
        }




    }
}
