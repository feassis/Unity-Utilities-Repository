using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Utilities.Command
{
    /// <summary>
    /// A class responsible for invoking and managing commands.
    /// </summary>
    public abstract class CommandInvoker
    {
        // A stack to keep track of executed commands.
        private Stack<ICommand> commandRegistry = new Stack<ICommand>();

        protected ReplayService replayService;
        protected CoroutineManager coroutineManager;
        protected ICommandProcessor commandProcessor;

        public CommandInvoker(ReplayService replayService, 
            ICommandProcessor commandProcessor, CoroutineManager coroutineManager)
        {
            this.replayService = replayService;
            SubscribeToEvents();
            this.commandProcessor = commandProcessor;
            this.coroutineManager = coroutineManager;
        }

        protected abstract void SubscribeToEvents();

        public virtual void SetReplayStack()
        {
            replayService.SetCommandStack(commandRegistry);
            commandRegistry.Clear();
        }


        /// <summary>
        /// Process a command, which involves both executing it and registering it.
        /// </summary>
        /// <param name="commandToProcess">The command to be processed.</param>
        public void ProcessCommand(ICommand commandToProcess)
        {
            coroutineManager.RunCoroutine(ExecComandRoutine(commandToProcess));
        }

        private IEnumerator ExecComandRoutine(ICommand commandToProcess)
        {
            yield return ExecuteCommand(commandToProcess);
            RegisterCommand(commandToProcess);
        }

        /// <summary>
        /// Execute a command, invoking its associated action.
        /// </summary>
        /// <param name="commandToExecute">The command to be executed.</param>
        public IEnumerator ExecuteCommand(ICommand commandToExecute)
        {
            commandToExecute.Execute();

            yield return new WaitForSeconds(1.5f);
        }

        /// <summary>
        /// Register a command by adding it to the command registry stack.
        /// </summary>
        /// <param name="commandToRegister">The command to be registered.</param>
        public void RegisterCommand(ICommand commandToRegister) => commandRegistry.Push(commandToRegister);

        private bool RegistryEmpty() => commandRegistry.Count == 0;

        private bool CommandBelongsToActivePlayer()
        {
            return (commandRegistry.Peek() as UnitCommand).commandData.ActorPlayerID == commandProcessor.GetActivePlayer();
        }

        public void Undo()
        {
            if (!RegistryEmpty() && CommandBelongsToActivePlayer())
                commandRegistry.Pop().Undo();
        }

        public void UndoEveryonesLastTurn()
        {
            if (RegistryEmpty())
            {
                return;
            }

            var currentActorID = (commandRegistry.Peek() as UnitCommand).commandData.ActorUnitID;

            commandRegistry.Pop().Undo();

            while (!RegistryEmpty() && (commandRegistry.Peek() as UnitCommand).commandData.ActorPlayerID != currentActorID)
            {
                commandRegistry.Pop().Undo();
            }
        }

        public List<ICommand> GetEveryOneLastAction()
        {
            if (RegistryEmpty())
            {
                return null;
            }

            var currentActorID = (commandRegistry.Peek() as UnitCommand).commandData.ActorUnitID;

            var commandStackArray = commandRegistry.ToArray();

            int index = commandStackArray.Length - 2;

            while (currentActorID != ((commandStackArray[index] as UnitCommand).commandData.ActorUnitID) && index >= 0)
            {
                index--;
            }

            List<ICommand> result = new List<ICommand>();

            for (int i = index; i < commandStackArray.Length; i++)
            {
                result.Add(commandStackArray[i]);
            }
            result.Reverse();
            return result;
        }
    }
}

