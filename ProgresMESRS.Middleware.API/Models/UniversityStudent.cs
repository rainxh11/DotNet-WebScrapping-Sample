using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invio.Hashing;

namespace ProgresMESRS.Middleware.API.Models
{
    public class UniversityStudent : IEquatable<UniversityStudent>
    {
        [BsonId]
        public int Index { get; set; }
        public string RowKey { get; set; }
        public string Matricule { get; set; }
        public string NIN { get; set; }
        public string FamilyName { get; set; }
        public string FirstName { get; set; }
        public string DateOfBirth { get; set; }
        
        public bool Equals(UniversityStudent other)
        {
            //return this.Index == other.Index;
            return other.NIN == NIN && other.Matricule == Matricule && DateOfBirth == other.DateOfBirth;
        }
        public override int GetHashCode()
        {
            return Invio.Hashing.HashCode.From(NIN, Matricule, DateOfBirth);
        }
    }
}
