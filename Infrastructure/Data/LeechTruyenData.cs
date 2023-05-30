using Dapper;
using Microsoft.Extensions.Options;
using SayHenTai_WebApp.Infrastructure.Entities;
using SayHenTai_WebApp.Models;
using System.Data.SqlClient;
using System.Drawing;
using System.Security.Policy;

namespace SayHenTai_WebApp.Infrastructure.Data
{
    public class LeechTruyenData
    {

        private readonly Connections _connections;
        public LeechTruyenData(IOptions<Connections> options)
        {
            _connections = options.Value;
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
    }
}
