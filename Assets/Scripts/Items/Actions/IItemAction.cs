using Edgar.Items.Core;

namespace Edgar.Items.Actions
{
    public interface IItemAction
    {
        void Execute(InspectableObject inspectableObject);
    }
}
