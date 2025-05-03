Feature: Taxonomy Navigation
As a bird enthusiast
I want to browse birds by their taxonomic classification
So that I can explore related species

Scenario: Navigate to Taxonomy Browser
    When I navigate to the Taxonomy Browser page
    Then I should see a list of bird orders
    And I should see a list of bird families

Scenario: View Birds in an Order
    When I navigate to the Taxonomy Browser page
    And I click on the order "Passeriformes"
    Then I should see a list of Passeriformes birds
    And each bird should belong to the "Passeriformes" order
    And I should see sorting options

Scenario: View Birds in a Family
    When I navigate to the Taxonomy Browser page
    And I click on the family "Turdidae"
    Then I should see a list of Thrush birds
    And each bird should belong to the "Thrushes and Allies" family
    And I should see sorting options

Scenario: Sort Birds in Taxonomic List
    When I navigate to the Taxonomy Browser page
    And I click on the order "Passeriformes"
    And I click the "Sort by Scientific Name" option
    Then the birds should be sorted by scientific name