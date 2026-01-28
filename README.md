# Starstuff-Task

This project is a technical task submission for Starstuff.

It demonstrates a Universal Vehicle Controller that can be attached to any arbitrary 3D object (table, bench, dog house, lamppost, etc.) and instantly make it drivable using arcade-style physics.

The goal is not realistic vehicle simulation, but responsive, grippy, and fun handling that works across:

- Different object shapes
- Different scales
- Different slope angles
- Desktop (keyboard) and mobile (virtual joystick)

## Unity Version
Unity 6.3
Physics: Unity PhysX (Rigidbody-based)

### Project Structure
Scripts (Assets/_Scripts)
1. UniversalVehicleController.cs (Main Task)

The core of the assignment.
Can be attached to any GameObject
Requires no per-vehicle manual configuration

Automatically adapts to:
- Object scale
- Collider shape
- Center of mass

Arcade-style handling:

- Snappy acceleration
- Strong steering response
- High grip
- Resistant to flipping
- Easy recovery if flipped

Supports:

- Keyboard input (WASD / Arrow keys)
- Virtual joystick input (mobile)

Key techniques used:

- Rigidbody-based force movement (not transform movement)
- Ground detection via raycast
- Slope-aware movement
- Downhill gravity cancellation for consistent uphill driving
- Upright stabilization torque
- Velocity clamping for predictable handling

2. CameraFollow.cs

- Simple third-person camera
- Smoothly follows the currently active drivable object
- Automatically updates when switching vehicles

3. UIController.cs

- Handles vehicle switching
- Integrates with on-screen UI buttons
- Designed for quick testing across multiple vehicle shapes

## Test Scene
### Scene: Gameplay

The test scene is designed to stress-test vehicle behavior across slopes and shapes.

Slope Testing

There are three sets of inclined slides:
- 🔴 Red Slides – 10° incline
- 🔵 Blue Slides – 20° incline
- 🟢 Green Slides – 40° incline

These are used to validate:

- Uphill acceleration
- Downhill stability
- Steering control on slopes
- Scale independence

### Drivable Objects

Under Drivable Objects in the hierarchy, multiple objects of different shapes and proportions are used:

- Table
- Bench
- Dog house
- Bottle crate
- Lamppost

All of them:

- Use the same UniversalVehicleController
- Have no manual tuning differences
- Remain controllable across all slope angles

Input
Desktop
- W / S or Up / Down → Forward / Reverse
- A / D or Left / Right → Steering

Mobile
- Virtual joystick (third-party asset)
- Same movement logic as desktop input
