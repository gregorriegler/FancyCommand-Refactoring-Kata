using System.Xml;

public class FancyProgram
{
    private readonly IUi _ui;

    public FancyProgram(IUi ui)
    {
        _ui = ui;
    }

    public static void Main(string[] args)
    {
        var evilUi = new EvilUi();
        NewMain(args, evilUi);
    }

    public static void NewMain(string[] args, EvilUi evilUi)
    {
        var program = new FancyProgram(evilUi);
        var mainDom = new XmlDocument();
        if (args.Length > 0)
        {
            program.ExecuteFancyCommand("App.fancy", args, mainDom);
        }
        else
        {
            Console.WriteLine("Executing a standard command");
            if (mainDom == null)
                return;

            var commandName = "App.standard";
            var ndCmd =
                mainDom.SelectSingleNode($"MainMenu/Commands/Command[@id='{commandName}']");
            if (ndCmd == null)
                return;

            var ndToolId = ndCmd.SelectSingleNode("Tool/@idref")?.Value;
            if (string.IsNullOrEmpty(ndToolId))
                return;

            var ndTool = mainDom.SelectSingleNode($"MainMenu/Lib/Tools/Tool[@id='{ndToolId}']");
            if (ndTool == null)
                return;

            program.StartUIWithCommand(ndToolId, ndTool.OuterXml);
        }
    }

    public void ExecuteFancyCommand(string commandName, string[] commandArgs, XmlDocument mainDom)
    {
        Console.WriteLine("Executing a fancy command");
        if (mainDom == null)
            return;

        var ndCmd =
            mainDom.SelectSingleNode($"MainMenu/Commands/Command[@id='{commandName}']");
        if (ndCmd == null)
            return;

        var ndToolId = ndCmd.SelectSingleNode("Tool/@idref")?.Value;
        if (string.IsNullOrEmpty(ndToolId))
            return;

        var ndTool = mainDom.SelectSingleNode($"MainMenu/Lib/Tools/Tool[@id='{ndToolId}']");
        if (ndTool == null)
            return;

        var doStartUi = Foo(commandArgs, ndCmd, ndTool, ndToolId);
        if (doStartUi)
            StartUIWithCommand(ndToolId, ndTool.OuterXml);
    }

    public static bool Foo(string[] commandArgs, XmlNode ndCmd, XmlNode ndTool, string ndToolId)
    {
        var docCmdNd = ndCmd.OwnerDocument;
        var ndPrms = ndTool.SelectSingleNode("Parameters");
        if (ndPrms == null && docCmdNd != null)
        {
            ndPrms = docCmdNd.CreateNode(System.Xml.XmlNodeType.Element, "Parameters", null);
            ndTool.AppendChild(ndPrms);
        }

        var cmdLnPrms = ndPrms?.SelectSingleNode("Parameter[@name='CustomParameters']");
        if (cmdLnPrms == null && docCmdNd != null)
        {
            cmdLnPrms =
                docCmdNd.CreateNode(System.Xml.XmlNodeType.Element, "Parameter", null);
            var nodeAttrName = docCmdNd.CreateAttribute("name");
            nodeAttrName.Value = "CustomParameter";
            cmdLnPrms.Attributes.Append(nodeAttrName);
            nodeAttrName = docCmdNd.CreateAttribute("value");
            nodeAttrName.Value = string.Join(", ", commandArgs); // TODO this is where the Parameters are created
            cmdLnPrms.Attributes.Append(nodeAttrName);
            ndPrms.AppendChild(cmdLnPrms);
        }
        else
        {
            var nodeAttrValue = (cmdLnPrms as System.Xml.XmlElement)?.GetAttributeNode("value");
            if (nodeAttrValue != null)
                nodeAttrValue.Value = string.Join(", ", commandArgs);
        }

        var doStartUi = !String.IsNullOrEmpty(ndToolId) && !String.IsNullOrEmpty(ndTool.InnerXml);
        return doStartUi;
    }

    /*
     * Idea: (Extract) Subclass and Override
     * Idea 2: Peel and Slice (but we found more returns, problematic)
     */
    public void StartUIWithCommand(string toolId, string nodeTool)
    {
        _ui.EvilUIMethod();
    }
}