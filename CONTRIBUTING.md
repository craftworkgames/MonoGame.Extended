# Contributing to MonoGame Extended

Welcome to MonoGame Extended! We're excited that you want to help expand the MonoGame ecosystem with additional functionality and tools.

MonoGame Extended is a community-driven extension library that adds features like sprite batching, input management, collision detection, content pipeline extensions, and much more to MonoGame. As an extension library, we have different priorities and workflows compared to the core MonoGame framework.

## Understanding Our Mission

MonoGame Extended is a collection of practical utilities designed to help you **prototype quickly and evolve into production** without throwing away your work. We're guided by four core principles:

**Prototype to Production**: Our tools help you manifest early prototypes and evolve them into working games without starting over. We focus on solving present-day pain points rather than anticipating future needs.

**Pragmatic, Not Opinionated**: We're not an engine or framework - we're decoupled utilities that solve specific game development problems. There's no "one right way" to make games, so we provide flexible tools that adapt to your constraints.

**Hardware-Aware**: When beneficial, our code leverages platform-specific capabilities rather than abstracting them away. Different hardware has different strengths, and good game development tools should respect that.

**Expert-Enabling**: We help you become a better game developer by providing well-documented, understandable solutions. Our goal is to save you time and share knowledge, not replace your expertise.

## Important Restrictions

Before you start contributing, please understand these critical limitations:

**Intellectual Property**: Only submit code you wrote personally or that you have explicit permission to contribute. Never use decompiled code from any source, including XNA, MonoGame, or other game engines. If you want to integrate external code, discuss it in an issue first. Your contributions will be distributed under the MIT License.

**What We Don't Accept**: We won't merge decompiled or reverse-engineered code, large PRs that change unrelated parts of the codebase, style-only changes without functional improvements, features that duplicate core MonoGame functionality, or breaking changes without prior discussion and approval.

## Getting Started Locally

Setting up your development environment is straightforward:

```bash
git clone https://github.com/MonoGame-Extended/MonoGame.Extended.git
cd MonoGame.Extended
dotnet restore
dotnet build
dotnet test
```

**Prerequisites**: You'll need .NET SDK 8.0 or later and MonoGame 3.8.* or compatible version. If you're working on content pipeline extensions, test your changes with sample content and document any new processor parameters.

**Development Workflow**: We use a feature branch workflow with `main` for stable releases and `develop` for active development. **All contributions should target the `develop` branch.**

## Ways to Contribute

### Bug Reports and Fixes

