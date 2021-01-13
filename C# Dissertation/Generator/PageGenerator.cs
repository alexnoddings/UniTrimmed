using System.Collections.Generic;
using System.IO;
using System.Text;
using AlexNoddings.Protocols.Core;
using AlexNoddings.Protocols.Parser.Parsing;

namespace AlexNoddings.Protocols.Generator
{
    public static class PageGenerator
    {
        public static string FromParsedGame(IParsedGame parsedGame) =>
            FromGame(parsedGame.Agents, parsedGame.Attacker, parsedGame.Steps);

        public static string FromGame(IReadOnlyCollection<IAgent> agentDefinitions, IAgent attackerDefinition, IReadOnlyCollection<IStep> stepDefinitions)
        {
            var script = JsGenerator.FromGame(agentDefinitions, attackerDefinition, stepDefinitions);
            var template = File.ReadAllText("./page-template.html");

            StringBuilder sb = new StringBuilder();
            sb.Append("<div>");
            sb.Append("<h6>Knowledge:</h6>");
            foreach (IAgent a in agentDefinitions)
            {
                sb.Append("<div>");
                sb.Append(a.Name);
                sb.Append(": [");
                sb.Append(string.Join(", ", a.Knowledge));
                sb.Append("]");
                sb.Append("</div>");
            }
            sb.Append("</div>");

            sb.Append("<div>");
            sb.Append("<h6>Steps:</h6>");
            sb.Append("<ol>");
            foreach (IStep s in stepDefinitions)
            {
                sb.Append("<li>");
                sb.Append(s.FromAgent.Name);
                sb.Append(" -> ");
                sb.Append(s.ToAgent.Name);
                sb.Append(": ");
                sb.Append(s.Knowledge);
                sb.Append("</li>");
            }
            sb.Append("</ol>");
            sb.Append("</div>");

            return template.Replace("%code%", script).Replace("%protodef%", sb.ToString());
        }
    }
}
