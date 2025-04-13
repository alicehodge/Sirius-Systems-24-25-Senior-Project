Feature: Milestone page content

  Scenario: Milestone page displays sightings made and photos contributed
    Given I open the milestone page as WingerHunter01@gmail.com
    Then I should see milestone text "You've made 77 sightings so far!"
    And I should see milestone text "You've uploaded 34 verified photos!"
    And I should see milestone text "Your most spotted bird is the Great Blue Heron"
    And I should see milestone text "April 10, 2025"