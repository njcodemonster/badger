using CommonHelper;
using Dapper;
using GenericModals.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace badgerApi.Interfaces
{
    public interface IClaimRepository
    {
        Task<PoClaim> ClaimInspect(int poId, int userId);
        Task<PoClaim> RemoveClaimInspect(int poId, int userId);
        Task<PoClaim> GetClaimInspect(int poId);
        Task<PoClaim> RemoveClaimPublish(int poId, int userId);
        Task<PoClaim> ClaimPublish(int poId, int userId);
        Task<PoClaim> GetClaimPublish(int poId);
        Task<PoClaim> GetClaim(int poId);
    }

    public class ClaimRepository : IClaimRepository
    {
        private readonly IConfiguration _config;
        private string TableName = "purchase_orders";
        private string selectlimit = "30";
        private CommonHelper.CommonHelper _common;
        public ClaimRepository(IConfiguration config)
        {

            _config = config;
            selectlimit = _config.GetValue<string>("configs:Default_select_Limit");
            _common = new CommonHelper.CommonHelper();

        }
        private IDbConnection Connection
        {
            get
            {
                return new MySqlConnection(_config.GetConnectionString("ProductsDatabase"));
            }
        }

        public async Task<PoClaim> GetClaimInspect(int poId)
        {
            var query = $"SELECT po_id,inspect_claimer,inspect_claimed_at,u.name as inspect_claimer_name" +
                $" FROM po_claim pc INNER JOIN users u ON pc.inspect_claimer = u.user_id WHERE po_id={poId} AND inspect_claimer IS NOT NULL LIMIT 1";
            using (IDbConnection conn = Connection)
            {
                var result = await conn.QuerySingleOrDefaultAsync<PoClaim>(query);
                return result == null ? new PoClaim() : result;
            }
        }

        public async Task<PoClaim> GetClaimPublish(int poId)
        {
            var query = $"SELECT po_id,publish_claimer,publish_claimed_at,u.name as publish_claimer_name" +
                $" FROM po_claim pc INNER JOIN users u ON pc.inspect_claimer = u.user_id WHERE po_id={poId} AND publish_claimer IS NOT NULL LIMIT 1";
            using (IDbConnection conn = Connection)
            {
                var result = await conn.QuerySingleOrDefaultAsync<PoClaim>(query);
                return result == null ? new PoClaim() : result;
            }
        }

        public async Task<PoClaim> GetClaim(int poId)
        {
            var query = string.Format(@" SELECT po_id,inspect_claimer, inspect_claimed_at, publish_claimer,
                        publish_claimed_at,u.name as inspect_claimer_name, u1.name as publish_claimer_name, 
                        u.claim_color AS inspect_claim_color, u1.claim_color AS publish_claim_color
                        FROM po_claim pc LEFT JOIN users u ON pc.inspect_claimer = u.user_id
                        LEFT JOIN users u1 ON pc.publish_claimer = u1.user_id
                        WHERE po_id={0} LIMIT 1", poId);
            using (IDbConnection conn = Connection)
            {
                var result = await conn.QuerySingleOrDefaultAsync<PoClaim>(query);
                return result == null ? new PoClaim() : result;
            }
        }

        public async Task<PoClaim> ClaimInspect(int poId, int userId)
        {
            var poClaim = new PoClaim
            {
                inspect_claimer = userId,
                po_id = poId,
                inspect_claimed_at = _common.GetTimeStemp()
            };
            string query = string.Format(@"INSERT INTO po_claim (po_id,inspect_claimer,inspect_claimed_at) VALUES ({0},{1},{2})
                ON DUPLICATE KEY UPDATE inspect_claimer = {1}, inspect_claimed_at = {2}"
            , poClaim.po_id, poClaim.inspect_claimer, poClaim.inspect_claimed_at);
            using (IDbConnection conn = Connection)
            {
                var result = await conn.ExecuteAsync(query);
                // poClaim.inspect_claimer_name = await GetUsernameByClaim(poId, ClaimerType.InspectClaimer);
            }
            return await GetClaim(poId);
        }

        public async Task<PoClaim> ClaimPublish(int poId, int userId)
        {
            var poClaim = new PoClaim
            {
                publish_claimer = userId,
                po_id = poId,
                publish_claimed_at = _common.GetTimeStemp()
            };
            string query = string.Format(@"INSERT INTO po_claim (po_id,publish_claimer,publish_claimed_at) VALUES ({0},{1},{2})
                ON DUPLICATE KEY UPDATE publish_claimer = {1}, publish_claimed_at = {2}"
            , poClaim.po_id, poClaim.publish_claimer, poClaim.publish_claimed_at);
            using (IDbConnection conn = Connection)
            {
                var result = await conn.ExecuteAsync(query);
                // poClaim.publish_claimer_name = await GetUsernameByClaim(poId, ClaimerType.PublishClaimer);
            }
            return await GetClaim(poId);
        }

        public async Task<PoClaim> RemoveClaimInspect(int poId, int userId)
        {
            var poClaim = new PoClaim
            {
                inspect_claimer = null,
                po_id = poId,
                inspect_claimed_at = _common.GetTimeStemp()
            };

            using (IDbConnection conn = Connection)
            {
                var result = await conn.ExecuteAsync($"UPDATE po_claim set inspect_claimer=NULL,inspect_claimed_at='{poClaim.inspect_claimed_at}'" +
                                                $" where po_id={poId}");
                return await GetClaim(poId);
            }
        }

        public async Task<PoClaim> RemoveClaimPublish(int poId, int userId)
        {
            var poClaim = new PoClaim
            {
                publish_claimer = null,
                po_id = poId,
                publish_claimed_at = _common.GetTimeStemp()
            };

            using (IDbConnection conn = Connection)
            {
                var result = await conn.ExecuteAsync($"UPDATE po_claim set publish_claimer=NULL,publish_claimed_at='{poClaim.publish_claimed_at}'" +
                                                $" where po_id={poId}");
                return await GetClaim(poId);
            }
        }
    }
}
