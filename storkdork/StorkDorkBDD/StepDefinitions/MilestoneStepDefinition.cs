using NUnit.Framework;
using Reqnroll;
using StorkDorkMain.Helpers; // new helper
using System;

namespace StorkDorkTests.Steps
{
    [Binding]
    public class MilestoneSteps
    {
        private int _achievement;
        private int _result;

        [Given(@"I have achieved (.*) milestones")]
        public void GivenIHaveAchievedMilestones(int achievement)
        {
            _achievement = achievement;
        }

        [When(@"I check the milestone tier")]
        public void WhenICheckTheMilestoneTier()
        {
            _result = MilestoneHelper.GetMilestoneTier(_achievement);
        }

        [Then(@"I should receive the (.*) tier")]
        public void ThenIShouldReceiveTheExpectedTier(string expectedTier)
        {
            int expectedValue = expectedTier switch
            {
                "GoldTier" => MilestoneHelper.GoldTier,
                "SilverTier" => MilestoneHelper.SilverTier,
                "BronzeTier" => MilestoneHelper.BronzeTier,
                "NoTier" => MilestoneHelper.NoTier,
                _ => throw new ArgumentException("Unknown tier: " + expectedTier)
            };

            Assert.That(_result, Is.EqualTo(expectedValue));
        }
    }
}
