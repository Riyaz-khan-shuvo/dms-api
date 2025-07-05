using Microsoft.AspNetCore.Mvc;
using ShampanDms.Service.Service;
using ShampanDms.Service.ViewModel;
using ShampanDms.Service.ViewModel.CommonVMs;
using ShampanDms.Service.ViewModel.KendoCommon;
using System;
using System.Threading.Tasks;

namespace ShampanDmsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignController : ControllerBase
    {
        CampaignService _campaignService = new CampaignService();

        // POST: api/Campaign/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(CampaignVM Campaign)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            _campaignService = new CampaignService();

            try
            {
                resultVM = await _campaignService.Insert(Campaign);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = Campaign
                };
            }
        }

        // POST: api/Campaign/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(CampaignVM Sale)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _campaignService = new CampaignService();
                resultVM = await _campaignService.Update(Sale);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = Sale
                };
            }
        }



        // POST: api/Campaign/List
        [HttpPost("List")]
        public async Task<ResultVM> List(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _campaignService = new CampaignService();
                resultVM = await _campaignService.List(new[] { "H.Id" }, new[] { vm.Id }, null);
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

        // GET: api/Campaign/ListAsDataTable
        [HttpGet("ListAsDataTable")]
        public async Task<ResultVM> ListAsDataTable(CampaignVM Sale)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _campaignService = new CampaignService();
                resultVM = await _campaignService.ListAsDataTable(new[] { "" }, new[] { "" });
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = Sale
                };
            }
        }

        //GET: api/Campaign/Dropdown
        [HttpGet("Dropdown")]
        public async Task<ResultVM> Dropdown()
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _campaignService = new CampaignService();
                resultVM = await _campaignService.Dropdown(); // Adjust if Dropdown requires a different method
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


        // POST: api/Campaign/MultiplePost
        [HttpPost("MultiplePost")]
        public async Task<ResultVM> MultiplePost(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                _campaignService = new CampaignService();

                resultVM = await _campaignService.MultiplePost(vm);
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


        // POST: api/Campaign/GetGridData
        [HttpPost("GetGridData")]
        public async Task<ResultVM> GetGridData(GridOptions options,string EnumId)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _campaignService = new CampaignService();

                resultVM = await _campaignService.GetGridData(options, EnumId, new[] { "H.BranchId", "H.IsPost", "H.CampaignStartDate between", "H.CampaignStartDate between" }, new[] { options.vm.BranchId.ToString(), options.vm.IsPost.ToString(), options.vm.FromDate.ToString(), options.vm.ToDate.ToString() });
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

        // POST: api/Campaign/GetGridData
        [HttpPost("GetDetailsGridData")]
        public async Task<ResultVM> GetDetailsGridData(GridOptions options, string EnumId)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _campaignService = new CampaignService();

                resultVM = await _campaignService.GetDetailsGridData(options, EnumId, new[] { "H.BranchId", "H.IsPost", "H.CampaignStartDate between", "H.CampaignStartDate between" }, new[] { options.vm.BranchId.ToString(), options.vm.IsPost.ToString(), options.vm.FromDate.ToString(), options.vm.ToDate.ToString() });
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
