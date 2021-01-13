using System.Collections.Generic;
using System.Linq;
using AlexNoddings.Protocols.Core;

namespace AlexNoddings.Protocols.Generator
{
    internal class StepWrapper
    {
        private int _stepNo = 0;
        private readonly IList<IStep> _steps;

        internal StepWrapper(IEnumerable<IStep> steps)
        {
            _steps = steps.ToList();
        }

        internal IAgent Receiver { get; private set; }
        internal IKnowledge? NextMessage { get; private set; }
        internal IAgent? NextAgent { get; private set; }

        internal bool MoveNext()
        {
            if (_stepNo >= _steps.Count) return false;

            IStep step = _steps[_stepNo];
            Receiver = step.ToAgent;

            if (_stepNo < _steps.Count - 1)
            {
                IStep nextStep = _steps[_stepNo + 1];
                NextMessage = nextStep.Knowledge;
                NextAgent = nextStep.ToAgent;
            }
            else
            {
                NextMessage = null;
                NextAgent = null;
            }

            _stepNo++;

            return true;
        }
    }
}
