using ShampanDms.Service.ViewModel.CommonVMs;
using ShampanDms.Service.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShampanDms.Service.Repository;
using System.Text.Json;
using System.Data;
using Microsoft.Extensions.Configuration;
using ShampanDms.Service.ViewModel.Utility;
using ShampanDms.Service.ViewModel.KendoCommon;

namespace ShampanDms.Service.Service
{
    public interface IRecentDateService
    {
        RecentDateVM GetRecentDate();
    }

}

namespace ShampanDms.Service.Service
{
    public class RecentDateService : IRecentDateService
    {
        private readonly IRecentDateRepository _recentDateRepository;

        public RecentDateService(IRecentDateRepository recentDateRepository)
        {
            _recentDateRepository = recentDateRepository;
        }

        public RecentDateVM GetRecentDate()
        {
            return _recentDateRepository.GetRecentDate();
        }
    }

}



