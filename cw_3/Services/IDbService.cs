using System.Collections.Generic;
using cw_3.Models;
using System.Data.SqlClient;

namespace cw_3.Services
{
    public interface IDbService
    {
        public List<object[]> ExecuteSelect(SqlCommand command);
        public void ExecuteInsert(SqlCommand command);
        public SqlConnection GetConnection();
    }
}