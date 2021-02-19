using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ReadingIsGood.OrderService.Entities
{
    public interface IDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        ObjectId Id { get; set; }
    }
}