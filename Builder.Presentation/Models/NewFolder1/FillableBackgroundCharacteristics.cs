namespace Builder.Presentation.Models.NewFolder1
{
    public class FillableBackgroundCharacteristics
    {
        public FillableField Traits { get; } = new FillableField();

        public FillableField Ideals { get; } = new FillableField();

        public FillableField Bonds { get; } = new FillableField();

        public FillableField Flaws { get; } = new FillableField();

        public void Clear(bool clearOriginalContent = false)
        {
            Traits.Clear(clearOriginalContent);
            Ideals.Clear(clearOriginalContent);
            Bonds.Clear(clearOriginalContent);
            Flaws.Clear(clearOriginalContent);
        }
    }
}
