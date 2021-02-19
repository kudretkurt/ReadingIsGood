using MongoDB.Bson;

namespace ReadingIsGood.OrderService.Entities
{
    public abstract class Document : IDocument
    {
        public ObjectId Id { get; set; }
    }
}