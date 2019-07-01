using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ImmutabilityDemo
{
    public sealed class MethodSyntax
    {
        public static readonly MethodSyntax Empty = new MethodSyntax("", ImmutableList<MethodParameterSyntax>.Empty, BodySyntax.Empty);
        public MethodID MethodID { get; }
        public string MethodName { get; }
        public IReadOnlyList<MethodParameterSyntax> Parameters { get; }
        public BodySyntax MethodBody { get; }
        //public string ReturnType { get; }

        private MethodSyntax(string methodName, IReadOnlyList<MethodParameterSyntax> parameters, BodySyntax methodBody)
        {
            MethodName = methodName ?? throw new ArgumentNullException(nameof(methodName));
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            MethodBody = methodBody ?? throw new ArgumentNullException(nameof(methodBody));
            MethodID = new MethodID(MethodName, Parameters);
        }

        public MethodSyntax WithMethodName(string methodName)
            => new MethodSyntax(methodName, Parameters, MethodBody);

        public MethodSyntax WithParameters(IReadOnlyList<MethodParameterSyntax> parameters)
            => new MethodSyntax(MethodName, parameters, MethodBody);

        public MethodSyntax WithBody(BodySyntax methodBody)
            => new MethodSyntax(MethodName, Parameters, methodBody);
    }
}
