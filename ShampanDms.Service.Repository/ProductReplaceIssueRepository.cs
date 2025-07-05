using ShampanDms.Service.ViewModel;
using ShampanDms.Service.ViewModel.CommonVMs;
using ShampanDms.Service.ViewModel.KendoCommon;
using ShampanDms.Service.ViewModel.Utility;
using System.Data;
using System.Data.SqlClient;

namespace ShampanDms.Service.Repository
{
    public class ProductReplaceIssueRepository : CommonRepository
    {
        // 1. Insert Method
        public async Task<ResultVM> Insert(ProductReplaceIssueVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                if (transaction == null)
                {
                    transaction = conn.BeginTransaction();
                }

                string query = @"
                INSERT INTO ProductReplaceIssues 
                (
                    Code, BranchId, CustomerId, CreatedBy, CreatedOn, CreatedFrom, LastUpdateFrom
                )
                VALUES 
                (
                    @Code, @BranchId, @CustomerId, @CreatedBy, @CreatedOn, @CreatedFrom, @LastUpdateFrom
                );
                SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Code", vm.Code ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@CustomerId", vm.CustomerId);
                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? "ERP");
                    cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                    cmd.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom ?? "ERP");
                    cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom ?? (object)DBNull.Value);

                    object newId = cmd.ExecuteScalar();
                    vm.Id = Convert.ToInt32(newId);

                    result.Status = "Success";
                    result.Message = "Data inserted successfully.";
                    result.Id = vm.Id.ToString();
                    result.DataVM = vm;
                }

                if (isNewConnection)
                {
                    transaction.Commit();
                }

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }

                result.ExMessage = ex.Message;
                result.Message = ex.Message;
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
        public async Task<ResultVM> Update(ProductReplaceIssueVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = vm.Id.ToString(), DataVM = vm };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                if (transaction == null)
                {
                    transaction = conn.BeginTransaction();
                }

                string query = @"
                UPDATE ProductReplaceIssues 
                SET 
                    BranchId = @BranchId, CustomerId = @CustomerId, 
                    LastModifiedBy = @LastModifiedBy, LastModifiedOn = GETDATE()
                WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@CustomerId", vm.CustomerId);
                    cmd.Parameters.AddWithValue("@LastModifiedBy", vm.CreatedBy ?? (object)DBNull.Value);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        result.Status = "Success";
                        result.Message = "Data updated successfully.";
                    }
                    else
                    {
                        result.Message = "No rows were updated.";
                        throw new Exception("No rows were updated.");
                    }
                }

                if (isNewConnection)
                {
                    transaction.Commit();
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
                result.ExMessage = ex.Message;
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
        public async Task<ResultVM> Delete(int[] IDs, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        // 4. List Method
        public async Task<ResultVM> List(string[] conditionalFields, string[] conditionalValues, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                string query = @"
                SELECT 
                    ISNULL(M.Id, 0) AS Id,
                    ISNULL(M.Code, '') AS Code,
                    ISNULL(M.BranchId, 0) AS BranchId,
                    ISNULL(M.CustomerId, 0) AS CustomerId,
                    ISNULL(M.CreatedBy, '') AS CreatedBy,
                    ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn
                FROM ProductReplaceIssues M
                WHERE 1 = 1";

                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);
                objComm.Fill(dataTable);

                var model = new List<ProductReplaceIssueVM>();
                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new ProductReplaceIssueVM
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Code = row["Code"].ToString(),
                        BranchId = Convert.ToInt32(row["BranchId"]),
                        CustomerId = Convert.ToInt32(row["CustomerId"]),
                        CreatedBy = row["CreatedBy"].ToString(),
                        CreatedOn = row["CreatedOn"].ToString()
                    });
                }

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = model;

                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = "Error in List.";
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

        // 5. ListAsDataTable Method
        public async Task<ResultVM> ListAsDataTable(string[] conditionalFields, string[] conditionalValues, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                string query = @"
                SELECT
                    ISNULL(M.Id, 0) AS Id,
                    ISNULL(M.Code, '') AS Code,
                    ISNULL(M.BranchId, 0) AS BranchId,
                    ISNULL(M.CustomerId, 0) AS CustomerId,
                    ISNULL(M.CreatedBy, '') AS CreatedBy
                FROM ProductReplaceIssues M
                WHERE 1 = 1";

                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);
                objComm.Fill(dataTable);

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = dataTable;

                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = "Error in ListAsDataTable.";
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

        // 6. InsertDetails Method
        public async Task<ResultVM> InsertDetails(ProductReplaceIssueDetailsVM details, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                string query = @"
                    INSERT INTO ProductReplaceIssueDetails
                    (ProductReplaceIssueId, ProductId, CtnQuantity, PcsQuantity, Quantity, UnitRate, SubTotal, SD, SDAmount)
                    VALUES 
                    (@ProductReplaceIssueId, @ProductId, @CtnQuantity, @PcsQuantity, @Quantity, @UnitRate, @SubTotal, @SD, @SDAmount);
                    SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@ProductReplaceIssueId", details.ProductReplaceIssueId);
                    cmd.Parameters.AddWithValue("@ProductId", details.ProductId);
                    cmd.Parameters.AddWithValue("@CtnQuantity", details.CtnQuantity);
                    cmd.Parameters.AddWithValue("@PcsQuantity", details.PcsQuantity);
                    cmd.Parameters.AddWithValue("@Quantity", details.Quantity);
                    cmd.Parameters.AddWithValue("@UnitRate", details.UnitRate);
                    cmd.Parameters.AddWithValue("@SubTotal", details.SubTotal);
                    cmd.Parameters.AddWithValue("@SD", details.SD);
                    cmd.Parameters.AddWithValue("@SDAmount", details.SDAmount);

                    object newId = cmd.ExecuteScalar();
                    details.Id = Convert.ToInt32(newId);

                    result.Status = "Success";
                    result.Message = "Details inserted successfully.";
                    result.Id = newId.ToString();
                    result.DataVM = details;
                }

                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
            }

            return result;
        }

        // 7. DetailsList Method
        public async Task<ResultVM> DetailsList(string[] conditionalFields, string[] conditionalValues, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                string query = @"
                SELECT 
                    ISNULL(M.Id, 0) AS Id,
                    ISNULL(M.ProductReplaceIssueId, 0) AS ProductReplaceIssueId,
                    ISNULL(M.ProductId, 0) AS ProductId,
                    ISNULL(M.Quantity, 0) AS Quantity,
                    ISNULL(M.Rate, 0) AS Rate,
                    ISNULL(M.SubTotal, 0) AS SubTotal
                FROM ProductReplaceIssueDetails M
                WHERE 1 = 1";

                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);
                objComm.Fill(dataTable);

                var model = new List<ProductReplaceIssueDetailsVM>();
                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new ProductReplaceIssueDetailsVM
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        ProductReplaceIssueId = Convert.ToInt32(row["ProductReplaceIssueId"]),
                        ProductId = Convert.ToInt32(row["ProductId"]),
                        Quantity = Convert.ToDecimal(row["Quantity"]),
                        UnitRate = Convert.ToDecimal(row["Rate"]),
                        SubTotal = Convert.ToDecimal(row["SubTotal"])
                    });
                }

                result.Status = "Success";
                result.Message = "Details retrieved successfully.";
                result.DataVM = model;

                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = "Error in DetailsList.";
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

        // 8. GetGridData Method
        public async Task<ResultVM> GetGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues, SqlConnection conn, SqlTransaction transaction)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                var data = new GridEntity<ProductReplaceIssueVM>();

                string sqlQuery = @"
    -- Count query
    SELECT COUNT(DISTINCT H.Id) AS totalcount
    FROM ProductReplaceIssues H
    WHERE 1 = 1
