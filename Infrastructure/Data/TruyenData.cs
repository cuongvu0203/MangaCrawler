using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SayHenTai_WebApp.Infrastructure.Entities;
using System.Data.SqlClient;

namespace SayHenTai_WebApp.Infrastructure.Data
{
    public class TruyenData
    {
        private readonly Connections _connections;
        public TruyenData(IOptions<Connections> options)
        {
            _connections = options.Value;
        }

        public async Task<IReadOnlyList<TRUYEN>> GetAllDanhSachTruyen()
        {
            var query = "SELECT TOP 100 * FROM TRUYEN ";
            using SqlConnection connection = new SqlConnection(_connections.SqlDefaultConnection);
            var queryData = (await connection.QueryAsync<TRUYEN>(query)).AsList();
            return queryData;
        }

        public async Task<IReadOnlyList<CHUONG_MUC>> GetChuongMucTruyenByIdTruyen(long Id)
        {
            var query = "select * from CHUONG_MUC where ID_TRUYEN=@Id ";
            using SqlConnection connection = new SqlConnection(_connections.SqlDefaultConnection);
            var queryData = (await connection.QueryAsync<CHUONG_MUC>(query, new { Id  = Id})).AsList();
            return queryData;
        }

        public virtual async Task<TRUYEN> GetTruyenByIdAsync(long Id)
        {
            var query = @"  SELECT * FROM TRUYEN WHERE ID=@Id";
            using SqlConnection connection = new(_connections.SqlDefaultConnection);
            return await connection.QueryFirstOrDefaultAsync<TRUYEN>(query, new { Id = Id });
        }

        public virtual async Task<IReadOnlyList<CHI_TIET_TRUYEN>> GetDetailTruyenByIdTruyenAndIdChuongMucAsync(long idChuongMuc, long idTruyen)
        {
            var query = "select * from CHI_TIET_TRUYEN where ID_TRUYEN=@idTruyen AND ID_CHUONG_MUC=@idChuongMuc ";
            using SqlConnection connection = new SqlConnection(_connections.SqlDefaultConnection);
            var queryData = (await connection.QueryAsync<CHI_TIET_TRUYEN>(query, new { idChuongMuc = idChuongMuc, idTruyen = idTruyen })).AsList();
            return queryData;
        }
    }
}
