using System.Collections.Generic;
using ValueSystem.Save;

namespace ValueSystem
{
    public interface IValueSystem
    {
        IReadOnlyDictionary<string, ValueHandler> ValueHandler { get; }

        void Setup(ValuesSave save);

        bool Change(ValueData data, float amount, bool forced = false);
        bool Change(string valueId, float amount, bool forced = false);

        bool Add(ValueData data, float amount);
        bool Add(string valueId, float amount);

        bool CanSubtract(ValueData data, float amount);
        bool CanSubtract(string valueId, float amount);

        ValueHandler GetValueHandler(ValueData data);
        ValueHandler GetValueHandler(string valueId);

        float GetValue(ValueData data);
        float GetValue(string valueId);

        bool TrySubtract(ValueData data, float amount, bool forced = false);
        bool TrySubtract(string valueId, float amount, bool forced = false);

        void SetValue(ValueData data, float value);
        void SetValue(string id, float value);
    }
}