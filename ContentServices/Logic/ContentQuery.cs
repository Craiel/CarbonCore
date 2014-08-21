namespace CarbonCore.ContentServices.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using CarbonCore.Utils;

    public enum CriterionType
    {
        Equals,
        Contains
    }

    public class ContentQuery
    {
        private readonly List<ContentCriterion> criteria;
        private readonly List<ContentOrder> order;

        private readonly IList<ContentReflectionProperty> eligibleProperties;

        private bool negateNext;
 
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ContentQuery(Type type)
        {
            this.criteria = new List<ContentCriterion>();
            this.order = new List<ContentOrder>();

            this.Type = type;

            this.eligibleProperties = ContentReflection.GetPropertyInfos(type);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Type Type { get; private set; }

        public ReadOnlyCollection<ContentCriterion> Criterion
        {
            get
            {
                return this.criteria.AsReadOnly();
            }
        }

        public ReadOnlyCollection<ContentOrder> Order
        {
            get
            {
                return this.order.AsReadOnly();
            }
        }

        public ContentQuery Not()
        {
            this.negateNext = true;
            return this;
        }

        public ContentQuery IsEqual(string property, object value)
        {
            var criterion = new ContentCriterion
                                {
                                    PropertyInfo = this.PropertyCheck(property),
                                    Type = CriterionType.Equals,
                                    Values = new[] { value }
                                };
            return this.AddCriterion(criterion);
        }

        public ContentQuery Contains(string property, object[] values)
        {
            var criterion = new ContentCriterion
                                {
                                    PropertyInfo = this.PropertyCheck(property),
                                    Type = CriterionType.Contains,
                                    Values = values
                                };
            return this.AddCriterion(criterion);
        }

        public ContentQuery OrderBy(string property, bool ascending = true)
        {
            var orderEntry = new ContentOrder { PropertyInfo = this.PropertyCheck(property), Ascending = ascending };
            return this.AddOrder(orderEntry);
        }
 
        public ContentQuery AddCriterion(ContentCriterion criterion)
        {
            if (this.criteria.Contains(criterion))
            {
                throw new ArgumentException("Criterion was already added");
            }

            if (this.negateNext)
            {
                criterion.Negate = true;
                this.negateNext = false;
            }

            this.criteria.Add(criterion);
            return this;
        }

        public ContentQuery AddOrder(ContentOrder entry)
        {
            if (this.order.Contains(entry))
            {
                throw new ArgumentException("Order Criterion was already added");
            }

            this.order.Add(entry);
            return this;
        }

        public override int GetHashCode()
        {
            return
                Tuple.Create(HashUtils.CombineObjectHashes(this.criteria), HashUtils.CombineObjectHashes(this.order))
                     .GetHashCode();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private ContentReflectionProperty PropertyCheck(string propertyName)
        {
            var entry = this.eligibleProperties.FirstOrDefault(x => x.Name.Equals(propertyName));
            if (entry == null)
            {
                throw new ArgumentException("Property was not found on underlying content object: " + propertyName);
            }

            return entry;
        }
    }
}