";

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex,
        ISNULL(H.Id, 0) AS Id,
        ISNULL(H.Code, '') AS Code,
        ISNULL(H.BranchId, 0) AS BranchId,
        ISNULL(H.CustomerId, 0) AS CustomerId,
        ISNULL(H.CreatedBy, '') AS CreatedBy,
        ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn
    FROM ProductReplaceIssues H
    WHERE 1 = 1
" + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<ProductReplaceIssueVM>.FilterCondition(options.filter) + ")" : "");

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                data = KendoGrid<ProductReplaceIssueVM>.GetTransactionalGridData_CMD(options, sqlQuery, "H.Id", conditionalFields, conditionalValues);

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = data;

                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
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

        // 9. GetDetailsGridData Method
        public async Task<ResultVM> GetDetailsGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues, SqlConnection conn, SqlTransaction transaction)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                var data = new GridEntity<ProductReplaceIssueDetailsVM>();

                string sqlQuery = @"
-- Count query
SELECT COUNT(DISTINCT D.Id) AS totalcount
FROM ProductReplaceIssueDetails D
LEFT OUTER JOIN Products P ON D.ProductId = P.Id
WHERE 1 = 1
";

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
-- Data query with pagination and sorting
SELECT * 
FROM (
    SELECT 
    ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "D.Id DESC") + @") AS rowindex,
    ISNULL(D.Id, 0) AS Id,
    ISNULL(D.ProductReplaceIssueId, 0) AS ProductReplaceIssueId,
    ISNULL(D.ProductId, 0) AS ProductId,
    ISNULL(P.Name, '') AS ProductName,  -- Assuming Product Name is from Products table
    ISNULL(D.CtnQuantity, 0) AS CtnQuantity,
    ISNULL(D.PcsQuantity, 0) AS PcsQuantity,
    ISNULL(D.Quantity, 0) AS Quantity,
    ISNULL(D.UnitRate, 0) AS UnitRate,
    ISNULL(D.SubTotal, 0) AS SubTotal,
    ISNULL(D.SD, 0) AS SD,
    ISNULL(D.SDAmount, 0) AS SDAmount
FROM ProductReplaceIssueDetails D
LEFT OUTER JOIN Products P ON D.ProductId = P.Id
WHERE 1 = 1
" + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<ProductReplaceIssueDetailsVM>.FilterCondition(options.filter) + ")" : "");

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
) AS a
WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                data = KendoGrid<ProductReplaceIssueDetailsVM>.GetTransactionalGridData_CMD(options, sqlQuery, "D.Id", conditionalFields, conditionalValues);

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = data;

                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
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
