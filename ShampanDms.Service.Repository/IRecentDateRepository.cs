using ShampanDms.Service.ViewModel;
using ShampanDms.Service.ViewModel.CommonVMs;
using ShampanDms.Service.ViewModel.KendoCommon;
using ShampanDms.Service.ViewModel.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShampanDms.Service.Repository
{
    public interface IRecentDateRepository
    {
        RecentDateVM GetRecentDate();
    }
}
namespace ShampanDms.Service.Repository
{
    public class RecentDateRepository : IRecentDateRepository
    {
        public RecentDateVM GetRecentDate()
        {
            return new RecentDateVM { Date = DateTime.Now };
        }
    }
}
