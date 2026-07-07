First Person Controller Prototype

A Unity-based First Person Controller prototype focused on physics-driven movement, modular architecture, and advanced traversal mechanics. This project was created as a learning exercise to explore gameplay programming, Unity physics, and clean software design principles.

Overview

The goal of this project was not only to build movement mechanics but also to design them in a maintainable and scalable way. Each major feature is implemented as an independent component with clear responsibilities, following the principles of composition and separation of concerns.

The prototype includes:

First Person Movement
Sprinting
Crouching
Slope Detection and Slope-Aware Movement
Wall Detection
Wall Climbing
Wall Running
Wall Jumping
Camera Roll/Tilt Effects
Physics-Based Character Movement
Gameplay Features
Movement
Rigidbody-driven movement
Ground and air movement handling
Movement state management
Adjustable movement speeds
Sprinting
Increased movement speed while grounded
Integrated into movement state system
Crouching
Dynamic player height scaling
Reduced movement speed while crouched
Slope Handling
Ground angle detection
Slope-aware movement projection
Prevents unwanted sliding behavior
Wall Climbing
Front-facing wall detection
Climb timers and cooldown management
Gravity override while climbing
Wall Running
Left and right wall detection
Dynamic wall direction calculation
Camera tilt feedback
Wall stick force for stable traversal
Wall Jumping
Dedicated wall jump system
Wall-normal based jump impulses
Compatible with both wall running and wall climbing states
Inspired by classic traversal mechanics
Project Architecture

The project follows a modular architecture where each system owns a single responsibility.

Player
├── Input
│   └── PlayerInput.cs
│
├── Core
│   ├── PlayerMovement.cs
│   ├── GroundDetector.cs
│   └── MovementDirectionOrientation.cs
│
├── Detection
│   ├── WallDetector.cs
│   └── WallRunDetector.cs
│
├── Traversal
│   ├── WallRun.cs
│   ├── WallClimb.cs
│   └── WallJump.cs
│
└── Camera
    ├── CameraFollow.cs
    └── PlayerCam.cs
Core Principles
Single Responsibility Principle (SRP)
Composition Over Inheritance
Decoupled Systems
Physics-Driven Gameplay
Maintainable Code Structure
Key Technical Learnings

Throughout development, this project provided practical experience with:

Unity Physics
Rigidbody movement
Force-based acceleration
Impulse-based jumping
Gravity manipulation
Drag and damping control
Environment Detection
Raycasting
Surface normal calculations
Wall angle validation
Ground checks
State Management
Movement state transitions
Traversal state handling
Cooldowns and timers
Priority-based movement modes
Software Architecture
Component-based design
Decoupling gameplay systems
Refactoring for maintainability
Clean responsibility boundaries
Notable Refactor

One of the biggest architectural improvements during development was extracting wall jump functionality from the main movement controller into its own dedicated WallJump component.

This change:

Reduced coupling between movement systems
Improved maintainability
Simplified future feature additions
Better aligned the project with SRP principles
Future Improvements

Architecture Diagram:<img width="1200" height="2008" alt="Frist Person Controller Architecture" src="https://github.com/user-attachments/assets/13ec6dba-9032-43c5-a620-31a6ae359224" />

Technologies
Unity
C#
Unity Physics (Rigidbody)
Raycasting
Repository Purpose

This repository serves as a learning project focused on:

Gameplay Programming
Unity Physics
Movement System Design
Software Architecture
Clean Code Practices

Feedback, suggestions, and code reviews are always welcome.
