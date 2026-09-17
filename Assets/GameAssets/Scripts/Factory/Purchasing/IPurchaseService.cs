using System;

namespace MiniFactory.Purchasing
{
    public interface IPurchaseService
    {
        event Action OnChanged;
        event Action<string> OnPurchaseSucceeded;
        event Action<string, string> OnPurchaseFailed;

        PurchaseStateType State { get; }
        string LocalizedPrice { get; }

        bool TryPurchase();
    }
}
