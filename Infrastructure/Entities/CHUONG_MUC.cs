using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace SayHenTai_WebApp.Infrastructure.Entities
{
    public class CHUONG_MUC
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string ID_TRUYEN { get; set; }
        public string? TEN { get; set; }
        public int SO_LAN_DOC { get; set; }
        public string? THOI_GIAN_CAP_NHAT { get; set; }
        public string? URL { get; set; }
    }
}
