# Labels

This repo uses a small, consistent label set to keep triage simple and filtering useful.

## Labeling rules (quick)

- Every issue/PR should have **exactly one `Kind`** label.
- Add **0-2 `scope:`** labels (keep it focused).
- Add **`platform:`** only when the issue is OS-specific or OS-reproducible.
- Add **`priority:`** only when you're intentionally ordering work.
- Use **`status:`** labels only when state is not obvious from the conversation or GitHub state.

---

## Kind (pick exactly one)

| Label         | Description                                                                                                                                                                                              |
| ------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `bug`         | Something is broken, crashes, regresses, or behaves unexpectedly. The behavior is observable and differs from documented or intended behavior.                                                           |
| `feature`     | A new user-facing capability that does not exist yet. Adds something new to the public surface: a new type, method, system, or subsystem.                                                                |
| `enhancement` | An improvement to an existing feature. Covers quality-of-life improvements, better defaults, expanded platform/format support, or additional overloads for existing APIs.                                |
| `refactor`    | Internal restructuring with no intended behavior change.If it *might* change behavior, consider `bug` or `enhancement` instead.                                                                          |
| `docs`        | Documentation only work: XML doc comments, README updates, guides, or website docs. If code is also changed, prefer the relevant `Kind` label and add `docs` only if documentation is the primary focus. |
| `question`    | Support, clarification, or discussion threads. These often close without code changes once answered.                                                                                                     |
| `security`    | A security concern or vulnerability. If sensitive, avoid posting details publicly; use private reporting where possible.                                                                                 |

---

## Compatibility

| Label             | Description                                                                                                                                                                                                                                                                                                      |
| ----------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `breaking-change` | The change requires user code changes or breaks binary/behavioral compatibility.  This includes, but is not limited to, public API signature changes or removals, behavioral changes that will break existing projects at runtime, and content pipeline changes requiring reexport, rebuild, or workflow changes |

---

## Scope (0-2 labels)

> Scopes describe **where** the work lands. Keep this focused; two labels max.

| Label                     | Description                                                                                                                                                                                                 |
| ------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `scope: api`              | The public API surface: types, method signatures, properties, interfaces, or serialization contracts. Use when the primary concern is what consumers see and call.                                          |
| `scope: rendering`        | SpriteBatch usage patterns, `PrimitiveBatch`/VectorDraw primitives, effects/shaders, `BitmapFonts` rendering, draw order, or render correctness.                                                            |
| `scope: viewport`         | `ViewportAdapter` implementations, camera systems, resolution scaling, letterboxing, pillarboxing, DPI handling, and safe-area support.                                                                     |
| `scope: tilemap`          | Tiled map editor (`.tmx`) loading and rendering, tile layers, object layers, tilesets, animated tiles, and the Tiled map renderer.                                                                          |
| `scope: content-pipeline` | MGCB workflows; importers and processors for atlases, bitmap fonts, animations, particles, Tiled maps, and JSON content; content build errors; pipeline output formats.                                     |
| `scope: math`             | Geometric primitives (`CircleF`, `RectangleF`, `EllipseF`, `Angle`, `Point2`), bounding volumes, transforms, shape utilities (`Polygon`, `Polyline`), and numeric correctness or precision issues.          |
| `scope: input`            | Keyboard, mouse, gamepad, and touch input; extended input state tracking; input mappings; platform-specific input quirks.                                                                                   |
| `scope: physics`          | The Collisions subsystem: broadphase/narrowphase collision detection, spatial queries, collision layers, and integration behavior. Note: MonoGame.Extended does not simulate forces or rigid-body dynamics. |
| `scope: animations`       | `AnimatedSprite`, animation controllers, animation state machines, playback/timing, the Tweening system, easing functions, and color/transform interpolation.                                               |
| `scope: ecs`              | The optional Entity Component System: entities, components, systems, and the world/entity manager. Use for issues specific to the ECS subsystem.                                                            |
| `scope: particles`        | The particle effects system: emitters, modifiers, particle data, profiles, and content pipeline integration for `.ember` particle effect files.                                                             |
| `scope: screens`          | The `ScreenManager`, game screen/state transitions, screen stack behavior, and game state lifecycle management.                                                                                             |
| `scope: collections`      | Performance-oriented collections: `Bag<T>`, `Deque<T>`, object pools, poolable types, and collection-related utilities.                                                                                     |
| `scope: performance`      | Allocation investigations, hot-path profiling, batching efficiency, benchmark additions, and profiling-driven improvements across any subsystem.                                                            |
| `scope: tests`            | Test additions, test fixes, test harness improvements, and validation coverage.                                                                                                                             |
| `scope: build`            | CI pipelines, NuGet packaging, release automation, build scripts, multi-targeting (MonoGame / FNA / KNI), and solution/project structure.                                                                   |

