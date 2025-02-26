using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace SayHenTai_WebApp.Infrastructure.Entities
{
    public class TRUYEN
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { set; get; }
        public string? TEN { set; get; }
        public string? TAC_GIA { set; get; }
        public int SO_LAN_DOC { set; get; }
        public string? TRANG_THAI { set; get; }
        public string? THE_LOAI { set; get; }
        public string? RATING { set; get; }
        public string? IMAGE_COVER { set; get; }
        public string Url => CoreUtility.UrlFromUnicode(TEN);
    }
}
