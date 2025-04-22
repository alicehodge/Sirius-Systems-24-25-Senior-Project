Feature: Milestone page content

  Scenario: Milestone page displays sightings made and photos contributed
    Given I log in as "WingerHunter01@gmail.com" with password "676770Winger!"
    When I navigate to "Milestone"
    Then I should see text "You've made 77 sightings so far!"
    And I should see text "You've uploaded 34 verified photos!"
    And I should see text "Your most spotted bird is the Great Blue Heron"
    And I should see text "April 10, 2025"
