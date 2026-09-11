# Project Rules & Guidelines

## 1. Architectural Decisions
* **Human-Led Architecture:** Any architecture for this project **MUST** be done by a human. AI tools and agents are not permitted to make or alter system architecture without explicit human design and authorization.
* **Package Approval & Justification:** All NuGet and Node packages **MUST** be explicitly user approved and justified in the architecture logs ([`architecture.md`](file:///C:/Users/tyler/Overload/architecture.md)). Transitive or package dependencies do not need to be justified, but top-level packages MUST be clearly justified.

## 2. AI Code Attribution
* **Explicit Commit Notation:** All AI-generated code must be explicitly noted in the git commit message.
* **Purpose Statement:** Commit messages must clearly document that AI-generated code was used and specify what it was used for.

## 3. Prompt Logging
* **Prompt Registry (`usedPrompts.md`):** Every prompt used with AI tools for this project must be saved to [`usedPrompts.md`](file:///C:/Users/tyler/Overload/usedPrompts.md).
* **Required Metadata:** Each prompt entry must record:
  - **Timestamp**
  - **User ID**
  - **Prompt text**

## 4. Test Coverage & Quality Assurance
* **Mandatory Test Coverage:** After the initial project scaffolding phase is complete, all features **MUST** have corresponding test coverage via a test suite.
* **Verification:** No new feature or module is complete without passing unit/integration tests in the test suite.
