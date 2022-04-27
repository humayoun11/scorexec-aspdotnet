using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Abp.Data;
using Abp.EntityFrameworkCore;
using Abp.Organizations;
using Microsoft.EntityFrameworkCore;
using ScoringAppReact.EntityFrameworkCore;
using ScoringAppReact.EntityFrameworkCore.Repositories;

namespace Rhithm.EntityFrameworkCore.Repositories
{
    public interface IRawSqlRepository
    {
        DbCommand CreateCommand(string commandText, CommandType commandType, params System.Data.SqlClient.SqlParameter[] parameters);
        void EnsureConnectionOpen();
    }


    public class RawSqlRepository : ScoringAppReactRepositoryBase<OrganizationUnit, long>, IRawSqlRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;

        public RawSqlRepository(IDbContextProvider<ScoringAppReactDbContext> dbContextProvider,
            IActiveTransactionProvider transactionProvider)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
        }

        public DbCommand CreateCommand(string commandText, CommandType commandType, params System.Data.SqlClient.SqlParameter[] parameters)
        {
            var command = Context.Database.GetDbConnection().CreateCommand();

            command.CommandText = commandText;
            command.CommandType = commandType;
            command.Transaction = GetActiveTransaction();

            foreach (var parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }

            return command;
        }
        public void EnsureConnectionOpen()
        {
            var connection = Context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }
        private DbTransaction GetActiveTransaction()
        {
            return (DbTransaction)_transactionProvider.GetActiveTransaction(new ActiveTransactionProviderArgs
            {
                {"ContextType", typeof(ScoringAppReactDbContext) },
                {"MultiTenancySide", MultiTenancySide }
            });
        }
    }
}
