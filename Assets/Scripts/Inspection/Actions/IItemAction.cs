using Edgar.Inspection.Core;

namespace Edgar.Inspection.Actions
{
    public interface IItemAction
    {
        void Execute(InspectableObject inspectableObject);
    }
}
