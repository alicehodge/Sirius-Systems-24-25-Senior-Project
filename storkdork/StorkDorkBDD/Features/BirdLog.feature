@birdlog
Feature: Bird Log Management

Scenario: Verify bird log display
    Given I log in as "mcaldwell@a.com" with password "Mcaldwell_01"
    When I navigate to the Bird Log page
    Then I should see either "No sightings found for your account." or existing bird data


@birdlog
Scenario: Log new bird sighting
    Given I log in as "mcaldwell@a.com" with password "Mcaldwell_01"
    When I navigate to the Bird Log create page
    And I fill in the sighting form with:
        | Field          | Value              |
        | Bird Species   | Northern Cardinal  |
        | Location       | Skagit Valley      |
    And I submit the form
    Then I should be redirected to the confirmation page


@birdlog
Scenario: New sighting increments milestones
  Given I log in as "mcaldwell@a.com" with password "Mcaldwell_01"
  When I navigate to the Milestones page
  And I capture the initial Sightings Made count

  When I navigate to the Bird Log create page
  And I fill in the sighting form with:
      | Field        | Value             |
      | Bird Species | Northern Cardinal |
      | Location     | Skagit Valley     |
  And I submit the form
  Then I should be redirected to the confirmation page
    When I navigate to the Milestones page
  Then the Sightings Made count should be incremented by one
