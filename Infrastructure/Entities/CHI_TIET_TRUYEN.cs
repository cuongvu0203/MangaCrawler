using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace SayHenTai_WebApp.Infrastructure.Entities
{
    public class CHI_TIET_TRUYEN
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { set; get; }
        public string? ID_CHUONG_MUC { set; get; }
        public string? ID_TRUYEN { set; get; }
        public string? IMAGE_URL { set; get; }
        public string? WIDTH { set; get; }
        public string? HEIGHT { set; get; }
        public string? ALT { set; get; }
        public string? ID_SOURCE { set; get; }
    }
}