---

## Priority (optional)

Only set priority when you're intentionally ordering work.

| Label              | Description                                                  |
| ------------------ | ------------------------------------------------------------ |
| `priority: high`   | Important; should be addressed next sprint/cycle.            |
| `priority: medium` | Default. Apply only if you want the priority to be explicit. |
| `priority: low`    | Nice to have; backlog item with no near-term urgency.        |

---

## Status (optional)

Use status labels to reflect workflow state *beyond* open/closed.

| Label                  | Description                                                                                                      |
| ---------------------- | ---------------------------------------------------------------------------------------------------------------- |
| `status: in-progress`  | Someone is actively working on it. Ideally link a branch, PR, or comment.                                        |
| `status: blocked`      | Cannot proceed until something external changes or a decision is made. Add a short note explaining the blocker.  |
| `status: needs-info`   | Waiting on the reporter or a maintainer to provide logs, a repro, a sample project, or other clarifying details. |
| `status: needs-review` | PR is ready for review (prefer on PRs, not issues).                                                              |

---

## Platform (optional)

Use when the issue is OS-specific or the repro is OS-dependent.

| Label               | Description |
| ------------------- | ----------- |
| `platform: windows` | Windows     |
| `platform: macos`   | macOS       |
| `platform: linux`   | Linux       |

---

## Community / Triage helpers

| Label              | Description                                                                                                                                                                                                                                                          |
| ------------------ | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `good first issue` | Beginner-friendly, well-scoped, and ready for an outside contributor. Should have clear reproduction steps or a precise acceptance criteria, pointers to the relevant files and subsystems, and requires minimal domain knowledge of MonoGame or Extended internals. |
| `help wanted`      | Maintainers are open to outside contribution. Ideally add pointers on where to start and what an acceptable solution looks like.                                                                                                                                     |
| `duplicate`        | The issue already exists. Close with a link to the original.                                                                                                                                                                                                         |
| `invalid`          | Not actionable, cannot be reproduced, or not a bug in MonoGame.Extended. Explain why when applying.                                                                                                                                                                  |
| `wontfix`          | Intentionally not addressing. Explain why and suggest alternatives where possible.                                                                                                                                                                                   |

---

## Examples

| Scenario                                   | Labels                                                  |
| ------------------------------------------ | ------------------------------------------------------- |
| Crash in Tiled renderer on Linux           | `bug`, `scope: tiled`, `platform: linux`                |
| Add new Camera helper API                  | `feature`, `scope: api`, `scope: viewport`              |
| Reduce allocations in SpriteBatch batching | `enhancement`, `scope: performance`, `scope: rendering` |
| ECS system registration order bug          | `bug`, `scope: ecs`                                     |
| Particle emitter `MaxParticles` ignored    | `bug`, `scope: particles`                               |
| Screen transition flicker on resize        | `bug`, `scope: screens`, `scope: viewport`              |
| Object pool leaks on `Return()`            | `bug`, `scope: collections`                             |
| Upgrade CI/release pipeline                | `enhancement`, `scope: build`                           |
| Add XML docs to public math types          | `docs`, `scope: math`                                   |
