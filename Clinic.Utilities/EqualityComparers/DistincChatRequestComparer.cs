using System;
using System.Collections.Generic;
using System.Text;
using Clinic.Models.DomainClasses.Others;

namespace Clinic.Utilities.EqualityComparers
{
    public class DistincChatRequestComparer : IEqualityComparer<ChatRequest>
    {
        public bool Equals(ChatRequest x, ChatRequest y)
        {
            return y != null && (x != null && x.PatientUsername.Equals(y.PatientUsername));
        }

        public int GetHashCode(ChatRequest obj)
        {
            return obj.PatientUsername.GetHashCode();

        }
    }
}
