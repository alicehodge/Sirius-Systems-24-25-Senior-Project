Feature: Range Submission
  As a user of StorkDork
  I want to submit updated range information for birds
  So that I can contribute to the accuracy of bird data

  Scenario: User can submit range information
    Given I log in as "squalicee@gmail.com" with password "Test1234"
    When I navigate to bird details for "Great Blue Heron"
    And I click "Submit Range Information"
    And I enter text "Found throughout North America, breeding from southern Canada through Florida." in "RangeDescription"
    And I enter text "Based on recent observations" in "SubmissionNotes"
    And I click "Submit Range Information"
    Then I should see text "Your range information has been submitted for review"
    And the submission should appear in the moderation queue

  Scenario: User cannot submit empty range description
    Given I log in as "squalicee@gmail.com" with password "Test1234"
    When I navigate to bird details for "Northern Cardinal"
    And I click "Submit Range Information"
    And I enter text "" in "RangeDescription"
    And I click "Submit Range Information"
    Then I should see validation error "Please provide range information"

  Scenario: User cannot submit very short range description
    Given I log in as "squalicee@gmail.com" with password "Test1234"
    When I navigate to bird details for "Northern Cardinal"
    And I click "Submit Range Information"
    And I enter text "USA" in "RangeDescription"
    And I click "Submit Range Information"
    Then I should see validation error "Range description should be at least 5 characters"

  Scenario: Anonymous user cannot submit range information
    When I navigate to bird details for "Great Blue Heron"
    Then I should not see "Submit Range Information" button

  Scenario: User can view submission status
    Given I log in as "squalicee@gmail.com" with password "Test1234"
    And I have submitted range information for "Great Blue Heron"
    When I navigate to bird details for "Great Blue Heron"
    Then I should see text "You have a pending range submission for this bird"