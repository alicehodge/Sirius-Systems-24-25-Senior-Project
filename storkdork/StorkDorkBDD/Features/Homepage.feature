Feature: Home page content

  Scenario: Home page shows welcome text
    Given I navigate to Home
    Then I should see text "Welcome to StorkDork"