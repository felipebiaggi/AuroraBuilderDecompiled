namespace Builder.Presentation.Services
{
    public interface IExpressionDataProvider
    {
        int GetLevel();

        int GetClassLevel(string className);

        int GetStrength();

        int GetDexterity();

        int GetConstitution();

        int GetIntelligence();

        int GetWisdom();

        int GetCharisma();

        int GetStatistic(string name);
    }

}
