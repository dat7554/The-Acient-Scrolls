# The-Acient-Scrolls
A 3D RPG prototype featuring dungeon exploration, narrative progression, and interactive gameplay systems.

## Demo
[Watch Gameplay](https://youtu.be/1DpH4dtubGQ?si=44-__G5cR8ZRsNw5)


## Key Systems

### Character Controller
- Implemented a **Collide-and-Slide movement system** to handles **slope detection**, collision response, and stable traversal across uneven terrain
- Integrated **jump buffering and coyote time**

### Combat System
- First-person melee combat with **stamina-based mechanics**  
- Supports attacking, blocking, and resource management

### Enemy AI
- Built using a **finite state machine (FSM)** architecture  
- States include **Patrol → Chase → Attack** with clear transition logic  
- Patrol system uses **waypoint navigation with closest-point initialization**  
- Integrated with Unity NavMesh for pathfinding and movement  

### Inventory System
- Designed a **data-driven inventory system** using item metadata
- Supports **item stacking, storage, and interactions**  
- Integrated with combat, trading, and quest systems


## Gameplay Features
- Dungeon exploration with interactive environments  
- NPC interaction for trading and progression  
- Narrative system powered by Ink  


## Screenshots

<p align="center">
<img width="1578" height="867" alt="image" src="https://github.com/user-attachments/assets/666ad474-efa9-477d-a831-1463eb67f29c" />
</p>

<p align="center">
<img width="1557" height="873" alt="image" src="https://github.com/user-attachments/assets/50c4320d-4844-465a-9ce4-776999ebc693" />
</p>

<p align="center">
<img width="1517" height="863" alt="image" src="https://github.com/user-attachments/assets/99c1ce7e-6dab-4193-8bb7-7b413ad34c21" />
</p>

---

## Technologies Used
- Unity version 6000.0.32f1.
- NavMesh
- [Ink](https://www.inklestudios.com/ink/).
