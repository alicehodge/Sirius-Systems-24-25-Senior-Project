@checklist
Feature: Checklist Management

Scenario: Verify checklist display
    Given I am logged in
    When I navigate to the Checklist page
    Then I should see either "No checklists found. Create one?" or existing checklist data


@checklist
Scenario: Create a new checklist with selected birds
    Given I am logged in
    When I navigate to the Checklist page
    And I click "Create New Checklist"
    And I enter "Spring Migration 2024" as the checklist name
    And I search for and select the following birds:
        | Bird Name          |
        | American Robin     |
        | Northern Cardinal  |
    And I verify at least 2 birds are selected
    And I submit the checklist form
    Then I should be redirected to the Checklist index page