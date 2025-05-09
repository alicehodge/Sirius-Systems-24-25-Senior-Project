Feature: Profile Page Content Check

Scenario: Profile Page displays correct statistics, email and display name
    Given I log in as "WingerHunter01@gmail.com" with password "676770Winger!"
    When I navigate to "Identity/Account/Manage"
    Then I should see text "77"
    And I should see text "34"
    And I should see text "Silver"
    And I should see text "Calcifer"
    And I should see text "WingerHunter01@gmail.com"