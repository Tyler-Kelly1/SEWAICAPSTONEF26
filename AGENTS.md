# AGY Project Memory & Operational Directives

> [!IMPORTANT]
> The agent **MUST** always reference, follow, and strictly enforce the project rules defined in [`rules.md`](file:///C:/Users/tyler/Overload/rules.md) and record prompt logs in [`usedPrompts.md`](file:///C:/Users/tyler/Overload/usedPrompts.md).

## Mandatory Core Rules

1. **Human-Led Architecture:**
   - Any architecture for this project **MUST** be designed and decided by a human. AI tools and agents are strictly forbidden from creating or altering system architecture without explicit human design and authorization.

2. **AI Code Attribution in Commits:**
   - All AI-generated code must be explicitly noted in git commit messages, detailing that AI code was used and what specific functionality/purpose it was generated for.

3. **Prompt Audit Logging (`usedPrompts.md`):**
   - Every prompt submitted to AGY/AI tools for this project **MUST** be recorded in [`usedPrompts.md`](file:///C:/Users/tyler/Overload/usedPrompts.md) with timestamp, user ID, and prompt text.

4. **Testing & Quality Assurance:**
   - After initial scaffolding is completed, all features **MUST** have corresponding test coverage via a test suite.

5. **Package Approval & Justification:**
   - All NuGet and Node top-level packages **MUST** be explicitly user approved and justified in the architecture logs ([`architecture.md`](file:///C:/Users/tyler/Overload/architecture.md)). Transitive or package dependencies do not need to be justified, but top-level packages must be clearly justified.

---
Refer to [`rules.md`](file:///C:/Users/tyler/Overload/rules.md) for complete project guidelines and [`architecture.md`](file:///C:/Users/tyler/Overload/architecture.md) for coding standards.

