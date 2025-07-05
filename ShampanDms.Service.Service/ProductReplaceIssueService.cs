using ShampanDms.Service.Repository;
using ShampanDms.Service.ViewModel;
using ShampanDms.Service.ViewModel.CommonVMs;
using ShampanDms.Service.ViewModel.Utility;
using ShampanDms.Service.ViewModel.KendoCommon;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace ShampanDms.Service.Service
{
    public class ProductReplaceIssueService
    {
        CommonRepository _commonRepo = new CommonRepository();

        // 1. Insert Method
        public async Task<ResultVM> Insert(ProductReplaceIssueVM model)
        {
            ProductReplaceIssueRepository _repo = new ProductReplaceIssueRepository();
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

                // Generate Code
                string code = _commonRepo.GenerateCode("ProductReplaceIssue", "ProductReplaceIssue", model.BranchName, model.BranchId, conn, transaction);

                if (!string.IsNullOrEmpty(code))
                {
                    model.Code = code;

                    // Insert ProductReplaceIssue
                    result = await _repo.Insert(model, conn, transaction);
                    model.Id = Convert.ToInt32(result.Id);

                    if (result.Status.ToLower() == "success")
                    {
                        // Insert Details
                        int lineNo = 1;
                        foreach (var details in model.ProductReplaceIssueDetails)
                        {
                            details.ProductReplaceIssueId = model.Id;
                            details.Line = lineNo;

                            var resultDetail = await _repo.InsertDetails(details, conn, transaction);
                            if (resultDetail.Status.ToLower() == "success")
                            {
                                lineNo++;
                            }
                            else
                            {
                                throw new Exception(resultDetail.Message);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception(result.Message);
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

        // 2. Update Method
        public async Task<ResultVM> Update(ProductReplaceIssueVM model)
        {
            ProductReplaceIssueRepository _repo = new ProductReplaceIssueRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = model.Id.ToString(), DataVM = model };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;
                transaction = conn.BeginTransaction();

                // Update ProductReplaceIssue
                result = await _repo.Update(model, conn, transaction);

                if (result.Status.ToLower() == "success")
                {
                    // Update Details
                    int lineNo = 1;
                    foreach (var details in model.ProductReplaceIssueDetails)
                    {
                        details.ProductReplaceIssueId = model.Id;
                        details.Line = lineNo;

                        var resultDetail = await _repo.InsertDetails(details, conn, transaction);
                        if (resultDetail.Status.ToLower() == "success")
                        {
                            lineNo++;
                        }
                        else
                        {
                            throw new Exception(resultDetail.Message);
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

        // 3. Delete Method
        public async Task<ResultVM> Delete(int[] IDs)
        {
            ProductReplaceIssueRepository _repo = new ProductReplaceIssueRepository();
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

                result = await _repo.Delete(IDs, conn, transaction);

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

        // 4. GetGridData Method
        public async Task<ResultVM> GetGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues)
        {
            ProductReplaceIssueRepository _repo = new ProductReplaceIssueRepository();
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

                result = await _repo.GetGridData(options, conditionalFields, conditionalValues, conn, transaction);

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

        // 5. GetDetailsGridData Method
        public async Task<ResultVM> GetDetailsGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues)
        {
            ProductReplaceIssueRepository _repo = new ProductReplaceIssueRepository();
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

                result = await _repo.GetDetailsGridData(options, conditionalFields, conditionalValues, conn, transaction);

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
    }
}
