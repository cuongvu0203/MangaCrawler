using Dapper;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SayHenTai_WebApp.Infrastructure.Entities;
using SayHenTai_WebApp.Infrastructure.MongoDB;
using SayHenTai_WebApp.Infrastructure.Service.Repositories;
using SayHenTai_WebApp.Models;
using System.Data.SqlClient;
using System.Drawing;
using System.Security.Policy;
using ZstdSharp.Unsafe;

namespace SayHenTai_WebApp.Infrastructure.Data
{
    public class LeechTruyenData
    {

        private readonly Connections _connections;
        private readonly GenericRepository<TRUYEN> _truyenRepository;
        private readonly GenericRepository<CHUONG_MUC> _chuongMucRepository;
        private readonly GenericRepository<CHI_TIET_TRUYEN> _chiTietTruyenRepository;
        public LeechTruyenData(IOptions<Connections> options, MongoDbContext context)
        {
            _connections = options.Value;
            _truyenRepository = new GenericRepository<TRUYEN>(context, "TRUYEN");
            _chuongMucRepository = new GenericRepository<CHUONG_MUC>(context, "CHUONG_MUC");
            _chiTietTruyenRepository = new GenericRepository<CHI_TIET_TRUYEN>(context, "CHI_TIET_TRUYEN");
        }

        public async Task<long> InsertThongTinTruyenAsync(string tenTruyen, string urlImgCover)
        {
            string query = @"INSERT INTO [dbo].[TRUYEN]
                               ([TEN]
                               ,[TAC_GIA]
                               ,[SO_LAN_DOC]
                               ,[TRANG_THAI]
                               ,[THE_LOAI]
                               ,[RATING]
                               ,[IMAGE_COVER])
                        OUTPUT INSERTED.ID
                         VALUES
                               (@TEN
                               ,@TAC_GIA
                               ,@SO_LAN_DOC
                               ,@TRANG_THAI
                               ,@THE_LOAI
                               ,@RATING
                               ,@IMAGE_COVER)";
            using SqlConnection connection = new SqlConnection(_connections.SqlDefaultConnectionWrite);
            return await connection.QuerySingleOrDefaultAsync<long>(query, new
            {
                TEN = tenTruyen,
                TAC_GIA = "Đang cập nhật",
                SO_LAN_DOC = 2345,
                TRANG_THAI = "Đang cập nhật",
                THE_LOAI = "Manhwa 18+",
                RATING = "4",
                IMAGE_COVER = urlImgCover,
            });
        }

        public bool InsertListChuongMuc(List<CHUONG_MUC> objects)
        {
            using (var connection = new SqlConnection(_connections.SqlDefaultConnectionWrite))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Use Dapper to insert the list into the database table
                        connection.Execute(@"INSERT INTO [dbo].[CHUONG_MUC]
                                               ([ID_TRUYEN]
                                               ,[TEN]
                                               ,[SO_LAN_DOC]
                                               ,[THOI_GIAN_CAP_NHAT]
                                               ,[URL])
                                         VALUES
                                               (@ID_TRUYEN
                                               ,@TEN
                                               ,@SO_LAN_DOC
                                               ,@THOI_GIAN_CAP_NHAT
                                               ,@URL)", objects, transaction: transaction);

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        public bool InsertListChiTietTruyen(List<CHI_TIET_TRUYEN> objects)
        {
            using (var connection = new SqlConnection(_connections.SqlDefaultConnectionWrite))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Use Dapper to insert the list into the database table
                        connection.Execute(@"INSERT INTO [dbo].[CHI_TIET_TRUYEN]
                                               ([ID_CHUONG_MUC]
                                               ,[ID_TRUYEN]
                                               ,[IMAGE_URL]
                                               ,[WIDTH]
                                               ,[HEIGHT]
                                               ,[ALT]
                                               ,[ID_SOURCE])
                                         VALUES
                                               (@ID_CHUONG_MUC
                                               ,@ID_TRUYEN
                                               ,@IMAGE_URL
                                               ,@WIDTH
                                               ,@HEIGHT
                                               ,@ALT
                                               ,@ID_SOURCE)", objects, transaction: transaction);

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }
        public async Task<IReadOnlyList<CHUONG_MUC>> GetDanhSachChuongMucTruyenByIdTruyen(long IdTruyen)
        {
            var query = "SELECT * FROM CHUONG_MUC WHERE ID_TRUYEN=@IdTruyen ";
            using SqlConnection connection = new SqlConnection(_connections.SqlDefaultConnection);
            var queryData = (await connection.QueryAsync<CHUONG_MUC>(query, new { IdTruyen = IdTruyen })).AsList();
            return queryData;
        }

        public async Task<TRUYEN> MgInsertThongTinTruyenAsync(string tenTruyen, string urlImgCover)
        {
            var truyen = new TRUYEN
            {
                TEN = tenTruyen,
                TAC_GIA = "Đang cập nhật",
                SO_LAN_DOC = 2345,
                TRANG_THAI = "Đang cập nhật",
                THE_LOAI = "Manhwa 18+",
                RATING = "4",
                IMAGE_COVER = urlImgCover,
            };
            await _truyenRepository.CreateAsync(truyen);
            return truyen;
        }
        public async Task<bool> MgInsertListChuongMucAsync(List<CHUONG_MUC> objects)
        {
            if (objects == null || objects.Count == 0)
            {
                return false; // Tránh chèn danh sách rỗng
            }

            await _chuongMucRepository.InsertManyAsync(objects);
            return true; // Trả về true nếu chèn thành công
        }
        public async Task<bool> MgInsertListChiTietTruyenAsync(List<CHI_TIET_TRUYEN> objects)
        {
            if (objects == null || objects.Count == 0)
            {
                return false; // Tránh chèn danh sách rỗng
            }

            await _chiTietTruyenRepository.InsertManyAsync(objects);
            return true; // Trả về true nếu chèn thành công
        }
        public async Task<IReadOnlyList<CHUONG_MUC>> MgGetDanhSachChuongMucTruyenByIdTruyen(string IdTruyen)
        {
            return await _chuongMucRepository.FindAsync(Builders<CHUONG_MUC>.Filter.Eq("ID_TRUYEN", IdTruyen));
        }

    }
}
