using ShampanDms.Service.ViewModel;
using ShampanDms.Service.ViewModel.CommonVMs;
using ShampanDms.Service.ViewModel.Utility;
using System.Data;
using System.Data.SqlClient;

namespace ShampanDms.Service.Repository
{
    public class MISReportRepository : CommonRepository
    {







        public async Task<ResultVM> ExportSaleAndPayment(SalePaymentVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
    SL,
    c.Name CustomerName,
    Date,
    [Invoice No.],
    [Payment No.],
    [Payment Mode],
    ISNULL(Opening,0) Opening,
    [Receive Amount],
    [Sales Amount],
    SUM(ISNULL(Opening,0) - [Sales Amount] + [Receive Amount]) 
        OVER (PARTITION BY a.CustomerID ORDER BY a.Date, SL ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS [Closing Balance],
    Remarks
FROM (
    SELECT a.*
    FROM (
        SELECT 'C' SL, 
               InvoiceDateTime Date,
               Code [Invoice No.], 
               '' [Payment No.],
               CustomerID,
               '' [Payment Mode],
               0 Opening,
               0 [Receive Amount],
               GrandTotalAmount [Sales Amount], 
               'Sales' Remarks 
        FROM SaleDeleveries h
        WHERE 1=1
            AND BranchId=@BranchId
            AND CustomerId=@CustomerId
            AND h.InvoiceDateTime >=@DateFrom
            AND h.InvoiceDateTime <=@DateTo
            AND IsPost=1
        
        UNION ALL
        
        SELECT 'B' SL, 
               CreatedOn Date,
               '' [Invoice No.], 
               Code [Payment No.],
               CustomerID,
               ModeOfPayment [Payment Mode],
               0 Opening,
               Amount [Receive Amount],
               0 [Sales Amount], 
               'Payment' Remarks 
        FROM CustomerPaymentCollection h
        WHERE 1=1
            AND BranchId=@BranchId
            AND CustomerId=@CustomerId
            AND h.CreatedOn >=@DateFrom
            AND h.CreatedOn <=@DateTo
            AND IsPost=1
    ) AS a
    
    UNION ALL
    
    SELECT 'A' SL,
           @Date Date,
           '' [Invoice No.],
           '' [Payment No.], 
           a.CustomerID,
           '' [Payment Mode],
           SUM(a.Opening) Opening,
           0 [Receive Amount],
           0 [Sales Amount], 
           'Opening' Remarks
    FROM (
        SELECT DISTINCT CustomerID, SUM(-1*GrandTotalAmount) Opening 
        FROM SaleDeleveries h
        WHERE 1=1
            AND BranchId=@BranchId
            AND h.InvoiceDateTime <@DateFrom
            AND IsPost=1
        GROUP BY h.CustomerID
        
        UNION ALL
        
        SELECT DISTINCT CustomerID, SUM(Amount) Opening 
        FROM CustomerPaymentCollection h
        WHERE 1=1
            AND BranchId=@BranchId
            AND CustomerId=@CustomerId
            AND h.CreatedOn <@DateFrom
            AND IsPost=1
        GROUP BY h.CustomerID
    ) AS a
    GROUP BY a.CustomerID
) AS a 
LEFT OUTER JOIN Customers c ON a.CustomerID=c.Id
ORDER BY c.Code, a.Date, SL
";

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // --- Add parameters for every @Variable in your query ---
                objComm.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId ?? (object)DBNull.Value);
                objComm.SelectCommand.Parameters.AddWithValue("@CustomerId", vm.CustomerId ?? (object)DBNull.Value);
                objComm.SelectCommand.Parameters.AddWithValue("@DateFrom", vm.DateFrom ?? (object)DBNull.Value);
                objComm.SelectCommand.Parameters.AddWithValue("@DateTo", vm.DateTo ?? (object)DBNull.Value);
                objComm.SelectCommand.Parameters.AddWithValue("@Date", vm.Date ?? (object)DBNull.Value);
                // --------------------------------------------------------

                objComm.Fill(dataTable);

                // Remove rows with Id = 0 if needed (not used in this query; you may skip this)
                // foreach (DataRow row in dataTable.Select("Id = 0"))
                // {
                //     dataTable.Rows.Remove(row);
                // }

                var lst = new List<SalePaymentVM>();
                int serialNumber = 1;

                foreach (DataRow row in dataTable.Rows)
                {
                    lst.Add(new SalePaymentVM
                    {
                        SL = row["SL"].ToString(),
                        CustomerName = row["CustomerName"].ToString(),
                        Date = row["Date"].ToString(),
                        InvoiceNo = row["Invoice No."].ToString(),
                        PaymentNo = row["Payment No."].ToString(),
                        PaymentMode = row["Payment Mode"].ToString(),
                        Opening = row["Opening"].ToString(),
                        ReceiveAmount = Convert.ToInt32(row["Receive Amount"]),
                        SalesAmount = Convert.ToInt32(row["Sales Amount"]),
                        ClosingBalance = Convert.ToInt32(row["Closing Balance"]),
                        Remarks = row["Remarks"].ToString(),
                    });

                    serialNumber++;
                }

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = lst;
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




        public async Task<ResultVM> ExportSaleAndPaymentSummary(SalePaymentVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
    c.Name CustomerName,
    SUM(ISNULL(Opening,0)) Opening,
    SUM([Receive Amount]) [Receive Amount],
    SUM([Sales Amount]) [Sales Amount],
    SUM(ISNULL(Opening,0)) - SUM([Sales Amount]) + SUM([Receive Amount]) AS [Closing Balance]
FROM (
    ------------------------------- Transaction --------------------------------------------
    SELECT a.*
    FROM (
        ------------------------------- Transaction --------------------------------------------
        SELECT 'C' SL, 
               InvoiceDateTime Date,
               Code [Invoice No.], 
               '' [Payment No.],
               CustomerID,
               '' [Payment Mode],
               0 Opening,
               0 [Receive Amount],
               GrandTotalAmount [Sales Amount], 
               'Sales' Remarks 
        FROM SaleDeleveries h
        WHERE 1=1
            AND BranchId=@BranchId
            AND CustomerId=@CustomerId
            AND h.InvoiceDateTime >=@DateFrom
            AND h.InvoiceDateTime <=@DateTo
            AND IsPost=1
        
        UNION ALL
        
        SELECT 'B' SL, 
               CreatedOn Date,
               '' [Invoice No.], 
               Code [Payment No.],
               CustomerID,
               ModeOfPayment [Payment Mode],
               0 Opening,
               Amount [Receive Amount],
               0 [Sales Amount], 
               'Payment' Remarks 
        FROM CustomerPaymentCollection h
        WHERE 1=1
            AND BranchId=@BranchId
            AND CustomerId=@CustomerId
            AND h.CreatedOn >=@DateFrom
            AND h.CreatedOn <=@DateTo
            AND IsPost=1
    ) AS a
    ------------------------------- Transaction --------------------------------------------
    
    ------------------------------- Opening --------------------------------------------
    UNION ALL
    
    SELECT 'A' SL,
           @Date Date,
           '' [Invoice No.],
           '' [Payment No.], 
           a.CustomerID,
           '' [Payment Mode],
           SUM(a.Opening) Opening,
           0 [Receive Amount],
           0 [Sales Amount], 
           'Opening' Remarks
    FROM (
        SELECT DISTINCT CustomerID, SUM(-1*GrandTotalAmount) Opening 
        FROM SaleDeleveries h
        WHERE 1=1
            AND BranchId=@BranchId
            AND CustomerId=@CustomerId
            AND h.InvoiceDateTime <@DateFrom
            AND IsPost=1
        GROUP BY h.CustomerID
        
        UNION ALL
        
        SELECT DISTINCT CustomerID, SUM(Amount) Opening 
        FROM CustomerPaymentCollection h
        WHERE 1=1
            AND BranchId=@BranchId
           -- AND CustomerId=@CustomerId
            AND h.CreatedOn <@DateFrom
            AND IsPost=1
        GROUP BY h.CustomerID
    ) AS a
    GROUP BY a.CustomerID
) AS a 
LEFT OUTER JOIN Customers c ON a.CustomerID=c.Id
GROUP BY c.Name, a.CustomerID
";

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                objComm.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId ?? (object)DBNull.Value);
                objComm.SelectCommand.Parameters.AddWithValue("@CustomerId", vm.CustomerId ?? (object)DBNull.Value);
                objComm.SelectCommand.Parameters.AddWithValue("@DateFrom", vm.DateFrom ?? (object)DBNull.Value);
                objComm.SelectCommand.Parameters.AddWithValue("@DateTo", vm.DateTo ?? (object)DBNull.Value);
                objComm.SelectCommand.Parameters.AddWithValue("@Date", vm.Date ?? (object)DBNull.Value);

                objComm.Fill(dataTable);
                var lst = new List<SalePaymentVM>();
                int serialNumber = 1;

                foreach (DataRow row in dataTable.Rows)
                {
                    lst.Add(new SalePaymentVM
                    {
                        CustomerName = row["CustomerName"].ToString(),
                        Opening = row["Opening"].ToString(),
                        ReceiveAmount = Convert.ToInt32(row["Receive Amount"]),
                        SalesAmount = Convert.ToInt32(row["Sales Amount"]),
                        ClosingBalance = Convert.ToInt32(row["Closing Balance"])
                    });

                    serialNumber++;
                }

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = lst;
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
