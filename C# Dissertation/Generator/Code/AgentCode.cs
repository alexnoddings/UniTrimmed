using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using AlexNoddings.Protocols.Core;

namespace AlexNoddings.Protocols.Generator.Code
{
    internal class AgentCode
    {
        // Spacing
        private const string ___ = "            ";
        private const string ______ = "                ";
        private const string _________ = "                    ";
        private const string ____________ = "                        ";
        private const string _______________ = "                            ";
        private const string __________________ = "                                ";
        private const string _____________________ = "                                    ";

        private IAgent Agent { get; }
        private bool Attacker { get; }

        internal AgentCode(IAgent agent, bool attacker = false) => (Agent, Attacker) = (agent ?? throw new ArgumentNullException(nameof(agent)), attacker);

        internal string JsVarName => $"agent{Agent.Name}";

        internal string JsClassName => $"g_agent{Agent.Name}";

        internal static readonly string JsBaseClassDefinition =
            $"{___}class g_Agent extends Agent {{                                                                                                        \n" +
            $"{______}constructor(id, facts, network) {{                                                                                                 \n" +
            $"{_________}super(id, facts, network);                                                                                                      \n" +
            $"{_________}this.session = 0;                                                                                                               \n" +
            $"{_________}this.state = 0;                                                                                                                 \n" +
            $"{_________}this.key = 0;                                                                                                                   \n" +
            $"{______}}}                                                                                                                                 \n" +
            $"{______}getNonce(fact, step) {{                                                                                                            \n" +
            $"{_________}var nonce = fact + '_' + this.session;                                                                                          \n" +
            $"{_________}this.learns(nonce, step);                                                                                                       \n" +
            $"{_________}return nonce;                                                                                                                   \n" +
            $"{______}}}                                                                                                                                 \n" +
            $"{______}process(msg, step) {{                                                                                                              \n" +
            $"{_________}var handler = this['g_state' + this.state] ?? this.g_stateUnhandled;                                                            \n" +
            $"{_________}handler.apply(this, [msg, step]);                                                                                               \n" +
            $"{_________}this.state++;                                                                                                                   \n" +
            $"{______}}}                                                                                                                                 \n" +
            $"{______}g_stateUnhandled(msg, step) {{                                                                                                     \n" +
            $"{_________}throw 'Unhandled state ' + this.state + ' for agent ' + this.id;                                                                \n" +
            $"{______}}}                                                                                                                                 \n" +
            $"{______}createKnowledge(knowledge, step) {{                                                                                                \n" +
            $"{_________}var index = knowledge.indexOf('#');                                                                                             \n" +
            $"{_________}if (index == -1) return knowledge;                                                                                              \n" +
            $"{_________}var name = '';                                                                                                                  \n" +
            $"{_________}var i = index + 1;                                                                                                              \n" +
            $"{_________}while (i < knowledge.length) {{                                                                                                 \n" +
            $"{____________}var c = knowledge[i++];                                                                                                      \n" +
            $"{____________}if (!c.match(/[a-z]/i)) break;                                                                                               \n" +
            $"{____________}name = name + c;                                                                                                             \n" +
            $"{_________}}}                                                                                                                              \n" +
            $"{_________}this.learns(name + '_' + this.session, step);                                                                                   \n" +
            $"{_________}return this.createKnowledge(knowledge.replace('#' + name, name + '_' + this.session));                                          \n" +
            $"{______}}}                                                                                                                                 \n" +
            $"{_________}getMissingKnowledge(knowledge) {{                                                                                               \n" +
            $"{_________}knowledge = knowledge.trim();                                                                                                   \n" +
            $"{_________}if (this.doesKnow(knowledge)) return null;                                                                                      \n" +
            $"{_________}var keyStart = knowledge.lastIndexOf('}}') + 1;                                                                                 \n" +
            $"{_________}var key = knowledge.substr(keyStart);                                                                                           \n" +
            $"{_________}if (key == '') {{ // not encrypted                                                                                              \n" +
            $"{____________}var parts = [];                                                                                                              \n" +
            $"{____________}var last = 1;                                                                                                                \n" +
            $"{____________}var depth = 1;                                                                                                               \n" +
            $"{____________}for (var i = 1; i < knowledge.length - 1; i++) {{                                                                            \n" +
            $"{_______________}var c = knowledge[i];                                                                                                     \n" +
            $"{_______________}if (c == '{{')                                                                                                            \n" +
            $"{__________________}depth++;                                                                                                               \n" +
            $"{_______________}else if (c == '}}')                                                                                                       \n" +
            $"{__________________}depth--;                                                                                                               \n" +
            $"{_______________}else if (c == ',' && depth < 2) {{                                                                                        \n" +
            $"{__________________}parts.push(knowledge.substr(last, i - last).replace(/^[\\s,]*/, ''));                                                  \n" +
            $"{__________________}last = i;                                                                                                              \n" +
            $"{_______________}}}                                                                                                                        \n" +
            $"{____________}}}                                                                                                                           \n" +
            $"{____________}parts.push(knowledge.substr(last, knowledge.length - last - 1).replace(/^[\\s,]*/, ''));                                     \n" +
            $"{____________}for (var p = 0; p<parts.length; p++) {{                                                                                      \n" +
            $"{_______________}var part = parts[p];                                                                                                      \n" +
            $"{_______________}if (!this.doesKnow(part)) {{                                                                                              \n" +
            $"{__________________}if (!part.startsWith('{{'))                                                                                            \n" +
            $"{_____________________}part = '{{' + part + '}}';                                                                                          \n" +
            $"{__________________}var missing = this.getMissingKnowledge(part);                                                                          \n" +
            $"{__________________}if (missing != null)                                                                                                   \n" +
            $"{_____________________}return missing;                                                                                                     \n" +
            $"{_______________}}}                                                                                                                        \n" +
            $"{____________}}}                                                                                                                           \n" +
            $"{____________}return null;                                                                                                                 \n" +
            $"{_________}}}                                                                                                                              \n" +
            $"{_________}if (!this.doesKnow(key)) {{                                                                                                     \n" +
            $"{_________}    return knowledge;                                                                                                           \n" +
            $"{_________}}}                                                                                                                              \n" +
            $"{_________}return this.getMissingKnowledge(knowledge.substr(0, keyStart));                                                                 \n" +
            $"{_________}}}                                                                                                                              \n" +
            $"{______}makeKnowledge(knowledge, step) {{                                                                                                  \n" +
            $"{_________}var k = this.createKnowledge(knowledge, step);                                                                                  \n" +
            $"{_________}return k;                                                                                                                       \n" +
            $"{______}}}                                                                                                                                 \n" +
            $"{______}sendMessage(msg, step) {{                                                                                                          \n" +
            $"{_________}var missingKnowledge = this.getMissingKnowledge(msg.content);                                                                   \n" +
            $"{_________}if (missingKnowledge != null) {{                                                                                                \n" +
            $"{____________}throw 'Agent ' + this.id + ': tried to send \"' + msg.content + '\" without knowing it (missing ' + missingKnowledge + ')';  \n" +
            $"{_________}}}                                                                                                                              \n" +
            $"{_________}this.network.addMessage(msg, step, 'w');                                                                                        \n" +
            $"{______}}}                                                                                                                                 \n" +
            $"{______}stripKnowledge(knowledge) {{                                                                                                       \n" +
            $"{_________}if (!knowledge.startsWith('{{')) return knowledge; // can't be further stripped                                                 \n" +
            $"{_________}var keyStart = knowledge.lastIndexOf('}}') + 1;                                                                                 \n" +
            $"{_________}var key = knowledge.substr(keyStart);                                                                                           \n" +
            $"{_________}if (this.doesKnow(key)) {{                                                                                                      \n" +
            $"{____________}return this.stripKnowledge(knowledge.substr(1, keyStart - 2));                                                               \n" +
            $"{_________}}}                                                                                                                              \n" +
            $"{_________}return knowledge;                                                                                                               \n" +
            $"{______}}}                                                                                                                                 \n" +
            $"{______}learns(knowledge, step) {{                                                                                                         \n" +
            $"{_________}var sk = this.stripKnowledge(knowledge);                                                                                        \n" +
            $"{_________}var skf = sk.getFacts();                                                                                                        \n" +
            $"{_________}for (var i = 0; i < skf.length; i++) {{                                                                                         \n" +
            $"{____________}var f = skf[i];                                                                                                              \n" +
            $"{____________}if (!this.doesKnow(f)) {{                                                                                                    \n" +
            $"{_______________}this.facts.push(new Fact(f, step));                                                                                       \n" +
            $"{____________}}}                                                                                                                           \n" +
            $"{_________}}}                                                                                                                              \n" +
            $"{______}}}                                                                                                                                 \n" +
            $"{______}doesKnow(knowledge) {{                                                                                                             \n" +
            $"{_________}return this.knows(knowledge.trim()) != null;                                                                                    \n" +
            $"{______}}}                                                                                                                                 \n" +
            $"{___}}}                                                                                                                                    \n";

