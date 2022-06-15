# Pathvis
Path tracing visualization in VR

<img src="https://user-images.githubusercontent.com/43035719/173844167-57506b1b-4387-4a23-ae97-3fce594696e0.png" width="800">

An application developed for my master thesis project in Computer Science. The PathVis application can visualize a simple and interactive path tracing algorithm in VR, through the use of a ray gun and a ray tracing camera. The application can work as an introduction to ray tracing in general, and help to gain a more intuitive understanding of ray tracing concepts.

<img src="https://user-images.githubusercontent.com/43035719/173848057-d449f4ad-4565-4500-9779-2f940a5dfbe0.png" height="300"> <img src="https://user-images.githubusercontent.com/43035719/173849107-68d9e77e-2a7e-45c5-8428-63bb3335a75e.png" height="300">

## Interactive ray tracing
PathVis implements a simplified version of the path tracer from the [Ray Tracing in One Weekend series of books](https://raytracing.github.io/):
- Only objects with colliders are involved in the interactive ray tracing
- A `RaytracingParticipator` component specifies the material of ray traced objects, similar to how regular materials in Unity work
- Textures need to have read/write enabled to show up in renders
- Rendering performance is not particularly since the rendering routine is a script

## Try the build
The release section has Windows builds of the application, including the prototype version and two versions tested by users.

To run a build, make sure you have SteamVR installed and a VR HMD connected to your pc, then launch the PathVis executable.
## Set up the project
1. Download or clone the project
2. Add the project to your Unity Hub
3. Ensure you have the correct version of Unity installed (2020.3.26f1)
4. Open the project

