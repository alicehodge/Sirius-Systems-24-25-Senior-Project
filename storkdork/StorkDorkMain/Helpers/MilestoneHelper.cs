namespace StorkDorkMain.Helpers
{
    public static class MilestoneHelper
    {
        public const int GoldTier = 1;
        public const int SilverTier = 2;
        public const int BronzeTier = 3;
        public const int NoTier = 0;

        public static int GetMilestoneTier(int achievement)
        {
            if (achievement >= 100)
                return GoldTier;
            else if (achievement >= 50)
                return SilverTier;
            else if (achievement >= 25)
                return BronzeTier;
            else
                return NoTier;
        }
    }
}