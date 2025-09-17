using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using CommonUtility;
using System.Data;

namespace Repository
{
    public interface IDataConnector
    {
    }
    public class DataConnector : IDataConnector
    {
        private readonly IConfiguration configuration;

        public DataConnector(IConfiguration _configuration)
        {
            this.configuration = _configuration;
        }



    }
}
