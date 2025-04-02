// A class responsible for managing and controlling the replay of recorded commands.
namespace Utilities.Command
{
    public interface ICommandProcessor
    {
        public void ProcessUnitCommand(ICommand commandToProcess);

        public int GetActivePlayer();
    }
}


