namespace Builder.Presentation.ViewModels.Content
{
    public class AbilityScoreImprovementContentViewModel : SupportExpanderViewModel
    {
        public override bool HasExpanders => true;

        public AbilityScoreImprovementContentViewModel()
            : base(new string[1] { "Ability Score Improvement" })
        {
        }
    }
}
