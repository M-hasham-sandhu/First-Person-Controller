# First Person Controller Prototype

A Unity-based First Person Controller prototype focused on physics-driven movement, advanced traversal mechanics, and clean software architecture. This project was developed as a learning exercise to deepen my understanding of gameplay programming, Unity physics, and maintainable system design.

## Features

### Core Movement
- Rigidbody-based first-person movement
- Ground and air movement handling
- Movement state management
- Physics-driven acceleration and momentum

### Sprinting
- Dynamic sprint speed
- Grounded movement checks

### Crouching
- Adjustable player height
- Reduced movement speed while crouched

### Slope Handling
- Ground detection using raycasts
- Slope-aware movement projection
- Prevention of unwanted sliding behavior

### Wall Climbing
- Front-facing wall detection
- Climb timers and cooldowns
- Custom gravity handling during climb

### Wall Running
- Left and right wall detection
- Dynamic wall direction calculation
- Wall stick force for stable movement
- Camera tilt feedback

### Wall Jumping
- Dedicated wall jump system
- Wall-normal based jump impulses
- Compatible with wall running and wall climbing
- Inspired by classic parkour and traversal mechanics

### Camera System
- Mouse look controller
- Smooth camera roll effects
- Wall-run visual feedback

---

## Architecture

The project follows a modular architecture where each component has a clear responsibility.

```text
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
```

### Design Principles

- Single Responsibility Principle (SRP)
- Composition Over Inheritance
- Modular Architecture
- Decoupled Gameplay Systems
- Maintainable and Scalable Codebase

---

## Technical Highlights

### Unity Physics
- Rigidbody-based movement
- Force-driven acceleration
- Impulse-based jumping
- Dynamic gravity control
- Linear damping and drag management

### Environment Detection
- Raycast-based wall detection
- Surface normal calculations
- Ground and slope checks
- Wall angle validation

### State Management
- Walking
- Sprinting
- Crouching
- Airborne
- Wall Running
- Wall Climbing

### Software Architecture
- Feature separation into dedicated components
- Reduced coupling between gameplay systems
- Easy feature extension and maintenance
- Clear ownership of responsibilities

---

## Key Learning Outcome

One of the most valuable improvements during development was refactoring wall jumping into its own dedicated `WallJump` component instead of keeping the logic inside the main movement controller.

This resulted in:

- Cleaner code organization
- Better maintainability
- Reduced dependencies between systems
- Improved adherence to the Single Responsibility Principle

---

## Challenges Solved

- Smooth transitions between movement states
- Preventing conflicts between wall climbing and wall running
- Reliable wall-normal calculations
- Balancing responsiveness and realism
- Managing gravity across multiple traversal mechanics
- Designing a scalable movement architecture

---

### Architecture Diagram
<img width="1200" height="2008" alt="Frist Person Controller Architecture" src="https://github.com/user-attachments/assets/809a4da2-177e-4643-b0a4-2e1c64b69546" />


---

## Technologies Used

- Unity
- C#
- Rigidbody Physics
- Raycasting
- Unity Input System
