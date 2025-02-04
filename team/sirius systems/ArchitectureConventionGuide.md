# Architecture and Naming Convention Guide for Stork Dork

## Naming Conventions:

- PascalCase for variables and function names, camelCase for parameter names
- Lowercase names for directories
- Use '\_' for spaces
- Git branch names using id (i.e. SD-27-(descriptor if needed))
- Database naming: Differentiate between Identity and regular DB. Ex: SDAppDB & SDIdentityDB
- .NET Version: 8.0.12
- Using plain Javascript
- Using Bootstrap for frontend CSS

## Git Workflow:

- Branch naming convention based on user story ID (i.e. SD-27)
- Squashing commits when merging PR into main repo (On maintainers end)

## Performance Considerations:

Lazy loading is okay for a default approach.