        internal string JsClassDefinition(IList<IStep> steps) => $"{___}class {JsClassName} extends g_Agent {{  \n" +
                                                                 $"{______}constructor(id, facts, network) {{   \n" +
                                                                 $"{_________}super(id, facts, network);        \n" +
                                                                 $"{______}}}                                   \n" +
                                                                 $"{JsStateHandlers(steps).TrimEnd('\n')}       \n" +
                                                                 $"{___}}}                                      \n" +
                                                                 $"                                             \n";

        internal string JsInstantiation => $"new {JsClassName}('{Agent.Name}', [{string.Join(",", Agent.Knowledge.Select(k => $"'{k}'").Where(k => k != "'{}'"))}], {JsGenerator.JsNetStackName})";

        internal string DisplayStr => $"{Agent.Name}: [{string.Join(", ", Agent.Knowledge)}]";

        internal string JsCreateDiv => Attacker ? "createDiv('adversary_box1', 'agent_top_right')" : "createDiv('agents_box1', 'agent')";

        internal string JsStateHandlers(IList<IStep> steps)
        {
            var sb = new StringBuilder();

            if (Attacker)
            {
                sb.Append($"{______}g_state0(receivingMsg, step) {{              \n" +
                          $"{_________}this.learns(receivingMsg.content, step);  \n" +
                          $"{_________}this.state = -1;                          \n" +
                          $"{______}}}                                           \n");
            }
            else
            {
                IStep firstStep = steps.First();
                var isInitialisingAgent = false;
                if (firstStep.FromAgent.Name == Agent.Name)
                {
                    isInitialisingAgent = true;
                    sb.Append($"{______}init (msg, step) {{           \n" +
                              $"{_________}this.g_state0(msg, step);  \n" +
                              $"{_________}this.state++;              \n" +
                              $"{______}}}                            \n");
                }

                var state = 0;
                foreach (IStep step in steps)
                {
                    if (step.FromAgent.Name != Agent.Name) continue;

                    sb.Append($"{______}g_state{state}(receivingMsg, step) {{\n");
                    if (!(isInitialisingAgent && state == 0))
                        sb.Append($"{_________}this.learns(receivingMsg.content, step);\n");

                    sb.Append(
                        $"{_________}var newMsg = {{ source: this.id, destination: '{step.ToAgent.Name}', content: this.makeKnowledge('{step.Knowledge}', step) }};\n");
                    sb.Append($"{_________}this.sendMessage(newMsg, step);\n");

                    sb.Append($"{______}}}\n");
                    state++;
                }

                IStep lastStep = steps.Last();
                if (lastStep.ToAgent.Name == Agent.Name)
                    sb.Append($"{______}g_state{state}(receivingMsg, step) {{         \n" +
                              $"{_________}this.learns(receivingMsg.content, step);   \n" +
                              $"{______}}}                                            \n");
            }

            return sb.ToString();
        }
    }
}
