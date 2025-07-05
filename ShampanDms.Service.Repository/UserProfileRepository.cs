using ShampanDms.Service.ViewModel;
using ShampanDms.Service.ViewModel.CommonVMs;
using ShampanDms.Service.ViewModel.KendoCommon;
using ShampanDms.Service.ViewModel.Utility;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;

namespace ShampanDms.Service.Repository
{
    public class UserProfileRepository : CommonRepository
    {
        // Insert Method
        public async Task<ResultVM> Insert(UserProfileVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        // Update Method
        public async Task<ResultVM> Update(UserProfileVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        // Delete Method
        public async Task<ResultVM> Delete(CommonVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        // List Method
        public async Task<ResultVM> List(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetAuthConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }


                string query = $@"
SELECT 
 U.Id 
,U.UserName
,U.FullName
,U.Email
,U.PhoneNumber
,U.PasswordHash
,U.NormalizedPassword
,ISNULL(U.IsHeadOffice,0) IsHeadOffice
,ISNULL(U.IsSalePerson,0) IsSalePerson

,ISNULL(U.SalePersonId,0) SalePersonId
,ISNULL(SP.Name,'') SalePersonName
,ISNULL(SP.Code,'') SalePersonCode


FROM 
[{DatabaseHelper.AuthDbName()}].[dbo].AspNetUsers AS U
LEFT OUTER JOIN [{DatabaseHelper.DBName()}].[dbo].SalesPersons SP ON ISNULL(U.SalePersonId,0) = ISNULL(SP.Id,0)

WHERE 1 = 1 ";

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND U.Id = @Id ";
                }

                // Apply additional conditions
                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
                }

                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new UserProfileVM
                {
                    Id = row["Id"].ToString(),
                    UserName = row["UserName"].ToString(),
                    FullName = row["FullName"].ToString(),
                    SalePersonCode = row["SalePersonCode"].ToString(),
                    SalePersonName = row["SalePersonName"].ToString(),
                    SalePersonId = Convert.ToInt32(row["SalePersonId"]),
                    IsHeadOffice = Convert.ToBoolean(row["IsHeadOffice"]),
                    IsSalePerson = Convert.ToBoolean(row["IsSalePerson"]),
                    Email = row["Email"].ToString(),
                    PhoneNumber = row["PhoneNumber"].ToString(),
                    Password = row["NormalizedPassword"].ToString(),
                    ConfirmPassword = row["NormalizedPassword"].ToString(),
                    CurrentPassword = row["NormalizedPassword"].ToString(),
                    NormalizedPassword = row["NormalizedPassword"].ToString(),
                }).ToList();

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;

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

        // ListAsDataTable Method
        public async Task<ResultVM> ListAsDataTable(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetAuthConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                string query = @"
SELECT 
 U.Id 
,U.UserName
,U.FullName
,U.Email
,U.PhoneNumber
,U.PasswordHash

FROM 

[dbo].[AspNetUsers] AS U

WHERE 1 = 1 ";

                DataTable dataTable = new DataTable();

                // Apply additional conditions
                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                objComm.Fill(dataTable);

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = dataTable;

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

        // Dropdown Method
        public async Task<ResultVM> Dropdown(SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetAuthConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                string query = @"
                SELECT Id, UserName Name
                FROM [dbo].[AspNetUsers]
                WHERE 1 = 1
                ORDER BY UserName ";

                DataTable dropdownData = new DataTable();
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    if (transaction != null)
                    {
                        adapter.SelectCommand.Transaction = transaction;
                    }
                    adapter.Fill(dropdownData);
                }

                result.Status = "Success";
                result.Message = "Dropdown data retrieved successfully.";
                result.DataVM = dropdownData;

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

        public async Task<ResultVM> GetGridData(GridOptions options, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetAuthConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                var data = new GridEntity<UserProfileVM>();

                // Define your SQL query string
                string sqlQuery = $@"
            -- Count query
                    SELECT COUNT(DISTINCT U.Id) AS totalcount
                    FROM 
                    [{DatabaseHelper.AuthDbName()}].[dbo].AspNetUsers AS U
                    LEFT OUTER JOIN [{DatabaseHelper.DBName()}].[dbo].SalesPersons SP ON ISNULL(U.SalePersonId,0) = ISNULL(SP.Id,0)
                    WHERE 1 = 1
                    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<UserProfileVM>.FilterCondition(options.filter) + ")" : "") + @"

                    -- Data query with pagination and sorting
                    SELECT * 
                    FROM (
                        SELECT 
                         ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "U.UserName DESC ") + $@") AS rowindex,
                         U.Id 
                        ,U.UserName
                        ,U.FullName
                        ,U.Email
                        ,U.PhoneNumber
                        ,U.PasswordHash
                        ,ISNULL(U.IsHeadOffice,0) IsHeadOffice                        
                        ,ISNULL(U.IsSalePerson,0) IsSalePerson
                        ,ISNULL(U.SalePersonId,0) SalePersonId
                        ,ISNULL(SP.Name,'') SalePersonName
                        ,ISNULL(SP.Code,'') SalePersonCode

                        FROM 
                        [{DatabaseHelper.AuthDbName()}].[dbo].AspNetUsers AS U
                        LEFT OUTER JOIN [{DatabaseHelper.DBName()}].[dbo].SalesPersons SP ON ISNULL(U.SalePersonId,0) = ISNULL(SP.Id,0)

                    WHERE 1 = 1
                  
            -- Add the filter condition
            " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<UserProfileVM>.FilterCondition(options.filter) + ")" : "") + @"

            ) AS a
            WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
        ";

                data = KendoGrid<UserProfileVM>.GetAuthGridData_CMD(options, sqlQuery, "U.UserName");

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
