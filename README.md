# Potato-Punch
A fast paced 2D semi open world wave survival game about fighting hordes of mutated Potatoes!

## What is currently built:
- Player Controller, that includes
  - Horizontal Movement
  - Jumping
  - Rolling (Like a slide)
  - Dashing
  - Grappling Hook

- Flyer enemy: Follows the player and shoots up to N bullets/balls at the player (Note increasing the count of balls will shoot them in directions arround the enemy)

- Rocket enemy: Falls from the sky, cannot be killed, on contact with any entity will give huge damage (no damage implemented yet) and a knock back.

- A world to test the player controller, and a section to with a timer for speedrunning.

## Goals of this project:
### Goal #1: *`ENEMIES`*
- Polish the `Flyer` enemy.
- Change the `Rocket` enemy behavior so it would follow the player and can be mounted by the player.
- A wave system.
- Grounded enemies that patrol an area but are indeppendent of each wave

### Goal #2: *`WORLD`*
- 2 Biomes: Over the ground and under the ground (can be considered just as a single biome)
- Obstacles:
  - Ground Flame: falme that comes out of the ground that is invoked by the player stepping over it.
  - Rocket Highways: Basically a rocket enemy comes out from one end of the wall, and keeps going in a straight line into the other end of the wall
- Other Objects:
  - A moving platform.
  
*(The following goals are ambitious and may not be done)*
### Goal #3: *`Boss Fight`*
[Still in designing]

### Goal #4: *`Story`*
The Potato world has 2 Potato guardians that have protected the land for thousands of years. In the modern world, the power of the guardians has grown weaker as the potato people has developped technologies that is starting to surpass their powers.

One of the days, a meteorite carrying a virus crashes on the Potato Planet and luckily the virus doesnt spread, the Potato government captures the parasite and keeps it a secret from the world, and especially the Guardians. The scientists discover that these parasites give their hosts powerful abilities, but it needs a host that can whistand this power. And so they try to lure the guardians to ally with the potato government so they can use it on them, one of them refuses while the other accepts. After injecting the parasite into the guardian, it looses control, and starts mutating all the scientists and soldiers in the facility.

Weeks pass and the world is a mess, no one can stop this spread. After the failure of the guardians, the gods send one brave powerful soul that sprouts out from the ground and they say to it: "Find the Guardians. Restore the balance."
