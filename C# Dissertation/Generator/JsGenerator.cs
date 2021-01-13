using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlexNoddings.Protocols.Core;
using AlexNoddings.Protocols.Generator.Code;
using AlexNoddings.Protocols.Parser.Parsing;

namespace AlexNoddings.Protocols.Generator
{
    public static class JsGenerator
    {
        private const string Pad = "            ";
        internal const string JsNetStackName = "networkStack";

        public static string FromParsedGame(IParsedGame parsedGame) =>
            FromGame(parsedGame.Agents, parsedGame.Attacker, parsedGame.Steps);

        public static string FromGame(IEnumerable<IAgent> agentDefinitions, IAgent attackerDefinition, IEnumerable<IStep> stepDefinitions)
        {
            List<IStep> stepsList = stepDefinitions.ToList();
            List<AgentCode> agents = agentDefinitions.Select(a => new AgentCode(a, a.Name == attackerDefinition.Name)).ToList();
            var attacker = new AgentCode(attackerDefinition);

            var sb = new StringBuilder();
            sb.Append(AgentCode.JsBaseClassDefinition);

            foreach (AgentCode agent in agents)
                sb.Append(agent.JsClassDefinition(stepsList));

            sb.Append($"{Pad}var {JsNetStackName} = new NetStack('message_list1');\n");

            foreach (AgentCode agent in agents)
                sb.Append($"{Pad}var {agent.JsVarName} = {agent.JsInstantiation};\n");

            string agentListStr = "[" + string.Join(", ", agents.Select(a => a.JsVarName)) +"]";
            sb.Append($"{Pad}{JsNetStackName}.registerAgents({agentListStr});\n");
            sb.Append($"{Pad}{JsNetStackName}.registerAttacker({attacker.JsVarName});\n");
            sb.Append($"{Pad}var commands = new Command({JsNetStackName}, {attacker.JsVarName}, {agentListStr});\n");

            foreach (AgentCode agent in agents)
                sb.Append($"{Pad}{agent.JsVarName}.{agent.JsCreateDiv};\n");

            sb.Append($"{Pad}setCurrentStep(0);\n");

            return sb.ToString();
        }
    }
}
