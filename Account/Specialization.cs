namespace ClickQuest.Account
{
    public partial class Specialization
    {

        #region Singleton
        private static Specialization _instance;

        public static Specialization Instance
        {
            get
            {
                if (_instance is null)
                {
                    _instance=new Specialization();
                }
                return _instance;
            }
        }
        #endregion

        #region Private Fields

        private int specBuyingAmount;
        private int specBuyingThreshold;
        private int specBuyingBuff;

        private int specMeltingAmount;
        private int specMeltingThreshold;
        private int specMeltingBuff;

        private int specCraftingAmount;
        private int specCraftingThreshold;
        private int specCraftingBuff;

        private int specQuestingAmount;
        private int specQuestingThreshold;
        private int specQuestingBuff;

        private int specBlessingAmount;
        private int specBlessingThreshold;
        private int specBlessingBuff;

        private int specKillingAmount;
        private int specKillingThreshold;
        private int specKillingBuff;   

        #endregion
    }
}