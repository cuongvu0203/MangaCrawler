using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SayHenTai_WebApp.Infrastructure.Entities;
using SayHenTai_WebApp.Infrastructure.MongoDB;
using SayHenTai_WebApp.Infrastructure.Service.Repositories;
using System.Data.SqlClient;

namespace SayHenTai_WebApp.Infrastructure.Data
{
    public class TruyenData
    {
        private readonly Connections _connections;
        private readonly GenericRepository<TRUYEN> _truyenRepository;
        private readonly GenericRepository<CHUONG_MUC> _chuongMucRepository;
        private readonly GenericRepository<CHI_TIET_TRUYEN> _chiTietTruyenRepository;

        public TruyenData(IOptions<Connections> options, MongoDbContext context)
        {
            _connections = options.Value;
            _truyenRepository = new GenericRepository<TRUYEN>(context, "TRUYEN");
            _chuongMucRepository = new GenericRepository<CHUONG_MUC>(context, "CHUONG_MUC");
            _chiTietTruyenRepository = new GenericRepository<CHI_TIET_TRUYEN>(context, "CHI_TIET_TRUYEN");
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

        public async Task<IReadOnlyList<TRUYEN>> MgGetAllDanhSachTruyen()
        {
            return await _truyenRepository.GetAllAsync();
        }
        public async Task<IReadOnlyList<CHUONG_MUC>> MgGetChuongMucTruyenByIdTruyen(string Id)
        {
             return await _chuongMucRepository.FindAsync(Builders<CHUONG_MUC>.Filter.Eq("ID_TRUYEN", Id));
        }
        public virtual async Task<TRUYEN> MgGetTruyenByIdAsync(string Id)
        {
            return await _truyenRepository.GetByIdAsync(Id.ToString());
        }
        public virtual async Task<IReadOnlyList<CHI_TIET_TRUYEN>> MgGetDetailTruyenByIdTruyenAndIdChuongMucAsync(string idChuongMuc, string idTruyen)
        {
            return await _chiTietTruyenRepository.FindAsync(Builders<CHI_TIET_TRUYEN>.Filter.Eq("ID_TRUYEN", idTruyen) & Builders<CHI_TIET_TRUYEN>.Filter.Eq("ID_CHUONG_MUC", idChuongMuc));
        }
    }
}
