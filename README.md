**This 'read me' file contains a history of all prototypes and milestones of previous versions of the game as nice gifs and some text :)**

# Untitled Potato Game
The game aims to be a fast paced 2D top down game about potatoes with dashing being your main fighting mechanic

> [!Important]
> _This repo code will not be updated past `Prototype B`, as development has been moved to a private repo.
> However all releases will be done on this repository and occasional readme updates.
> (I may update this code at some points in development, but highly unlikely)_

---

For contact you can reach me here:
- medyan.mhd@gmail.com
- atomic_potato_32 (Discord)

# Concept art
<table style="border: none;">
  <tr>
    <!-- Left cell: vertical image spanning two rows -->
    <td rowspan="2" style="vertical-align: top;">
      <img src="https://github.com/user-attachments/assets/737d3fb3-b2dc-424c-a64d-adf45c69eb4c" 
           alt="Vertical Image" 
           style="">
    </td>
    <!-- Right cell: top square image -->
    <td style="vertical-align: top;">
      <img src="https://github.com/user-attachments/assets/cb3802ef-bf08-4143-b258-7c4acd01e882" 
           alt="Square Image 1" 
           style=" object-fit: cover;">
    </td>
  </tr>
  <tr>
    <!-- Right cell: bottom square image -->
    <td style="vertical-align: top;">
      <img src="https://github.com/user-attachments/assets/47f35db2-b672-4459-882a-64623d7bef42" 
           alt="Square Image 2" 
           style=" object-fit: cover;">
    </td>
  </tr>
</table>


# Prototype B
This prototype overhauled the game, centering the focus on the dashing. And an enemy behavior that resembles a chase.

<div display="flex"
    flex-wrap="nowrap">
  <img src="https://github.com/Atomic-Potato/Untitled-Potato-Game/assets/55362397/71e81102-7f19-4081-b396-8ea0f0e94bee" width="350"/>
  <img src="https://github.com/Atomic-Potato/Untitled-Potato-Game/assets/55362397/b44bf3b1-c0ad-4c4e-a973-c63996e654ca" width="350"/>
  <img src="https://github.com/Atomic-Potato/Untitled-Potato-Game/assets/55362397/36f08de3-4801-40eb-89ec-ccbf16ebd13b" height="325"/>
  <img src="https://github.com/Atomic-Potato/Untitled-Potato-Game/assets/55362397/e391f93b-4559-49fe-aaf1-f0a0d730ec37" height="325"/>
<div/>
  
# Prototype A (aka v.0.2-alpha)
**This build contains:**
- Player Controller, that includes
  - Horizontal Movement
  - Jumping
  - Rolling (Like a slide)
  - Dashing
  - Grappling Hook
 
 ![Player movement gif](https://user-images.githubusercontent.com/55362397/195272106-9dcaeb1f-1dc7-4b0f-ae77-17873ad9dfb7.gif)


- A companion that follows the player arround that can also be used to shoot behind the player while moving

![companion gif](https://user-images.githubusercontent.com/55362397/195274148-2c5689b3-f554-41bd-96ce-eedac8fa6167.gif)

- Flyer enemy: Follows the player and shoots up to N bullets/balls at the player (Note increasing the count of balls will shoot them in directions arround the enemy)

![flyer gif](https://user-images.githubusercontent.com/55362397/195274188-3392ea93-5f46-4eed-a0d4-944e380f9acb.gif)

- Rocket enemy: Falls from the sky, cannot be killed, on contact with any entity will give huge damage (no damage implemented yet) and a knock back.

![rocket gif](https://user-images.githubusercontent.com/55362397/195275522-35950a9b-0b1d-408a-854a-6e7c58d0c63a.gif)

- A world to test the player controller, and a section with a timer for speedrunning.

![course gif](https://user-images.githubusercontent.com/55362397/195274245-5f329c1b-b7fb-4904-844c-dc8673497581.gif)

