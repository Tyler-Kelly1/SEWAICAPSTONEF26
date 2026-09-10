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

### [2026-09-10 12:16:40 -05:00] - User: tyler
**Prompt:**
> commit and push these changes, with the follow message "Update architecture.md for request DTOs, audit demo to ensure compliance - Tyler (Human Written)"



