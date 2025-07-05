using Microsoft.AspNetCore.Mvc;
using ShampanDms.Service.Service;
using ShampanDms.Service.ViewModel;
using ShampanDms.Service.ViewModel.CommonVMs;
using System;
using System.Threading.Tasks;

namespace ShampanDmsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignDetailByInvoiceValueController : ControllerBase
    {
        // POST: api/CampaignDetailByInvoiceValue/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(CampaignDetailByInvoiceValueVM campaignDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            CampaignDetailByInvoiceValueService _campaignDetailService = new CampaignDetailByInvoiceValueService();

            try
            {
                resultVM = await _campaignDetailService.Insert(campaignDetail);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not created.",
                    ExMessage = ex.Message,
                    DataVM = campaignDetail
                };
            }
        }

        // POST: api/CampaignDetailByInvoiceValue/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(CampaignDetailByInvoiceValueVM campaignDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CampaignDetailByInvoiceValueService _campaignDetailService = new CampaignDetailByInvoiceValueService();
                resultVM = await _campaignDetailService.Update(campaignDetail);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not updated.",
                    ExMessage = ex.Message,
                    DataVM = campaignDetail
                };
            }
        }

        // POST: api/CampaignDetailByInvoiceValue/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(CampaignDetailByInvoiceValueVM campaignDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                CampaignDetailByInvoiceValueService _campaignDetailService = new CampaignDetailByInvoiceValueService();

                string?[] IDs = null;
                IDs = new string?[] { campaignDetail.Id.ToString() };

                resultVM = await _campaignDetailService.Delete(IDs);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not deleted.",
                    ExMessage = ex.Message,
                    DataVM = campaignDetail
                };
            }
        }

        // POST: api/CampaignDetailByInvoiceValue/List
        [HttpPost("List")]
        public async Task<ResultVM> List(CampaignDetailByInvoiceValueVM campaignDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CampaignDetailByInvoiceValueService _campaignDetailService = new CampaignDetailByInvoiceValueService();
                resultVM = await _campaignDetailService.List(new[] { "M.Id" }, new[] { campaignDetail.Id.ToString() }, null);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not fetched.",
                    ExMessage = ex.Message,
                    DataVM = null
                };
            }
        }

        // GET: api/CampaignDetailByInvoiceValue/ListAsDataTable
        [HttpGet("ListAsDataTable")]
        public async Task<ResultVM> ListAsDataTable(CampaignDetailByInvoiceValueVM campaignDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CampaignDetailByInvoiceValueService _campaignDetailService = new CampaignDetailByInvoiceValueService();
                resultVM = await _campaignDetailService.ListAsDataTable(new[] { "" }, new[] { "" });
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not fetched.",
                    ExMessage = ex.Message,
                    DataVM = null
                };
            }
        }

        // GET: api/CampaignDetailByInvoiceValue/Dropdown
        [HttpGet("Dropdown")]
        public async Task<ResultVM> Dropdown()
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CampaignDetailByInvoiceValueService _campaignDetailService = new CampaignDetailByInvoiceValueService();
                resultVM = await _campaignDetailService.Dropdown(); // Adjust if Dropdown requires a different method
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not fetched.",
                    ExMessage = ex.Message,
                    DataVM = null
                };
            }
        }
    }
}
