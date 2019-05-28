using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;
using DemoAnalyzer;

namespace DemoAnalyzer.Test
{
    [TestClass]
    public class UnitTest : CodeFixVerifier
    {

        //No diagnostics expected to show up
        [TestMethod]
        public void TestMethod1()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public void TestMethod2()
        {
            var test = @"
            using System;
            using System.Linq;
            using System.Collections.Generic;

            namespace FixingProject
            {
                public class Foo<Bar> where Bar : class
                {
                    public Bar CurrentBar { get; }

                    public Foo(Bar bar) 
                        => CurrentBar = bar ?? throw new ArgumentNullException(nameof(bar));

                    private class SomeNestedClass
                    {
                        public int Bar { get; set; }
                    }
                }
            }";
            var expected = new DiagnosticResult
            {
                Id = "AV1709",
                Message = $"Generic type name 'Bar' does not begin with the letter 'T'.",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 8, 34)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
            using System;
            using System.Linq;
            using System.Collections.Generic;

            namespace FixingProject
            {
                public class Foo<TBar> where TBar : class
                {
                    public TBar CurrentBar { get; }

                    public Foo(TBar bar) 
                        => CurrentBar = bar ?? throw new ArgumentNullException(nameof(bar));

                    private class SomeNestedClass
                    {
                        public int Bar { get; set; }
                    }
                }
            }";
            VerifyCSharpFix(test, fixtest);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new DemoAnalyzerCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new DemoAnalyzerAnalyzer();
        }
    }
}
