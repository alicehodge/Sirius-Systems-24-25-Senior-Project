Feature: GetSightingsByUser

    Scenario: Valid user ID returns list of sightings
        Given a valid user ID of 1
        And the user had 2 sightings
        When I request the sightings for the user
        Then the response should be OK
        And the result should contain 2 sightings

    Scenario: Invalid user ID returns bad request
        Given an invalid user ID of -5
        When I request the sightings for the user
        Then the response should be a BadRequest