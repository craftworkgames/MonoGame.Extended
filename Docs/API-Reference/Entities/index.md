# Home

The Entities package is a high performance Entity Component System  inspired by [Artemis-odb](https://github.com/junkdog/artemis-odb/) and many other entity component systems.

## What is an ECS?

https://www.gamedev.net/articles/programming/general-and-gameplay-programming/understanding-component-entity-systems-r3013

An Entity Component System (ECS) is a way to build and manage the entities (or game objects) in your game by composing their component parts together. An ECS consists of three main parts:

### Components

A component is simply a class that holds some state data about the entity. Typically, components are lightweight and don't contain any game logic. 

### Entities

An entity is a composition of components identified by and ID. Often you only need the ID of the entity to work with it.

### Systems

A system is a class that will run during the game's `Update` or `Draw` calls. They usually contain the game logic about how to manage a filtered collection of entities and their components. 

