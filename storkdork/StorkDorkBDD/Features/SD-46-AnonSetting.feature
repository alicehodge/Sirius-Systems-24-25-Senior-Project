Feature: Anonymous Birder/Settings

    Scenario: Settings page keeps previous choice, and birder is properly anonymous
        Given I log in as user "privers@b.com" with password "Privers_02"
        When I navigate to route "/UserSettings/Settings"
        Then I should see the text "I want my sightings to be anonymous"
        When I confirm the checkbox
        Then I click the submit button
        When I refresh the page
        Then I should see the checkbox still checked

    Scenario: Sightings on the map show anonymous instead of Patricia Rivers
        Given I log in as "privers@b.com" with password "Privers_02"
        When I have the anonymous setting enabled
        When I Navigate to the map page
        Then I should see that my sightings
        Then I should see "Birder: Anonymous"