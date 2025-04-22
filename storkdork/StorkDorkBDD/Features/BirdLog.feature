@birdlog
Feature: Bird Log Management

Scenario: Verify bird log display
    Given I log in as "mcaldwell@a.com" with password "Mcaldwell_01"
    When I navigate to the Bird Log page
    Then I should see either "No sightings found for your account." or existing bird data

# @birdlog
# Scenario: Log new bird sighting
#    Given I log in as "mcaldwell@a.com" with password "Mcaldwell_01"
#    When I navigate to the Bird Log create page
#    And I fill in the sighting form with:
#       | Field          | Value              |
#        | Bird Species   | Northern Cardinal  |
#        | Location       | Skagit Valley      |
#    And I submit the form
#   Then I should see "Log Created Successfully" on the confirmation page
