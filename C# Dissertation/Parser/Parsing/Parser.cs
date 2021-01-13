using System;
using System.Collections.Generic;
using System.Linq;
using AlexNoddings.Protocols.Core;
using AlexNoddings.Protocols.Parser.Core;
using AlexNoddings.Protocols.Parser.Extensions;
using AlexNoddings.Protocols.Parser.Tokenisation;

namespace AlexNoddings.Protocols.Parser.Parsing
{
    public class Parser
    {
        private Stream<Token> Tokens { get; }

        private Parser(string text) => Tokens = Tokeniser.Tokenise(text).Where(t => t.Type.IsImportant).AsStream();

        public static IParsedGame Parse(string text) => new Parser(text ?? throw new ArgumentNullException(nameof(text))).Parse();

        private IParsedGame Parse()
        {
            (IList<ParsedAgent> parsedAgents, IList<ParsedStep> parsedSteps) = ParseGame();

            if (parsedAgents.Count < 2)
                throw new InvalidOperationException($"Must have at least 2 agents");

            var agentNames = parsedAgents.Select(a => a.Name).ToList();
            var duplicateAgentName = agentNames.FirstOrDefault(an => agentNames.Count(n => an == n) > 1);
            if (duplicateAgentName != null)
                throw new InvalidOperationException($"Agent {duplicateAgentName} was defined more than once");

            int attackerCount = parsedAgents.Count(a => a.IsAttacker);
            if (attackerCount == 0)
                throw new InvalidOperationException($"No agent designated as the attacker");
            if (attackerCount > 1)
                throw new InvalidOperationException($"Only one attacker is permitted");
            ParsedAgent attacker = parsedAgents.First(a => a.IsAttacker);
            
            var steps = new List<IStep>();
            foreach (ParsedStep parsedStep in parsedSteps)
            {
                IAgent fromAgent = parsedAgents.FirstOrDefault(a => a.Name == parsedStep.FromAgentName);
                IAgent toAgent = parsedAgents.FirstOrDefault(a => a.Name == parsedStep.ToAgentName);
                if (fromAgent == toAgent)
                    throw new InvalidOperationException("Cannot talk to self");
                steps.Add(new GameStep(fromAgent, toAgent, parsedStep.Knowledge));
            }

            return new ParsedGame(parsedAgents.Cast<IAgent>(), attacker, steps);
        }

        private (IList<ParsedAgent>, IList<ParsedStep>) ParseGame()
        {
            var agents = new List<ParsedAgent>();
            var steps = new List<ParsedStep>();

            while (!Tokens.Peek.Equals(default(Token)))
            {
                Token token = Tokens.Next();
                if (token.Type == ParseDefinitions.BaseStateOpenAgentsBlock)
                {
                    agents.AddRange(ParseAgentsBlock());
                }
                else if (token.Type == ParseDefinitions.BaseStateOpenStepsBlock)
                {
                    steps.AddRange(ParseStepsBlock());
                }
                else
                {
                    throw new InvalidOperationException($"Unexpected token \"{token.Value}\" of type {token.Type.Name}");
                }
            }

            return (agents, steps);
        }

        private List<ParsedAgent> ParseAgentsBlock()
        {
            var agents = new List<ParsedAgent>();
            while (NotReached(ParseDefinitions.AgentsBlockStatePopToken))
                agents.Add(ParseAgent());
            // Remove the AgentsBlockStatePopToken
            Tokens.MoveNext();
            return agents;
        }

        private ParsedAgent ParseAgent()
        {
            Token nameToken = AssertNextToken(ParseDefinitions.AgentNameToken);
            var isAttacker = false;
            if (Tokens.Peek.Type == ParseDefinitions.AgentsBlockStateAttackerToken)
            {
                isAttacker = true;
                // Remove the AgentsBlockStateAttackerToken
                Tokens.MoveNext();
            }

            IKnowledge knowledge = ParseKnowledge();
            if (knowledge.Key == null && knowledge.Parts.Count > 1)
                // If the knowledge is un-encrypted and contains multiple parts, expand it
                return new ParsedAgent(nameToken.Value, knowledge.Parts, isAttacker);
            return new ParsedAgent(nameToken.Value, knowledge, isAttacker);
        }

        private List<ParsedStep> ParseStepsBlock()
        {
            var steps = new List<ParsedStep>();
            while (NotReached(ParseDefinitions.StepsBlockStatePopToken))
                steps.Add(ParseStep());
            // Remove the StepsBlockStatePopToken
            Tokens.MoveNext();
            return steps;
        }

        private ParsedStep ParseStep()
        {
            Token fromToken = AssertNextToken(ParseDefinitions.AgentNameToken);
            Token toToken = AssertNextToken(ParseDefinitions.AgentNameToken);
            IKnowledge knowledge = ParseKnowledge();
            return new ParsedStep(fromToken.Value, toToken.Value, knowledge);
        }

        private IKnowledge ParseKnowledge()
        {
            AssertNextToken(ParseDefinitions.KnowledgeOpenToken);
            var subKnowledge = new List<IKnowledge>();
            while (true)
            {
                if (Tokens.Peek.Type == ParseDefinitions.KnowledgeOpenToken)
                {
                    subKnowledge.Add(ParseKnowledge());
                }
                else
                {
                    Token next = Tokens.Next();
                    if (next.Type == ParseDefinitions.KnowledgeStateNameToken)
                    {
                        subKnowledge.Add(new ParsedFact(next.Value));
                    }
                    else if (next.Type == ParseDefinitions.KnowledgeStatePopToken)
                    {
                        if (Tokens.Peek.Type == ParseDefinitions.KnowledgeStateNameToken)
                        {
                            var key = new ParsedFact(Tokens.Next().Value);
                            return new ParsedKnowledge(subKnowledge, key);
                        }

                        return subKnowledge.Count switch
                        {
                            0 => new ParsedKnowledge(new List<IKnowledge>(0)),
                            1 => subKnowledge.First(),
                            _ => new ParsedKnowledge(subKnowledge),
                        };
                    }
                    else if (next.Type == ParseDefinitions.KnowledgeStateSeparatorToken)
                    {
                        // nop
                    }
                    else
                    {
                        throw new InvalidOperationException( /* ToDo */);
                    }
                }
            }
        }

        private Token AssertNextToken(TokenType requiredTokenType)
        {
            if (Tokens.Peek.Type != requiredTokenType)
                throw new InvalidOperationException($"Unexpected token {Tokens.Peek}");
            return Tokens.Next();
        }

        private bool Reached(TokenType tokenType) => Tokens.Peek.Type == tokenType;
        private bool NotReached(TokenType tokenType) => !Reached(tokenType);
    }
}
