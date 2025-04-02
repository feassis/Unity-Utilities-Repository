
namespace Utilities.Command
{
    public interface ICommand
    {
        void Execute();

        void Undo();
    }
}


