using ShampanDms.Service.Repository;
using ShampanDms.Service.ViewModel.CommonVMs;
using ShampanDms.Service.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using ShampanDms.Service.ViewModel.Utility;
using ShampanDms.Service.ViewModel.KendoCommon;
using Newtonsoft.Json;

namespace ShampanDms.Service.Service
{
    public class CampaignService
    {
        CommonRepository _commonRepo = new CommonRepository();

        public async Task<ResultVM> Insert(CampaignVM campaign)
        {
            string CodeGroup = "Campaign";
            string CodeName = campaign.EnumName.ToString();
            CampaignRepository _repo = new CampaignRepository();
            _commonRepo = new CommonRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            bool isNewConnection = false;
            bool CampaignCheckExists = false;
            

    SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();

                string code = _commonRepo.GenerateCode(CodeGroup, CodeName, campaign.CreatedOn, campaign.BranchId, conn, transaction);

                if (!string.IsNullOrEmpty(code))
                {
                    (bool isExists, int campaignId) = await _repo.CampaignCheckExists(campaign, conn, transaction);
                    if (isExists)
                    {
                        throw new Exception($"A campaign was already entered today for Campaign Entry Date {campaign.CampaignEntryDate}.");
                    }
                    campaign.Code = code;
                    result = await _repo.Insert(campaign, conn, transaction);

                    if (result.Status.ToLower() == "success")
                    {
                        int LineNo = 1;
                        if (campaign.EnumName == "CampaignByProductQuantity")
                        {


                            foreach (var details in campaign.campaignDetailByQuantity)
                            {
                                
                                details.CampaignId = campaign.Id;
                                details.CampaignStartDate = campaign.CampaignStartDate;
                                details.CampaignEndDate = campaign.CampaignEndDate;
                                details.CampaignEntryDate = campaign.CampaignEntryDate;
                                details.BranchId = campaign.BranchId;

                               

                                var resultDetail = await _repo.InsertCampaignByQuantityDetails(details, conn, transaction);


                                if (resultDetail.Status.ToLower() == "fail")
                                {
                                    throw new Exception(resultDetail.Message);
                                }

                                LineNo++;
                            }
                        }
                        else if (campaign.EnumName == "CampaignByProductValue")
                        {
                            foreach (var details in campaign.campaignDetailByProductValue)
                            {
                               
                                details.CampaignId = campaign.Id;
                                details.CampaignStartDate = campaign.CampaignStartDate;
                                details.CampaignEndDate = campaign.CampaignEndDate;
                                details.CampaignEntryDate = campaign.CampaignEntryDate;
                                details.BranchId = campaign.BranchId;



                                var resultDetail = await _repo.InsertCampaignByProductValueDetails(details, conn, transaction);


                                if (resultDetail.Status.ToLower() == "fail")
                                {
                                    throw new Exception(resultDetail.Message);
                                }

                                LineNo++;
                            }
                        }

                        else if (campaign.EnumName == "CampaignByProductTotalValue")
                        {
                            foreach (var details in campaign.campaignDetailByProductTotalValue)
                            {
                                
                                details.CampaignId = campaign.Id;
                                details.CampaignStartDate = campaign.CampaignStartDate;
                                details.CampaignEndDate = campaign.CampaignEndDate;
                                details.CampaignEntryDate = campaign.CampaignEntryDate;
                                details.BranchId = campaign.BranchId;



                                var resultDetail = await _repo.InsertCampaignByProductTotalValueDetails(details, conn, transaction);


                                if (resultDetail.Status.ToLower() == "fail")
                                {
                                    throw new Exception(resultDetail.Message);
                                }

                                LineNo++;
                            }
                        }
                        else if (campaign.EnumName == "CampaignByInvoiceValue")
                        {
                            foreach (var details in campaign.campaignDetailByInvoiceValue)
                            {
                                

                                details.CampaignId = campaign.Id;
                                details.CampaignStartDate = campaign.CampaignStartDate;
                                details.CampaignEndDate = campaign.CampaignEndDate;
                                details.CampaignEntryDate = campaign.CampaignEntryDate;
                                details.BranchId = campaign.BranchId;



                                var resultDetail = await _repo.InsertCampaignDetailByInvoiceValueDetails(details, conn, transaction);

                                if (resultDetail.Status.ToLower() == "fail")
                                {
                                    throw new Exception(resultDetail.Message);
                                }
                               

                                LineNo++;
                            }
                        }
                    }
                    if (isNewConnection && result.Status == "Success")
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        throw new Exception(result.Message);
                    }

                    return result;
                }
                else
                {
                    throw new Exception("Code Generation Failed!");
                }
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }
                result.Status = "Fail";
                result.Message = ex.Message.ToString();
                result.ExMessage = ex.ToString();
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }


        public async Task<ResultVM> Update(CampaignVM campaign)
        {
            CampaignRepository _repo = new CampaignRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            _commonRepo = new CommonRepository();
            bool isNewConnection = false;
            bool CampaignCheckExists = false;

            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();
                if (campaign.EnumName == "CampaignByProductQuantity")
                {
                    result = _commonRepo.DetailsDelete("CampaignDetailByQuantities", new[] { "CampaignId" }, new[] { campaign.Id.ToString() }, conn, transaction);
                }
                else if (campaign.EnumName == "CampaignByProductValue")
                {
                    result = _commonRepo.DetailsDelete("CampaignDetailByProductValues", new[] { "CampaignId" }, new[] { campaign.Id.ToString() }, conn, transaction);
                }
                else if (campaign.EnumName == "CampaignByProductTotalValue")
                {
                    result = _commonRepo.DetailsDelete("CampaignDetailByProductTotalValues", new[] { "CampaignId" }, new[] { campaign.Id.ToString() }, conn, transaction);

                }
                else if (campaign.EnumName == "CampaignByInvoiceValue")
                {
                    result = _commonRepo.DetailsDelete("CampaignDetailByInvoiceValues", new[] { "CampaignId" }, new[] { campaign.Id.ToString() }, conn, transaction);

                }

                if (result.Status == "Fail")
                {
                    throw new Exception("Error in Delete for Details Data.");
                }

                result = await _repo.Update(campaign, conn, transaction);

                if (result.Status.ToLower() == "success")
                {
                    int LineNo = 1;
                    if (campaign.EnumName == "CampaignByProductQuantity")
                    {


                        foreach (var details in campaign.campaignDetailByQuantity)
                        {
                            details.CampaignId = campaign.Id;
                            details.CampaignStartDate = campaign.CampaignStartDate;
                            details.CampaignEndDate = campaign.CampaignEndDate;
                            details.CampaignEntryDate = campaign.CampaignEntryDate;
                            details.BranchId = campaign.BranchId;



                            var resultDetail = await _repo.InsertCampaignByQuantityDetails(details, conn, transaction);


                            if (resultDetail.Status.ToLower() == "fail")
                            {
                                throw new Exception(resultDetail.Message);
                            }

                            LineNo++;
                        }
                    }
                    else if (campaign.EnumName == "CampaignByProductValue")
                    {
                        foreach (var details in campaign.campaignDetailByProductValue)
                        {
                            details.CampaignId = campaign.Id;
                            details.CampaignStartDate = campaign.CampaignStartDate;
                            details.CampaignEndDate = campaign.CampaignEndDate;
                            details.CampaignEntryDate = campaign.CampaignEntryDate;
                            details.BranchId = campaign.BranchId;



                            var resultDetail = await _repo.InsertCampaignByProductValueDetails(details, conn, transaction);


                            if (resultDetail.Status.ToLower() == "fail")
                            {
                                throw new Exception(resultDetail.Message);
                            }

                            LineNo++;
                        }
                    }

                    else if (campaign.EnumName == "CampaignByProductTotalValue")
                    {
                        foreach (var details in campaign.campaignDetailByProductTotalValue)
                        {
                            details.CampaignId = campaign.Id;
                            details.CampaignStartDate = campaign.CampaignStartDate;
                            details.CampaignEndDate = campaign.CampaignEndDate;
                            details.CampaignEntryDate = campaign.CampaignEntryDate;
                            details.BranchId = campaign.BranchId;



                            var resultDetail = await _repo.InsertCampaignByProductTotalValueDetails(details, conn, transaction);


                            if (resultDetail.Status.ToLower() == "fail")
                            {
                                throw new Exception(resultDetail.Message);
                            }

                            LineNo++;
                        }
                    }
                    else if (campaign.EnumName == "CampaignByInvoiceValue")
                    {
                        foreach (var details in campaign.campaignDetailByInvoiceValue)
                        {
                            details.CampaignId = campaign.Id;
                            details.CampaignStartDate = campaign.CampaignStartDate;
                            details.CampaignEndDate = campaign.CampaignEndDate;
                            details.CampaignEntryDate = campaign.CampaignEntryDate;
                            details.BranchId = campaign.BranchId;



                            var resultDetail = await _repo.InsertCampaignDetailByInvoiceValueDetails(details, conn, transaction);

                            if (resultDetail.Status.ToLower() == "fail")
                            {
                                throw new Exception(resultDetail.Message);
                            }

                            LineNo++;
                        }
                    }
                }

                if (isNewConnection && result.Status == "Success")
                {
                    transaction.Commit();
                }
                else
                {
                    throw new Exception(result.Message);
                }

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }
                result.Message = ex.Message.ToString();
                result.ExMessage = ex.ToString();
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }



        public async Task<ResultVM> List(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            CampaignRepository _repo = new CampaignRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();

                result = await _repo.List(conditionalFields, conditionalValues, vm, conn, transaction);

                var lst = new List<CampaignVM>();

                string data = JsonConvert.SerializeObject(result.DataVM);
                lst = JsonConvert.DeserializeObject<List<CampaignVM>>(data);

                if (lst.FirstOrDefault().EnumName == "CampaignByProductQuantity")
                {
                    var CampaignByQuantity = await _repo.CampaignByQuantityDetailsList(new[] { "D.CampaignId" }, conditionalValues, vm, conn, transaction);

                    if (CampaignByQuantity.Status == "Success" && CampaignByQuantity.DataVM is DataTable dt)
                    {
                        string json = JsonConvert.SerializeObject(dt);
                        var CampaignByQuantitydetails = JsonConvert.DeserializeObject<List<CampaignDetailByQuantityVM>>(json);

                        lst.FirstOrDefault().campaignDetailByQuantity = CampaignByQuantitydetails;
                        result.DataVM = lst;

                    }
                }
                else if (lst.FirstOrDefault().EnumName == "CampaignByProductValue")
                {
                    var CampaignByProductValue = await _repo.CampaignProductValueDetailsList(new[] { "D.CampaignId" }, conditionalValues, vm, conn, transaction);

                    if (CampaignByProductValue.Status == "Success" && CampaignByProductValue.DataVM is DataTable dt)
                    {
                        string json = JsonConvert.SerializeObject(dt);
                        var CampaignByProductValuedetails = JsonConvert.DeserializeObject<List<CampaignDetailByProductValueVM>>(json);

                        lst.FirstOrDefault().campaignDetailByProductValue = CampaignByProductValuedetails;
                        result.DataVM = lst;

                    }
                }
                else if (lst.FirstOrDefault().EnumName == "CampaignByProductTotalValue")
                {
                    var CampaignByProductTotalValue = await _repo.CampaignByProductTotalValueList(new[] { "D.CampaignId" }, conditionalValues, vm, conn, transaction);

                    if (CampaignByProductTotalValue.Status == "Success" && CampaignByProductTotalValue.DataVM is DataTable dt)
                    {
                        string json = JsonConvert.SerializeObject(dt);
                        var CampaignByProductTotalValuedetails = JsonConvert.DeserializeObject<List<CampaignDetailByProductTotalValueVM>>(json);

                        lst.FirstOrDefault().campaignDetailByProductTotalValue = CampaignByProductTotalValuedetails;
                        result.DataVM = lst;

                    }
                }
                else if (lst.FirstOrDefault().EnumName == "CampaignByInvoiceValue")
                {
                    var CampaignByInvoiceValue = await _repo.CampaignDetailByInvoiceValueList(new[] { "D.CampaignId" }, conditionalValues, vm, conn, transaction);

                    if (CampaignByInvoiceValue.Status == "Success" && CampaignByInvoiceValue.DataVM is DataTable dt)
                    {
                        string json = JsonConvert.SerializeObject(dt);

                        var CampaignByInvoiceValuedetail = JsonConvert.DeserializeObject<List<CampaignDetailByInvoiceValueVM>>(json);

                        lst.FirstOrDefault().campaignDetailByInvoiceValue = CampaignByInvoiceValuedetail;
                        result.DataVM = lst;
                    }
                }
                if (isNewConnection && result.Status == "Success")
                {
                    transaction.Commit();
                }
                else
                {
                    throw new Exception(result.Message);
                }

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }
                result.Message = ex.Message.ToString();
                result.ExMessage = ex.ToString();
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }

        public async Task<ResultVM> ListAsDataTable(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            CampaignRepository _repo = new CampaignRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();

                result = await _repo.ListAsDataTable(conditionalFields, conditionalValues, vm, conn, transaction);

                if (isNewConnection && result.Status == "Success")
                {
                    transaction.Commit();
                }
                else
                {
                    throw new Exception(result.Message);
                }

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }
                result.Message = ex.ToString();
                result.ExMessage = ex.ToString();
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }

        public async Task<ResultVM> Dropdown()
        {
            CampaignRepository _repo = new CampaignRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();

                result = await _repo.Dropdown(conn, transaction);

                if (isNewConnection && result.Status == "Success")
                {
                    transaction.Commit();
                }
                else
                {
                    throw new Exception(result.Message);
                }

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }
                result.Message = ex.ToString();
                result.ExMessage = ex.ToString();
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }


        public async Task<ResultVM> MultiplePost(CommonVM vm)
        {
            CampaignRepository _repo = new CampaignRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = vm.IDs, DataVM = null };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();

                result = await _repo.MultiplePost(vm, conn, transaction);

                if (isNewConnection && result.Status == "Success")
                {
                    transaction.Commit();
                }
                else
                {
                    throw new Exception(result.Message);
                }

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }

                result.ExMessage = ex.ToString();
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }


        public async Task<ResultVM> GetGridData(GridOptions options,string EnumId, string[] conditionalFields, string[] conditionalValues)
        {
            CampaignRepository _repo = new CampaignRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();

                result = await _repo.GetGridData(options, EnumId, conditionalFields, conditionalValues, conn, transaction);

                if (isNewConnection && result.Status == "Success")
                {
                    transaction.Commit();
                }
                else
                {
                    throw new Exception(result.Message);
                }

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }
                result.Message = ex.ToString();
                result.ExMessage = ex.ToString();
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }


        public async Task<ResultVM> GetDetailsGridData(GridOptions options, string EnumId, string[] conditionalFields, string[] conditionalValues)
        {
            CampaignRepository _repo = new CampaignRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();

                result = await _repo.GetDetailsGridData(options, EnumId, conditionalFields, conditionalValues, conn, transaction);

                if (isNewConnection && result.Status == "Success")
                {
                    transaction.Commit();
                }
                else
                {
                    throw new Exception(result.Message);
                }

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }
                result.Message = ex.ToString();
                result.ExMessage = ex.ToString();
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }
    }


}
