using Robust.Shared.Serialization;

namespace Content.Shared.VendingMachines
{
<<<<<<< HEAD
    [NetSerializable, Serializable]
    public sealed class VendingMachineInterfaceState : BoundUserInterfaceState
    {
        public List<VendingMachineInventoryEntry> Inventory;
        //ADT-Economy-Start
        public double PriceMultiplier;
        public int Credits;
        public VendingMachineInterfaceState(List<VendingMachineInventoryEntry> inventory, double priceMultiplier, int credits)
        //ADT-Economy-End
        {
            Inventory = inventory;
            //ADT-Economy-Start
            PriceMultiplier = priceMultiplier;
            Credits = credits;
            //ADT-Economy-End
        }
    }
    //ADT-Economy-Start
    [Serializable, NetSerializable]
    public sealed class VendingMachineWithdrawMessage : BoundUserInterfaceMessage
    {
    }
    //ADT-Economy-End

=======
>>>>>>> 3fafe230f07f62b6118213808a149ff188246809
    [Serializable, NetSerializable]
    public sealed class VendingMachineEjectMessage : BoundUserInterfaceMessage
    {
        public readonly InventoryType Type;
        public readonly string ID;
        public VendingMachineEjectMessage(InventoryType type, string id)
        {
            Type = type;
            ID = id;
        }
    }

    [Serializable, NetSerializable]
    public enum VendingMachineUiKey
    {
        Key,
    }
}
