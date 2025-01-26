# Requirements Workup

## Elicitation:
### Is the goal or outcome well defined? Does it make sense?
* (First draft) The **goal** is to create a birding web app that will help users log their bird sights, search for and learn about *bird species*, as well as provide users a way to track their progress and accomplishments. 
* This goal does a good job at suggesting who the target audience is, as well as defining a few key features of the web app, but could use a bit of clarification in some places. It could use some more specificity, specifically in technical areas such as addressing offline functionality, mobile web design and more. We can review these points and create a more refined goal statement. 
* By interviewing stakeholders, potential users, and development team member we could also gather more information that could help us improve our goal.
* (Final Draft) The goal is to create a user-friendly birding web app that enables users to log bird sightings, search for and learn about bird species through detailed resources, and track their birding progress and accomplishments (e.g., species logged, milestones reached). The app will support users in enhancing their birding experience by offering personalized insights and tools

### What is not clear from the given description?

What specific *features* are most important to users?
* Bird Species Data
* Map Data
* Bird Region Data
* Bird Logging
* Milestone Tracking/Displaying

What features will be present for user accessibility? (e.g. text to speech, colorblind mode, etc)

Often times birders find themselves without cell service, how will the app function offline?
* Preload region and bird data ahead of time?
* No offline service?
* Downloadable information for offline?

### How about scope? Is it clear what is included and what isn't?
It is clear what is included within the web app and what isn't.

#### Included:
* Searching for bird and region information
* Referencing bird patterns with weather data
* Logging bird spotting
* Earning and displaying milestones/accomplishments

#### Not Included:
* Chat features
* Store features
* AI bird identification

### What do you not understand?
#### Bird/Weather Domain Specifics:
* How do bird migration patterns work?
* When/Why/Where do certain birds migrate?
* How does the weather effect their behavior?
* Weather patterns and conditions
* etc

#### Working with geolocation data, weather data and APIs for those topics

#### Business Domain:

What is the workflow of a birder in a professional capacity? How does this app fit into that routine? How does this app fit into the routine of a non professional birder?

### Is there something missing?

Some missing information could include; Integration process with existing bird databases (e.g. eBird), defining a proper privacy and user policy that is clear and understandable, details on the milestone system and what purpose it serves in the app as well as safety information (e.g. avoid poison oak, watch for snakes, etc.)

### Get answers to these questions.

Discuss with stakeholders, project manager, potential users.

## Analysis:
### For each attribute, term, entity, relationship, activity ... precisely determine its bounds, limitations, types and constraints in both form and function. Write them down.