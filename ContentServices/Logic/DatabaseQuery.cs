namespace CarbonCore.ContentServices.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using CarbonCore.Utils;

    public class DatabaseQuery
    {
        private readonly List<DatabaseQueryCriterion> criteria;
        private readonly List<DatabaseQueryOrder> order;

        private readonly DatabaseEntryDescriptor descriptor;

        private bool negateNext;
 
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DatabaseQuery(DatabaseEntryDescriptor descriptor)
        {
            this.criteria = new List<DatabaseQueryCriterion>();
            this.order = new List<DatabaseQueryOrder>();

            this.descriptor = descriptor;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public DatabaseEntryDescriptor Descriptor
        {
            get
            {
                return this.descriptor;
            }
        }

        public ReadOnlyCollection<DatabaseQueryCriterion> Criterion
        {
            get
            {
                return this.criteria.AsReadOnly();
            }
        }

        public ReadOnlyCollection<DatabaseQueryOrder> Order
        {
            get
            {
                return this.order.AsReadOnly();
            }
        }

        public DatabaseQuery Not()
        {
            this.negateNext = true;
            return this;
        }

        public DatabaseQuery IsEqual(string element, params object[] values)
        {
            DatabaseEntryElementDescriptor elementDescriptor = this.descriptor.GetElementByName(element);
            var criterion = new DatabaseQueryCriterion(elementDescriptor.Name, elementDescriptor.InternalType, elementDescriptor.DatabaseType, values)
                                {
                                    Type = DatabaseQueryCriterionType.Equals
                                };

            return this.AddCriterion(criterion);
        }

        public DatabaseQuery Contains(string element, params object[] values)
        {
            DatabaseEntryElementDescriptor elementDescriptor = this.descriptor.GetElementByName(element);
            var criterion = new DatabaseQueryCriterion(element, elementDescriptor.InternalType, elementDescriptor.DatabaseType, values)
                                {
                                    Type = DatabaseQueryCriterionType.Contains
                                };

            return this.AddCriterion(criterion);
        }

        public DatabaseQuery OrderBy(string element, bool ascending = true)
        {
            var orderEntry = new DatabaseQueryOrder(element)
                                 {
                                     Ascending = ascending
                                 };

            return this.AddOrder(orderEntry);
        }
 
        public DatabaseQuery AddCriterion(DatabaseQueryCriterion criterion)
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

        public DatabaseQuery AddOrder(DatabaseQueryOrder entry)
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
    }
}
