namespace ET.Analyzer
{
    public static class AnalyzeAssembly
    {
        private const string Model = "Model";
        private const string Hotfix = "Hotfix";


        public static readonly string[] AllHotfix =
        {
            Hotfix
        };

        public static readonly string[] AllModel =
        {
            Model
        };

        public static readonly string[] AllModelHotfix =
        {
            Model,Hotfix
        };
        
        public static readonly string[] All =
        {
            Model,Hotfix
        };
        
        
    }
}