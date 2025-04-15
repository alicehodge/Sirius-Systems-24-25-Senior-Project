Feature: Milestone Tiers
  As a user of Stork Dork
  I want my achievements to show the correct milestone tier
  So that I can visually track my progress

  Scenario Outline: Get correct milestone tier for achievement
    Given I have achieved <achievement> milestones
    When I check the milestone tier
    Then I should receive the <expectedTier> tier

    Examples:
      | achievement | expectedTier |
      | 5           | NoTier       |
      | 25          | BronzeTier   |
      | 50          | SilverTier   |
      | 100         | GoldTier     |