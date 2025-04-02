// A class responsible for managing and controlling the replay of recorded commands.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Command
{
    public class ReplayService
    {
        protected ICommandProcessor commandProcessor;

        // A stack to store recorded commands for replay.
        private Stack<ICommand> replayCommandStack;

        // Property to get or set the current replay state.
        public ReplayState ReplayState { get; private set; }

        // Constructor for the ReplayService. Initializes the replay state as "DEACTIVE."
        public ReplayService(ICommandProcessor commandProcessor)
        {
            SetReplayState(ReplayState.DEACTIVE);
            this.commandProcessor = commandProcessor;
        }

        /// Set the replay state to the specified state.
        public void SetReplayState(ReplayState stateToSet) => ReplayState = stateToSet;

        // Set the command stack for replay, providing a collection of commands to replay.
        public void SetCommandStack(Stack<ICommand> commandsToSet) => replayCommandStack = new Stack<ICommand>(commandsToSet);

        // Execute the next recorded command in the stack if there are commands left to replay.
        public IEnumerator ExecuteNext()
        {
            yield return new WaitForSeconds(1f);

            if (replayCommandStack.Count > 0)
                commandProcessor.ProcessUnitCommand(replayCommandStack.Pop());
        }
    }
}


