using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImmutabilityDemo
{
    /// <summary>
    /// Both the method name and parameter list determine if the method is unique
    /// </summary>
    public sealed class MethodID
    {
        public string MethodName { get; }
        public IReadOnlyList<MethodParameterSyntax> Parameters { get; }

        public MethodID(string methodName, IReadOnlyList<MethodParameterSyntax> parameters)
        {
            MethodName = methodName ?? throw new ArgumentNullException(nameof(methodName));
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        }

        private bool Equals(MethodID other)
        {
            return string.Equals(MethodName, other.MethodName) && Parameters.Equals(other.Parameters);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is MethodID other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (MethodName.GetHashCode() * 397) ^ Parameters.GetHashCode();
            }
        }
    }
}
