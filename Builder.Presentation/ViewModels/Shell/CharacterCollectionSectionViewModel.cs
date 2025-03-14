using Builder.Presentation.ViewModels.Base;
using System.Threading.Tasks;

namespace Builder.Presentation.ViewModels.Shell
{
    public class CharacterCollectionSectionViewModel : ViewModelBase
    {
        public CharacterCollectionSectionViewModel()
        {
            _ = base.IsInDesignMode;
        }

        public override Task InitializeAsync(InitializationArguments args)
        {
            return base.InitializeAsync(args);
        }

        protected override void InitializeDesignData()
        {
            base.InitializeDesignData();
        }
    }
}
