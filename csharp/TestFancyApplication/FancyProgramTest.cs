using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Xunit;
using VerifyTests;
using VerifyXunit;
using Xunit;

namespace TestFancyProgram;

public class FancyProgramTest
{
    [Fact]
    public Task ExecuteFancyCommand()
    {
        // sample test data and custom parameters you could use
        var xmlInput = GetTestData("Sample.xml");
        var customParams = new string[] { "//ID: 71mUJgN0sKbr", "//Customer: Acme" };

        var ui = new MockUi();
        var program = new FancyProgram(ui);
        var mainDom = new XmlDocument();
        mainDom.LoadXml(xmlInput);

        program.ExecuteFancyCommand("App.fancy", customParams, mainDom);

        var stringWriter = new StringWriter();
        var xmlWriter = new XmlTextWriter(stringWriter);
        mainDom.WriteTo(xmlWriter);
        stringWriter.Flush();
        var s = stringWriter.ToString();
        return Verifier.Verify(s);
        // TODO: assert something
    }

    private string GetTestData(string filename)
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestData", filename);
        return File.ReadAllText(path);
    }

    public class MockUi : IUi
    {
        public int CalledTimes;
        public void EvilUIMethod()
        {
            CalledTimes++;
        }
    }
}