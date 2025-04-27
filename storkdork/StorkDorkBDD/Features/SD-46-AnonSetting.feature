Feature: Anonymous Birder/Settings

    Scenario: Settings page keeps previous choice, and birder is properly anonymous
        Given I log in as user "privers@b.com" with password "Privers_02"
        When I navigate to settings "/UserSettings/Settings"
        Then I should see the text "I want my sightings to be anonymous"
        When I confirm the checkbox
        Then I click the submit button
        When I refresh the page
        Then I should see the checkbox still checked
        When I navigate to map "/Leaflet/Map"
        Then I should see my sightings and click on a sighting marker
        Then I should see on popup "Spotted by: Anonymous"