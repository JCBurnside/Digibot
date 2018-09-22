namespace DigiBotExtension.Analyzer.Tests
{
    using System;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Xunit;

    public class Digi001
    {
        private static readonly DIGI001IrcConnectShouldImplementIIrcConnection Analyzer = new DIGI001IrcConnectShouldImplementIIrcConnection();
        private static readonly IrcConnectShouldImplementIIrcConnectionCodeFixProvider CodeFix = new IrcConnectShouldImplementIIrcConnectionCodeFixProvider();

        [Fact]
        [Trait("Case", "NotFound")]
        public void Digi001NotFound()
        {
            var testCode = @"
namespace RosylnSandbox
{
    class MuhClass {}
}";
            AnalyzerAssert.AddTransitiveMetadataReferences(typeof(DIGI001IrcConnectShouldImplementIIrcConnection).Assembly);
            AnalyzerAssert.Valid(Analyzer, testCode);
            AnalyzerAssert.ResetMetadataReferences();
        }

        [Fact]
        [Trait("Case", "Valid")]
        public void Digi001Valid()
        {
            var testCode = @"
namespace RosylnSandbox
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Linq;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using System.Threading;
    using DigiBotExtension;
    using Avalonia.Controls;
    [IrcConnection("""")]
    public class ValidClass : IIrcConnection
    {
        public int RetryLimit { get; set; }
        public Action<StreamWriter,IEnumerable<string>> Pong { get; set; }
        public Action<StreamWriter> Logon { get; set; }
        public Avalonia.Controls.Control ConfigBox { get; }
        public event MessageReceived OnMessageReceived;
        public event ConnectedEvent OnConnected;
        public event DisconnectEvent OnDisconnect;
        public string Channel { get; set; }
        public bool Connected { get; }
    
        public Task Start(){ return Task.CompletedTask; }
        public Task Stop(){ return Task.CompletedTask; }
        public void WriteLine(object o){}
        public void WriteLine(string format, params object[] o){}

        public void Dispose(){}
    }
}";
            AnalyzerAssert.AddTransitiveMetadataReferences(typeof(DigiBotExtension.IIrcConnection).Assembly);
            AnalyzerAssert.AddTransitiveMetadataReferences(typeof(Avalonia.Controls.Control).Assembly);
            AnalyzerAssert.AddTransitiveMetadataReferences(typeof(DIGI001IrcConnectShouldImplementIIrcConnection).Assembly);
            AnalyzerAssert.Valid(Analyzer, testCode);
            AnalyzerAssert.ResetMetadataReferences();
        }

        [Fact]
        [Trait("Case", "Found")]
        public void Digi001Found()
        {
            var testCode = @"
namespace RoslynSandBox
{
    using DigiBotExtension;
    [IrcConnection("""")]
    public class ↓MuhConnection
    {
    }
}";
            AnalyzerAssert.AddTransitiveMetadataReferences(typeof(DigiBotExtension.IIrcConnection).Assembly);
            AnalyzerAssert.AddTransitiveMetadataReferences(typeof(Avalonia.Controls.Control).Assembly);
            AnalyzerAssert.AddTransitiveMetadataReferences(typeof(DIGI001IrcConnectShouldImplementIIrcConnection).Assembly);
            var expectedMessage = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated(
                "DIGI001",
                "The class MuhConnection should impliment: IIrcConnection to use IrcConnectionAttribute.",
                testCode,
                out testCode);
            AnalyzerAssert.Diagnostics<DIGI001IrcConnectShouldImplementIIrcConnection>(expectedMessage, testCode);
            AnalyzerAssert.ResetMetadataReferences();
        }

        [Fact]
        [Trait("Case", "CodeFix")]
        public void Digi001CodeFix()
        {
            var testCode = @"
namespace RoslynSandBox
{
    using DigiBotExtensions;
    [IrcConnection("""")]
    public class ↓MuhConnection
    {
    }
}";
            var fixedCode = @"
namespace RoslynSandBox
{
    using DigiBotExtensions;
    [IrcConnection("""")]
    public class MuhConnection : IIrcConnection
    {
    }
}";
            AnalyzerAssert.AddTransitiveMetadataReferences(typeof(DigiBotExtension.IIrcConnection).Assembly);
            AnalyzerAssert.AddTransitiveMetadataReferences(typeof(Avalonia.Controls.Control).Assembly);
            AnalyzerAssert.AddTransitiveMetadataReferences(typeof(DIGI001IrcConnectShouldImplementIIrcConnection).Assembly);
            AnalyzerAssert.CodeFix(Analyzer, CodeFix, testCode, fixedCode, allowCompilationErrors: AllowCompilationErrors.No);
            AnalyzerAssert.ResetMetadataReferences();
        }
    }
}
