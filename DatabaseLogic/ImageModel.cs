using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;

namespace Infrasctructure.Models
{
    public class ImageModel
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public float[,] Descriptor { get; set; }
        public bool[] ImageHash { get; set; }
        public string ImagePath { get; set; }
    }
}
