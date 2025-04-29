Feature: Content Moderation
  As a moderator of StorkDork
  I want to review and manage user submitted content
  So that I can maintain quality and accuracy of the site

  Scenario: Moderator can view pending content queue
    Given I log in as "admin@storkdork.com" with password "Test1234!"
    When I navigate to "Moderation"
    Then I should see text "Content Moderation"
    And I should see text "Submission Date"
    And I should see text "Status"

  Scenario: Moderator can approve range submission
    Given I log in as "admin@storkdork.com" with password "Test1234!"
    And there is a pending range submission for "Great Blue Heron"
    When I navigate to "Moderation"
    And I click the "Approve" button for "Great Blue Heron" submission
    And I enter text "Range information verified" in "moderator-notes"
    And I click "Submit Approval"
    Then I should see status "Approved" for "Great Blue Heron" submission

  Scenario: Moderator can reject range submission
    Given I log in as "admin@storkdork.com" with password "Test1234!"
    And there is a pending range submission for "Northern Cardinal"
    When I navigate to "Moderation"
    And I click the "Reject" button for "Northern Cardinal" submission
    And I enter text "Incorrect range data" in "rejection-reason"
    And I click "Submit Rejection"
    Then I should see status "Rejected" for "Northern Cardinal" submission

  Scenario: Regular user cannot access moderation pages
    Given I log in as "user@storkdork.com" with password "Test5678!"
    When I navigate to "Moderation"
    Then I should see text "Access Denied"
    And I should be on page "/Identity/Account/AccessDenied"

  Scenario: Filter moderation history by status
    Given I log in as "admin@storkdork.com" with password "Test1234!"
    And there are moderated submissions with different statuses
    When I navigate to "Moderation/History"
    And I click "Approved" in filter buttons
    Then all visible submissions should have status "Approved"
    When I click "Rejected" in filter buttons
    Then all visible submissions should have status "Rejected"