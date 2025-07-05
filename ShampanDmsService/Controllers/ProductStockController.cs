using Microsoft.AspNetCore.Mvc;
using ShampanDms.Service.Service;
using ShampanDms.Service.ViewModel;
using ShampanDms.Service.ViewModel.CommonVMs;
using ShampanDms.Service.ViewModel.KendoCommon;

namespace ShampanDmsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductStockController : ControllerBase
    {
        ProductStockService _productStockService = new ProductStockService();
        // POST: api/ProductPriceGroup/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(List<ProductStockVM> productStockVM)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            _productStockService = new ProductStockService();

            try
            {
                resultVM = await _productStockService.Insert(productStockVM);
                return resultVM;
            }

            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = productStockVM
                };
            }

        }



        // POST: api/ProductStock/GetGridData
        [HttpPost("GetGridData")]
        public async Task<ResultVM> GetGridData(GridOptions options, string ProductId)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _productStockService = new ProductStockService();
                resultVM = await _productStockService.GetGridData(options, ProductId);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = null
                };
            }
        }

    }
}
