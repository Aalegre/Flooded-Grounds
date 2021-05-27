# [AA3] Unity in-engine audio demo 

## Integrants
* Alberto Alegre
* Francesc Obrador
* Salvio Calvillo

## Execution notes & warnings
For this demo we used a pre-made scene and a first person controller from the asset store.

The first person controller is a little janky, sometimes it gets stuck on ledges, for example when going upstairs. 
We decided not to modify it too much because of time constraints. We only deactivated the audio system in order to code our own.

### Controls
* **WASD**: movement
* **Space**: jump
* **Left click**: Toggle flashlight
* **Left shift**: Sprint

### Asset locations
#### Scripts
 All the custom scripts created by ourselves are in ```Assets/Scripts```
##### AmbienceSoundManager.cs
This script controls the ambient and music loops
##### AudioController.cs
This is a helper script that we use to have more variations from fewer audio clips. It enables sound instancing and helps us control the behaibour of an audio source.
##### AudioThumper.cs
This scirpt uses an instance of AudioController to play sounds when the object is hitted with an external force. This is used on the objects with physics enabled.
##### CustomVolumeGroup.cs
This script allows us to create custom volumes that check if the camera is inside or near in order to make custom area effects.
##### CustomAudioArea.cs
This script uses a CustomVolumeGroup to modify parameters of the mixer
##### FirstPerson_AIO_Helper.cs
We use this script to read from the First person controller that we downloaded from the asset store. Checks the floor, plays the footstep sounds, and toggles the flashlight
#### Sounds
All the sounds are in ```Assets/Sounds```
We did not use all the sounds that we have in the project.
#### Scene
The scene is in ```Assets/Flooded Grounds/Scenes/Scene A.unity```

The scene hierarchy is:
* **Animated**: Animated objects
* **Props**: Dynamic/Physics enabled objects (some audio sources are placed inside these objects)
* **Static**: Buildings, roads, trees... (some audio sources are placed inside these objects)
* **Lights**: different lights
* **Effects**: visual effects
* **Audio**: audio content
 * **AudioArea**: The area inside the house that changes mixer settings
 * **SoundManager**: The object that contains the AmbienceSoundManager, audio sources for ambient 2D sounds and the soundtrack loop
 * **Sources**: 2D and 3D audio sources
 * **Effects**: Reverb zones
* **Water**: water from the scene
* **FPS display**: displays current fps when playing
* **GUI**: Shows the move controls
* **EventSystem**: Canvas event system
* **FirstPerson-AIO**: First person controller, this has the camera, flashlight, footsteps, rain effect...

## Features
### 2D Ambience sound
We used a combination of dog barks and frog sounds. The sounds are randomly pitched, panned, and faded.

### Soundtrack
We used a low drone in loop to give an eery atmosphere.

### Footsteps system
We implemented a complex footsteps system that detects surfaces, not by the tag, but by the physic material.

The system distinguishes between regular footsteps, jump, and land. Applies volumes and effects accordingly.

Each step sounds with a random pitch and random volume to have more variations.

These are the different surfaces that we added, but its not fixed to the script, we could add more surfaces and sounds without changing any code:
* Grass
* Concrete
* Wood
* Carpet

### Reverb
Inside the house we added different reverb zones to emulate the different spaces inside.

### Mixer exposed parameters
We developed a custom script that checks if the camera is inside areas defined by the user that can change exposed parameters from the Mixer.

This custom script is used to change the sounds grouped under the "Outside" mix group, inside the house. We change volume, lowpass, and EQ parameters, in order to give the effect of an inside area.

### Interactive sounds
We have two different "interactive" sounds:

The **flashlight** has a "tick" sound when you toggle it.

Some furniture has physics and when you bump into it makes a wooden thump. The sounds are randomized and are more loud when the impact is greater.

### 3D sound sources
There are multitude of 3D sounds but the two more noticeable are:

The water sound that the "river" makes when you approach it in the beggining.

The different clocks that tick inside the house

### Sound grouping
We group all the sounds in these different mixer groups:
* Master
 * Music
 * SFX
   * Outside
   * Footsteps
   * Other

### Snapshots
We did not find any use for snapshots in this scenario.