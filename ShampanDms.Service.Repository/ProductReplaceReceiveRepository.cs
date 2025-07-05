using ShampanDms.Service.ViewModel;
using ShampanDms.Service.ViewModel.CommonVMs;
using ShampanDms.Service.ViewModel.KendoCommon;
using ShampanDms.Service.ViewModel.Utility;
using System.Data;
using System.Data.SqlClient;

namespace ShampanDms.Service.Repository
{
    public class ProductReplaceReceiveRepository : CommonRepository
    {
        // 1. Insert Method
        public async Task<ResultVM> Insert(ProductReplaceReceiveVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
                INSERT INTO ProductReplaceReceives 
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
        public async Task<ResultVM> Update(ProductReplaceReceiveVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
                UPDATE ProductReplaceReceives 
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
                FROM ProductReplaceReceives M
                WHERE 1 = 1";

                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);
                objComm.Fill(dataTable);

                var model = new List<ProductReplaceReceiveVM>();
                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new ProductReplaceReceiveVM
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
                FROM ProductReplaceReceives M
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
        public async Task<ResultVM> InsertDetails(ProductReplaceReceiveDetailsVM details, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                string query = @"
                    INSERT INTO ProductReplaceReceiveDetails
                    (ProductReplaceReceiveId, ProductId, Quantity, Rate, SubTotal)
                    VALUES 
                    (@ProductReplaceReceiveId, @ProductId, @Quantity, @Rate, @SubTotal);
                    SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@ProductReplaceReceiveId", details.ProductReplaceReceiveId);
                    cmd.Parameters.AddWithValue("@ProductId", details.ProductId);
                    cmd.Parameters.AddWithValue("@Quantity", details.Quantity);
                    cmd.Parameters.AddWithValue("@Rate", details.UnitRate);
                    cmd.Parameters.AddWithValue("@SubTotal", details.SubTotal);

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
                    ISNULL(M.ProductReplaceReceiveId, 0) AS ProductReplaceReceiveId,
                    ISNULL(M.ProductId, 0) AS ProductId,
                    ISNULL(M.Quantity, 0) AS Quantity,
                    ISNULL(M.Rate, 0) AS Rate,
                    ISNULL(M.SubTotal, 0) AS SubTotal
                FROM ProductReplaceReceiveDetails M
                WHERE 1 = 1";

                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);
                objComm.Fill(dataTable);

                var model = new List<ProductReplaceReceiveDetailsVM>();
                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new ProductReplaceReceiveDetailsVM
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        ProductReplaceReceiveId = Convert.ToInt32(row["ProductReplaceReceiveId"]),
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

                var data = new GridEntity<ProductReplaceReceiveVM>();

                string sqlQuery = @"
    -- Count query
    SELECT COUNT(DISTINCT H.Id) AS totalcount
    FROM ProductReplaceReceives H
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
    FROM ProductReplaceReceives H
    WHERE 1 = 1
" + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<ProductReplaceReceiveVM>.FilterCondition(options.filter) + ")" : "");

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                data = KendoGrid<ProductReplaceReceiveVM>.GetTransactionalGridData_CMD(options, sqlQuery, "H.Id", conditionalFields, conditionalValues);

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

                var data = new GridEntity<ProductReplaceReceiveDetailsVM>();

                string sqlQuery = @"
    -- Count query
    SELECT COUNT(DISTINCT D.Id) AS totalcount
    FROM ProductReplaceReceiveDetails D
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
        ISNULL(D.ProductReplaceReceiveId, 0) AS ProductReplaceReceiveId,
        ISNULL(D.ProductId, 0) AS ProductId,
        ISNULL(D.Quantity, 0) AS Quantity,
        ISNULL(D.Rate, 0) AS Rate,
        ISNULL(D.SubTotal, 0) AS SubTotal
    FROM ProductReplaceReceiveDetails D
    WHERE 1 = 1
" + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<ProductReplaceReceiveDetailsVM>.FilterCondition(options.filter) + ")" : "");

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                data = KendoGrid<ProductReplaceReceiveDetailsVM>.GetTransactionalGridData_CMD(options, sqlQuery, "D.Id", conditionalFields, conditionalValues);

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

        // 10. ProductReplaceReceiveList Method
        public async Task<ResultVM> ProductReplaceReceiveList(string[] conditionalFields, string[] conditionalValues, SqlConnection conn = null, SqlTransaction transaction = null)
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
                    ISNULL(M.Code, '') AS Code,
                    ISNULL(M.BranchId, 0) AS BranchId,
                    ISNULL(M.CustomerId, 0) AS CustomerId,
                    ISNULL(M.CreatedBy, '') AS CreatedBy,
                    ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn
                FROM ProductReplaceReceives M
                WHERE 1 = 1";

                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);
                objComm.Fill(dataTable);

                var model = new List<ProductReplaceReceiveVM>();
                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new ProductReplaceReceiveVM
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
                result.Message = "Error in ProductReplaceReceiveList.";
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

        // 11. ProductReplaceReceiveDetailsList Method
        public async Task<ResultVM> ProductReplaceReceiveDetailsList(string[] conditionalFields, string[] conditionalValues, SqlConnection conn = null, SqlTransaction transaction = null)
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
                    ISNULL(D.Id, 0) AS Id,
                    ISNULL(D.ProductReplaceReceiveId, 0) AS ProductReplaceReceiveId,
                    ISNULL(D.ProductId, 0) AS ProductId,
                    ISNULL(D.Quantity, 0) AS Quantity,
                    ISNULL(D.Rate, 0) AS Rate,
                    ISNULL(D.SubTotal, 0) AS SubTotal
                FROM ProductReplaceReceiveDetails D
                WHERE 1 = 1";

                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);
                objComm.Fill(dataTable);

                var model = new List<ProductReplaceReceiveDetailsVM>();
                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new ProductReplaceReceiveDetailsVM
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        ProductReplaceReceiveId = Convert.ToInt32(row["ProductReplaceReceiveId"]),
                        ProductId = Convert.ToInt32(row["ProductId"]),
                        Quantity = Convert.ToDecimal(row["Quantity"]),
                        UnitRate = Convert.ToDecimal(row["Rate"]),
                        SubTotal = Convert.ToDecimal(row["SubTotal"])
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
                result.Message = "Error in ProductReplaceReceiveDetailsList.";
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
