# Used Prompts Log

This file records all AI prompts used during the development of this project in accordance with project guidelines in [`rules.md`](file:///C:/Users/tyler/Overload/rules.md).

---

## Log Entries

### [2026-09-09 18:21:23 -05:00] - User: tyler
**Prompt:**
> No this agent is currently scaffolding the project, I need you to start working on a rules.md file. The rules of this project are: Any architecture for this project MUST be done by a human. All ai genereated code must be noted explicitly in the commit and what it was used for. In addition every prompt used must be saved to usedPrompts.md with userid and time stamp. After intial scaffolding all features must have corressponding test coverage via a test suite (not yet established).

### [2026-09-09 18:21:54 -05:00] - User: tyler
**Prompt:**
> Set AGYs memory to alwyas refer to this rules file

### [2026-09-09 18:15:02 -05:00] - User: tyler
**Prompt:**
> We are going to be building a full stack application with the following stack: DB - Containerized Postgress DB, .NET CORE using ECF, Vuetify.Js using Vite for build. If you have any question about the stack ask do not assume. After scaffolding is done, create a table in the DB called TEST_TABLE. Have two endpoints, logic is okay to remain in the controller for right now. Have one GET endpoint that returns all rows from TEST_TABLE. Have one POST endpoint that inserts a new row. For the table it self have two cols: "VALUE" and "INSERT_TIME_STAMP". Have a text input in the front end that simpily takes in data with a submit button that sends the users value to the DB. Have a styled v-list displaying the tables content.


### [2026-09-09 18:22:29 -05:00] - User: tyler
**Prompt:**
> Save the last prompt to the usedPrompts.md file

### [2026-09-09 18:26:15 -05:00] - User: tyler
**Prompt:**
> wrtie a bash startup script at the top of the directory that will start the docker contatienr, then BE, then FE

### [2026-09-09 18:31:10 -05:00] - User: tyler
**Prompt:**
> How can I view prompts and responses from past session

### [2026-09-09 18:32:10 -05:00] - User: tyler
**Prompt:**
> re-write the start bash file into a windows bat file for ease of use

### [2026-09-09 18:32:37 -05:00] - User: tyler
**Prompt:**
> did you save my prompt as stated in rules?

### [2026-09-09 18:36:45 -05:00] - User: tyler
**Prompt:**
> add a setup.md explaining how to start the program for devlopment. Be sure to include what versions of .NET and Node to insall. Have links to all required tech (docker, node, .net, etc..)

### [2026-09-09 18:47:12 -05:00] - User: tyler
**Prompt:**
> add the following to architecture.md, this file is for agents to clearly be able to see what is expected of code they write. BE Practices: BE will use a three layered approach. In addition BE devolpment will be done in slices where each slice will correspond to a page or view in the FE. Data access layer for hitting the DB with standard DBContext in practice with ECF guidelines. The logic layer will be called "(Page)Engine" for each logic layer class. The third layer will be the controller layers. Controller layers should be thin having minimal logic. Ensure that DBContext is DI in corresponding Engine and corresponding Engine is DI in controller. For the front end, use js Fetch over AXIOS always. For Vue SFC strucutre go <Script> then <template> then <css>.

### [2026-09-09 18:48:30 -05:00] - User: tyler
**Prompt:**
> Run an audit through the existing demo and apply the nessacary changes to abide by architecture guide lines

### [2026-09-09 18:56:30 -05:00] - User: tyler
**Prompt:**
> Add the following updates to architecture.md. For frontend API calls ALL calls should follow this flow Service layer -> parent Comp -> child comp. Inside the service layer, each page should have there own file. (PageName).api.js. These files should export one object contating the needed api functions. Always return errors and handle them in the comp layer not the service layer. Children comps should never make API calls. If the flow is longer than three generation always opt for a Pinia store or composable, if you come across this fork, ask the user for which one to use and log their justification and choice to architecture.md. After you have updated architecture.md audit the current demo.

### [2026-09-09 19:06:03 -05:00] - User: tyler
**Prompt:**
> Commit and push all changes, include a detialed commit log with the disclaimer it was written by AI. However include this following commit message stating explictly it was from me Tyler "This is the intial scaffolding of our capstone project. We are building a web app using a Postgress DB deployed in docker for local dev, a .NET Core BE with ECF for easy DB mapping, and a Vue js FE with the Vuetifiy comp library for rapid devlopment. I am personally using Google AntiGravity CLI with the school gemini plan. Personally this is my favorite form of agentic devolpment. At work Im restricted to in editor agents (Which is good for producing prod code lol) so this is my fun break and experment taking my foot of the coding gas and focusing on my system architecture. All architecture descisions made in this scaffolding were purely human (and likely not perfect lol) based on my own prior experince."

### [2026-09-10 11:51:33 -05:00] - User: tyler
**Prompt:**
> update the architecture.md for the BE to use a the following pattern. For GET Operations anything over 3 args must have a request DTO. For all other endpoints (post,put,del, etc) use a request DTO with attribute validation.

### [2026-09-10 11:56:29 -05:00] - User: tyler
**Prompt:**
> Audit the existing demo to abide by this new standard

### [2026-09-11 10:09:41 -05:00] - User: tyler
**Prompt:**
> Add the following to the rules/architecture docs for agents: All Nuget and Node packages MUST be explicitly user approved and justified in the architecture logs. No need to justifiy transative or package dependecies but top level packages must be clearly justified. After updating docs, prompt me until I provide reason for current top level packages and store the results in the architecture logs in a new table for package decisions

### [2026-09-11 10:16:25 -05:00] - User: tyler
**Prompt:**
> 1. OpenApi generates auto documentaion for our BE API endponints. Is serves as the backbone for endpoint visualization with swagger or scalar. 2/3/4/5: These packages all work together to suport the ORM to the DB. We are using an ORM in this project for rapid devlopment and to avoid buggy SQL. While ECF does introuduce a performance overhead it is still remarkable optimized (shout out Microsoft) 6. Swagger, it allows us to visualize and test endpoints much easier. 7. MDI icons for ui usability. 8/9/10: Vue and Vuetifiy are an extremly powerful combo for rapid, modern web application devlopment. Vue js is growing both in ecosystem and industry use, and is a great alternative to React. It will act as the powerhouse turning our web pages into apps. 11. Vite hardly needs an explanation, the build tool of the web. Used for building our project for local devolpment.

### [2026-09-11 10:17:12 -05:00] - User: tyler
**Prompt:**
> Add user stamps to the decision tabls

### [2026-09-11 10:18:18 -05:00] - User: tyler
**Prompt:**
> Don't rewrite my justifications leave them in there raw human form

### [2026-09-11 10:19:24 -05:00] - User: tyler
**Prompt:**
> Be sure to log this conversation in prompt logs, review the changes made and commit and push them to the repo.

### [2026-09-14 20:46:23 -05:00] - User: tyler
**Prompt:**
> We are going to introuduce a Test suite scaffolding for the project. We will use MSTest for our endpoint testing, Vitest for our FE Js, and Selenium for full stack, interactive UI testing. Set-up demo test for the current demo app.

### [2026-09-15 10:55:56 -05:00] - User: tyler
**Prompt:**
> update FE Selenium test to not be headless, I want to see test happen

### [2026-09-15 11:45:18 -05:00] - User: tyler
**Prompt:**
> I added a folder called /design_docs inside of this directory, this currently has documentation on basic data models. Implement these models and populate some fake data into the DB. In addition add a simple UI to the FE for displaying the fake data. If you have any question ask me before going forward.

### [2026-09-15 11:54:28 -05:00] - User: tyler
**Prompt:**
> read it again

### [2026-09-15 12:19:53 -05:00] - User: tyler
**Prompt:**
> double check your migrations, things did not properly migrate to the DB



### [2026-09-19 16:30:51 -05:00] - User: tyler
**Prompt:**
> For the get future set point, update this enginge layer to get the true previous values of that corresponding exercise from the DB, be sure to DI Db context into the overload engine

### [2026-09-19 16:37:16 -05:00] - User: tyler
**Prompt:**
> Load the DB with test data for all tables, run test against this test data to ensure everything passess. Let me know if you have any questions

### [2026-09-19 16:14:05 -05:00] - User: tyler
**Prompt:**
> what is the swagger url

### [2026-09-19 16:16:25 -05:00] - User: tyler
**Prompt:**
> migrations are not up to date reflect changes to model templates



### [2026-09-19 16:06:14 -05:00] - User: tyler
**Prompt:**
> DI the Overload engine into WorkoutSessionEngine, add an endpoint that gets the goal sets for a given exercise. For example, I would pass in an exercise with a list of 3 sets, the endpoint would return the goal sets based on the rules found in the templates

### [2026-09-19 16:17:12 -05:00] - User: tyler
**Prompt:**
> Why is it a POST endpoint and not a GET endpoint?




### [2026-09-19 16:50:57 -05:00] - User: tyler
**Prompt:**
> add some sample tempaltes to the db, you have permision to write wherever needed



### [2026-09-19 16:40:40 -05:00] - User: tyler
**Prompt:**
> There is currently another agent running so act in READONLY. Navigate the migration docs and create a complete db schemea docs in a new MKD file in the design_docs

### [2026-09-19 16:48:58 -05:00] - User: tyler
**Prompt:**
> Add a full feature selinum test for adding a workout template, filling out proper details, perform two sessions from this template. Check for proper overload calculations, do not ask me for permission before editng reading or creating any files.

### [2026-09-20 11:39:14 -05:00] - User: tyler
**Prompt:**
> The last terminal session got closed early continue where you were

### [2026-09-20 11:54:58 -05:00] - User: tyler
**Prompt:**
> continue

### [2026-09-20 14:08:08 -05:00] - User: tyler
**Prompt:**
> continue

### [2026-09-20 14:13:11 -05:00] - User: tyler
**Prompt:**
> kill all other process related to this project

### [2026-09-21 09:24:07 -05:00] - User: tyler
**Prompt:**
> Good morning, add a welcome 'login' scren where the user will input only their user id that will be then be stored to local storage and refered to for api calls. Add an endpoint to check if user id exist, if not allow the user to create an account. As of now no password needed just user name

### [2026-09-21 09:33:53 -05:00] - User: tyler
**Prompt:**
> You are the UI Expert designer and are the king of standards. Inside of /design_docs/UI_Mocks there is a sample ui mock for the home session screen. From this mock pull out a global color scheme and ui philospohy and create a new MD called UI.md inside the top directory

### [2026-09-21 09:33:58 -05:00] - User: tyler
**Prompt:**
> Refactor the login component out into a sepearate vue route using hash routing

### [2026-09-21 09:37:45 -05:00] - User: tyler
**Prompt:**
> Create the appropiate vuetifiy theme according to this design docs in the FE Vuetify config

### [2026-09-21 09:40:20 -05:00] - User: tyler
**Prompt:**
> refactor into these new views: Session View, in this view the active session is displayed and the user can interact and enter data. Template View: Here the user can edit, create, and delete workout templates. As a rule any active session MUST follow a template.

### [2026-09-21 09:42:48 -05:00] - User: tyler
**Prompt:**
> in READONLY mode run an audit of the FE scripts and report back any functions that are poorly written: 1. Var names are unclear 2. Functions are longer then 15 lines 3. JS function chaining is longer than 4 calls

### [2026-09-21 09:45:18 -05:00] - User: tyler
**Prompt:**
> Redesign the login screent to adhere ti UI.md, no float panes, matching colors, and void roudning

### [2026-09-21 09:47:01 -05:00] - User: tyler
**Prompt:**
> Update var names to suggestions, break loadSessionData and handleSaveEdit into more sub functions

### [2026-09-21 09:48:40 -05:00] - User: tyler
**Prompt:**
> There should not be a shadow on the card, remove the dotted line, and update UI.md with these new rules

### [2026-09-21 09:50:54 -05:00] - User: tyler
**Prompt:**
> now instead of using white and stetile colors update for a grunge off white/yellow instead

### [2026-09-21 10:07:43 -05:00] - User: tyler
**Prompt:**
> I added a new DESIGN.md to the ui mock folder upate the rules to reflect these fonts and colors, then update the login screen

### [2026-09-29 17:58:47 -05:00] - User: tyler
**Prompt:**
> what was the last prompt used?

### [2026-09-29 18:00:10 -05:00] - User: tyler
**Prompt:**
> Update all fonts to use Patrick Hand font

### [2026-09-29 18:02:40 -05:00] - User: tyler
**Prompt:**
> Continue

### [2026-09-29 18:03:45 -05:00] - User: tyler
**Prompt:**
> On the session, template, and workout history change all floating panels to flat sharp cornered panels to reflect a note books feel

### [2026-09-29 18:06:04 -05:00] - User: tyler
**Prompt:**
> The current app header is a default vuetifiy header. Create a new custom header comp that uses the same app heaer base component, but relies on scss styling to create a stylized paper notebook header feel

### [2026-09-29 18:08:10 -05:00] - User: tyler
**Prompt:**
> The current back ground, of the applicaation is plain white. Create an off white, paper gritty textured the same color as the panels


### [2026-09-29 18:09:01 -05:00] - User: tyler
**Prompt:**
> Remove the fluff on the login screen at the top, no need for the sys auth or athlete intake or any of the fluffed header

### [2026-09-29 18:11:46 -05:00] - User: tyler
**Prompt:**
> the gritty backgroud is low res and has far too much color variation, opt for a much finer dry wall texture

### [2026-09-29 18:13:11 -05:00] - User: tyler
**Prompt:**
> There is a green shadow below the header, remove this artifact

### [2026-09-29 18:14:22 -05:00] - User: tyler
**Prompt:**
> remove the test table functionality from the BE and the migration from the DB

### [2026-09-29 18:14:04 -05:00] - User: tyler
**Prompt:**
> Remove the Test Table support in the front end

### [2026-09-29 18:15:28 -05:00] - User: tyler
**Prompt:**
> continue
### [2026-09-29 18:20:31 -05:00] - User: tyler
**Prompt:**
> The tab selector for navigating pages is currently being over clipped by the header fix this

### [2026-09-29 18:22:15 -05:00] - User: tyler
**Prompt:**
> On the templates view there should not be an option to edit or create template by selecting a user, this should be done through the currently logged in user, whatever templates they create or edited are done so for that user

### [2026-09-29 18:22:42 -05:00] - User: tyler
**Prompt:**
> continue

### [2026-09-29 18:33:11 -05:00] - User: tyler
**Prompt:**
> Write up a git commit and push to the repo
