# MonoGame.Extended Coding Guidelines

## Introduction

As MonoGame.Extended evolves, we need consistent coding conventions to ensure maintainability and readability. This document outlines the standards expected to be followed when contributing to MonoGame.Extended.

These guidelines follow the general principle of "use Visual Studio defaults" and align with standard .NET coding conventions while addressing specific needs of the MonoGame.Extended library.

## Code Formatting

1. **Braces**: Use [Allman style](http://en.wikipedia.org/wiki/Indent_style#Allman_style) braces, where each brace begins on a new line.

2. **Indentation**: Use four spaces of indentation (no tabs).

3. **Single-line Statements**: All `if`/`else if`/`else` blocks should always use braces, even for single statements. This maintains consistency across the codebase.

    ```csharp
    // correct
    if (foo == true)
    {
        DoSomething();
    }

    // incorrect
    if (foo == true)
        DoSomething();
    ```

4. **Line Spacing**: Avoid more than one empty line at any time. Do not have two blank lines between members of a type.

5. **White Space**: Avoid spurious free spaces (e.g., avoid `if (someVar == 0)...`).

6. **This Keyword**: Avoid `this.` unless absolutely necessary.

## Naming Conventions

### Type Naming

- **Classes and Structs**: PascalCase (e.g., `HslColor`, `ParticleEmitter`)
- **Interfaces**: Prefix with "I" + PascalCase (e.g., `IGameComponent`, `IUpdateable`)
- **Enums**: PascalCase for type and values (e.g., `BlendMode.Additive`)
- **Type Parameters**: Single capital letter (T) or prefixed with "T" (e.g., `TKey`, `TValue`)
- **Make all internal and private types static or sealed** unless derivation is required.

### Member Naming

- **Methods**: PascalCase for all method names, including local functions (e.g., `Update()`, `Draw()`)
- **Properties**: PascalCase (e.g., `Position`, `Rotation`)
- **Private/Internal Fields**:
  - Use `_camelCase` for internal and private instance fields (e.g., `_particles`, `_isActive`)
  - Prefix static fields with `s_` (e.g., `s_defaultSize`)
  - Prefix thread static fields with `t_` (e.g., `t_instance`)
  - Use `readonly` where possible (after `static` when used together)
- **Public Fields**: Use PascalCasing with no prefix (use public fields sparingly; prefer properties)
- **Constants**: Use CONSTANT_CASE (e.g., `MAX_PARTICLES`, `DEFAULT_DURATION`)
- **Events**: PascalCase with "EventHandler" suffix for delegate types (e.g., `CollisionEventHandler`)

### Extension Methods

**Preferred Approach for MonoGame.Extended Types**:
- For types that are part of MonoGame.Extended (where we control the source code), prefer static methods within the type itself rather than extension methods
- Extension methods should primarily be used for extending types from MonoGame core or external libraries where we don't control the source

**When Extension Methods Are Necessary**:
- **Extension Class Naming**: `{TypeName}Extensions` (e.g., `ColorExtensions`, `Vector2Extensions`)
- **One Extension Class Per Type**: Each type being extended should have its own extension class
- **No Mixed Extensions**: Don't extend multiple types in a single extension class

### Conversion Methods

- **Method Naming Standard**:
  - Use `From{SourceType}` for static factory methods on the target type
    - Example: `TargetType.FromSourceType(source)` creates a `TargetType` from a `SourceType`
  - Use `To{TargetType}` for extension methods on the source type
    - Example: `source.ToTargetType()` as an extension method on `SourceType` returns a `TargetType`
  - Helper classes should use `{SourceType}To{TargetType}` naming for conversion methods
    - Example: `ConversionHelper.SourceTypeToTargetType(source)`

### Helper Classes

- **Helper Class Naming**: `{Domain}Helper` (e.g., `ColorHelper`, `MathHelper`)
- **Purpose**: For utility methods that don't belong to a specific type
- **Static Only**: Helper classes should be static classes with static methods
- **No Instance State**: Helper classes should not maintain instance state

## Declaration and Usage Guidelines

1. **Visibility**: Always specify visibility, even if it's the default (e.g., `private string _foo` not `string _foo`).
   - Visibility should be the first modifier (e.g., `public abstract` not `abstract public`).

2. **Namespace Imports**:
   - Specify at the top of the file, *outside* of `namespace` declarations
   - Order as follows:
     1. `System.*` namespaces
     2. MonoGame types at `Microsoft.Xna.Framework.*`
     3. MonoGame.Extended types
   - Example:

     ```csharp
     using System;
     using System.Collections.Generic;

     using Microsoft.Xna.Framework;
     using Microsoft.Xna.Framework.Graphics;

     using MonoGame.Extended.Graphics;
     using MonoGame.Extended.Particles;
     ```

3. **Type References**:
   - Use language keywords instead of BCL types (e.g., `int, string, float` instead of `Int32, String, Single`)
   - This applies to both type references and method calls (e.g., `int.Parse` instead of `Int32.Parse`)

4. **Variable Declarations**:
   - Highly discourage the use of `var`. C# is a strongly typed language, and using explicit types improves code readability.
   - Use `var` only when absolutely necessary, not as a general practice.
   - Target-typed `new()` can only be used when the type is explicitly named on the left-hand side (e.g., `SourceType source = new()`)

5. **String References**: Use `nameof(...)` instead of `"..."` whenever possible and relevant.

## Code Organization

### Namespace Structure

- **Mirror MonoGame Structure**: Follow the same organizational patterns as the core MonoGame framework
  - Graphics-related classes should be in `MonoGame.Extended.Graphics` (corresponding to MonoGame's `Microsoft.Xna.Framework.Graphics`)
  - Input-related classes should be in `MonoGame.Extended.Input` (corresponding to MonoGame's `Microsoft.Xna.Framework.Input`)
  - Math-related utilities should be in `MonoGame.Extended` directly (corresponding to MonoGame's `Microsoft.Xna.Framework`)
  - Audio extensions should be in `MonoGame.Extended.Audio` (corresponding to MonoGame's `Microsoft.Xna.Framework.Audio`)
- **Core Namespace**: `MonoGame.Extended` for fundamental extensions and types
- **Domain-Specific Namespaces**: `MonoGame.Extended.{Domain}` for features that extend beyond MonoGame's core
  - Example: `MonoGame.Extended.Particles`
  - Example: `MonoGame.Extended.Tiled`

### File and Class Organization

- **File Layout**: One type per file with matching filename
- **Fields**: Fields should be specified at the top within type declarations
- **Member Ordering**:
  - Fields (private, then protected, then public)
  - Properties
  - Constructors
  - Methods (grouped by functionality)
  - Nested types

## Documentation

- **Public APIs**: All public APIs must have XML documentation comments
- **Parameter Documentation**: Document all parameters with `<param>` tags
- **Return Value**: Document return values with `<returns>` tags
- **Exceptions**: Document exceptions with `<exception>` tags

## Error Handling

- **Validation**:
  - Validate parameters using `ArgumentNullException.ThrowIfNull` and similar methods where available.
- **Avoid Swallowing Exceptions**: Do not catch exceptions without handling or re-throwing

## Implementation Examples

### Extension Methods

```csharp
// CORRECT: Dedicated extension class for Foo
public static class FooExtensions
{
    public static Bar ToBar(this Foo foo)
    {
        // Implementation
    }
}

// CORRECT: Dedicated extension class for Bar
public static class BarExtensions
{
    public static Foo ToFoo(this Bar bar)
    {
        // Implementation
    }
}
```

### Conversion Methods

```csharp
// CORRECT: Factory method on target type
public struct TargetType
{
    public static TargetType FromSourceType(SourceType source)
    {
        // Implementation
    }
}

// CORRECT: Extension method on source type
public static class SourceTypeExtensions
{
    public static TargetType ToTargetType(this SourceType source)
    {
        // Implementation
    }
}

// CORRECT: Helper class method naming
public static class ConversionHelper
{
    public static TargetType SourceTypeToTargetType(SourceType source)
    {
        // Implementation
    }

    public static SourceType TargetTypeToSourceType(TargetType target)
    {
        // Implementation
    }
}
```

## Example File Structure

Below is an example following our style guidelines:

**ExampleClass.cs:**

```csharp
using System;
using System.Collections.Generic;

namespace Example
{
    public partial class ExampleClass
    {
        private readonly List<Item> _items;
        private float _rate;

        public int Count
        {
            get { return _items.Count; }
        }

        public bool IsEnabled { get; private set; }

        public ExampleClass(int capacity)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);

            _items = new List<Item>(capacity);
            IsEnabled = true;
        }

        public void Update(GameTime gameTime)
        {
            if (!IsEnabled)
            {
                return;
            }

            UpdateItems();
            CreateItems();
        }

        private void UpdateItems(GameTime gameTime)
        {
            // Implementation
        }

        private void CreateItems(GameTime gameTime)
        {
            // Implementation
        }
    }
}
```
