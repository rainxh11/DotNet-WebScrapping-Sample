using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgresMESRS.Middleware.API.Models
{
    public class RowKeyModel
    {
        [BsonId] 
        public int Index { get; set; }
        public string RowKey { get; set; }
    }
    public class StudentDetailsResponse
    {
        [BsonId]
        public int Index { get; set; }
        public string RowKey { get; set; }
        public string Response { get; set; }
    }
}
