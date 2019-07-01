using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImmutabilityDemo
{
    public sealed class ClassSyntax
    {
        public static readonly ClassSyntax Empty = new ClassSyntax("", ImmutableDictionary<MethodID, MethodSyntax>.Empty);
        public string ClassName { get; }
        private readonly IImmutableDictionary<MethodID, MethodSyntax> methods;
        public IEnumerable<MethodSyntax> Methods => methods.Values;

        private ClassSyntax(string className, IImmutableDictionary<MethodID, MethodSyntax> methods)
        {
            ClassName = className ?? throw new ArgumentNullException(nameof(className));
            this.methods = methods ?? throw new ArgumentNullException(nameof(methods));
        }

        public ClassSyntax WithClassName(string className)
            => new ClassSyntax(className, methods);

        public ClassSyntax WithMethod(MethodSyntax method)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            var newMethods = this.methods.Add(method.MethodID, method);
            return new ClassSyntax(ClassName, newMethods);
        }

        public ClassSyntax ExceptMethod(MethodSyntax method)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            var newMethods = this.methods.Remove(method.MethodID);
            return new ClassSyntax(ClassName, newMethods);
        }
    }
}
