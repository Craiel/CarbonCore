namespace D3Theory.Contracts
{
    using System;
    using System.Collections.Generic;

    using D3Theory.Data;

    public interface IAttributeSet
    {
        event Action AttributesChanged;

        void Merge(IDictionary<D3Attribute, float> other, float factor = 1.0f);
        void Merge(IAttributeSet other, float factor = 1.0f);

        Dictionary<D3Attribute, float> GetAttributes();
    }
}
