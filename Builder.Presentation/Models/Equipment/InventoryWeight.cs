using Builder.Core;

namespace Builder.Presentation.Models.Equipment
{
    public class InventoryWeight : ObservableObject
    {
        private decimal _weightCarried;

        private decimal _weightCapacity;

        private decimal _liftingWeightCapacity;

        public decimal WeightCarried
        {
            get
            {
                return _weightCarried;
            }
            set
            {
                SetProperty(ref _weightCarried, value, "WeightCarried");
            }
        }

        public decimal WeightCapacity
        {
            get
            {
                return _weightCapacity;
            }
            set
            {
                SetProperty(ref _weightCapacity, value, "WeightCapacity");
            }
        }

        public decimal LiftingWeightCapacity
        {
            get
            {
                return _liftingWeightCapacity;
            }
            set
            {
                SetProperty(ref _liftingWeightCapacity, value, "LiftingWeightCapacity");
            }
        }

        public InventoryWeight()
        {
            _weightCarried = default(decimal);
            _weightCapacity = default(decimal);
            _liftingWeightCapacity = default(decimal);
        }
    }
}
