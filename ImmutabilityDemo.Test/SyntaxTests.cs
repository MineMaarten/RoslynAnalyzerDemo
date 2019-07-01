using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ImmutabilityDemo.Test
{
    [TestClass]
    public class SyntaxTests
    {
        [TestMethod]
        public void SyntaxTest()
        {
            var method1 = MethodSyntax.Empty
                .WithMethodName("Method1")
                .WithParameters(new List<MethodParameterSyntax>
                {
                    MethodParameterSyntax.Empty //Usually some parameter declaration
                }.AsReadOnly());

            //Build a class named 'Class1' containing 'Method1'
            var classSyntax = ClassSyntax.Empty
                .WithClassName("Class1")
                .WithMethod(method1);

            //Now add a second method 'Method2'
            var method2 = MethodSyntax.Empty
                .WithMethodName("Method2")
                .WithParameters(new List<MethodParameterSyntax>
                {
                    MethodParameterSyntax.Empty //Usually some parameter declaration
                }.AsReadOnly());

            classSyntax = classSyntax.WithMethod(method2);

            Console.WriteLine("Class name: " + classSyntax.ClassName);
            Console.WriteLine($"Methods ({classSyntax.Methods.Count()} in total) :");
            foreach (var method in classSyntax.Methods)
            {
                Console.WriteLine("Method name: " + method.MethodName);
            }
        }
    }
}