Found a bug? First check our [existing issues](https://github.com/MonoGame-Extended/MonoGame.Extended/issues) to avoid duplicates. When creating a bug report, provide a clear description of the problem, step-by-step reproduction instructions, expected versus actual behavior, configuration details, and error messages when available. Include a minimal code example or link to a reproduction repository when possible—text is preferred over screenshots for searchability.

### Feature Requests and Implementation

Feature requests require thoughtful consideration since they affect the entire community. Before implementing any new feature, submit a feature request using our template explaining what problem the feature solves and why it's important for game developers. Wait for community discussion and approval, ensure the scope aligns with our utility-focused approach, and plan for comprehensive test coverage.

### Documentation and Examples

Help other developers by improving API documentation, creating usage examples, writing tutorials or guides, and updating the wiki. Clear documentation is just as valuable as code contributions.

## Code Quality and Standards

We maintain consistent standards to ensure the codebase remains readable, maintainable, and performant across the entire MonoGame Extended ecosystem.

### Architecture and Design

Follow MonoGame's existing patterns and conventions while designing for extensibility—other developers should be able to build on your work. Prefer composition over inheritance where appropriate, keep dependencies minimal and well-justified, and ensure new features integrate well with existing components. Don't break existing APIs without strong justification, and consider deprecation paths for necessary breaking changes.

### Performance Considerations

Game development demands performance-conscious code. Minimize allocations in hot code paths, use object pooling for frequently created and destroyed objects, and prefer `Span<T>` and `ReadOnlySpan<T>` for memory-efficient operations. Consider cache-friendly data layouts for performance-critical components and profile your code when it affects game loop performance. Document any performance implications of new features.

### Testing Requirements

All new functionality needs comprehensive test coverage. Write unit tests that cover both happy paths and edge cases, test error conditions and parameter validation, and verify cross-platform compatibility when possible. Update existing tests if your changes affect them, and include manual testing instructions when automated testing isn't sufficient.

For complete coding standards including naming conventions, code organization, and documentation requirements, refer to our comprehensive [CODING_GUIDELINES](CODING_GUIDELINES.md) document.

## Pull Request Process

### Before You Submit

Fork the repository and create a branch from `develop` with a descriptive name. Make focused commits with clear, descriptive messages that explain what changed and why. Ensure your code follows our standards and includes appropriate test coverage.

### The Pull Request

Complete our PR checklist. **We won't review incomplete submissions**. Verify no overlapping PRs exist, follow our contribution guidelines and code of conduct, and write a descriptive title that summarizes your changes. Fill out the PR template with a clear description of the purpose and problem solved, links to related issues (use "closes #123" to auto-close issues), and a high-level overview of technical changes.

Keep PRs reasonably sized and focused on a single concern. Include tests that verify your changes work correctly, update documentation for any API changes, and ensure our GitHub Actions CI passes. Be patient during review—we're all volunteers with varying availability.

## Code Review Process

Understanding our review process helps set proper expectations and ensures smoother collaboration.

### What to Expect

We aim to acknowledge new PRs within 3-5 business days, though initial response times may vary during holidays or when maintainers have competing priorities. Simple fixes typically get reviewed within 1-2 weeks, while complex features may take longer. Other community members may provide feedback before maintainer review.

### Review Focus Areas

Our reviewers evaluate contributions across several dimensions. Code quality includes adherence to established coding standards, proper error handling and input validation, performance considerations for game development scenarios, and clear, self-documenting code. We look for adequate test coverage of new functionality, tests that cover edge cases and error conditions, and verification that existing tests still pass.

Documentation requirements include XML documentation for public APIs, updated README or wiki pages for new features, clear commit messages and PR descriptions, and migration notes for breaking changes. Integration aspects cover how well changes work with existing MonoGame Extended components, avoiding unnecessary breaking changes to public APIs, consistency with the library's architectural patterns, and proper namespace and assembly organization.

### During Review

Address all reviewer comments, even if just to explain your reasoning. Make requested changes in new commits rather than force-pushing during review, ask questions if feedback isn't clear, and be open to alternative approaches suggested by reviewers. Our reviewers focus on the code rather than the person, provide constructive feedback with explanations, and may offer mentoring for newer contributors.

## Version Compatibility

As an extension library, MonoGame Extended must maintain compatibility with specific MonoGame versions while supporting contributors working across different environments.

We currently target MonoGame 3.8.* as our stable baseline and track MonoGame development releases without guaranteeing compatibility until official release. When contributing code that affects dependencies, new features should target the current supported MonoGame version, avoid using MonoGame APIs introduced in development builds, and discuss version bumps in an issue before implementation.

Minimize new external dependencies and justify why existing .NET or MonoGame functionality isn't sufficient. Use conditional compilation for platform-specific features and test compatibility across target platforms when possible. If your contribution requires a MonoGame version bump, discuss the necessity first, document the impact on affected APIs, update documentation to reflect new minimum requirements, and consider whether alternatives might work with current versions.

MonoGame Extended follows [semantic versioning](https://semver.org/) with major versions for breaking changes, minor versions for new features, and patch versions for bug fixes. We maintain compatibility with the latest stable MonoGame release and typically support one previous major version.

## Platform Considerations

MonoGame Extended runs across Windows, macOS, Linux, and mobile platforms. When writing platform-specific code, use conditional compilation directives:

```csharp
#if WINDOWS
// Windows-specific implementation
#elif LINUX
// Linux-specific implementation
#endif
```

Test your changes on multiple platforms when possible, be mindful of platform-specific limitations, and document any platform restrictions in your code and PR description.

## Getting Help

Need assistance or have questions about contributing?

**Discord**: Join our [Discord server](https://discord.gg/xPUEkj9) for real-time discussion and community support.
**Issues**: Use GitHub issues for bug reports and feature requests with our provided templates.
**Discussions**: Start conversations about design decisions or general questions in GitHub Discussions.

## Licensing

MonoGame Extended is under the [MIT License](https://opensource.org/licenses/MIT) unless a portion of code is explicitly stated elsewhere. See the [LICENSE](LICENSE) for more details. Third-party libraries used by MonoGame Extended are under their own licenses, we always seek permission from the original author of those libraries.. Please refer to those libraries for details on the license they use.

We accept contributions in "good faith" that it isn't bound to a conflicting license. By submitting a PR you agree to distribute your work under the MonoGame Extended license and copyright.

## Recognition and Community

Contributors are recognized in our release notes and README. Significant contributors may be invited to join the maintainer team and help shape the future direction of MonoGame Extended.

---

Thank you for helping make MonoGame Extended better for the entire game development community!

💖 The MonoGame Extended Team
