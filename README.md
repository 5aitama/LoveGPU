# Love GPU ❤️
![](./Recordings/gif_animation_002.gif)

## Description
This is a mini project that i make for the fun and also to remind me how things that we can make with the GPU is beautiful. 
First of all this project demonstrate how to draw a big amount of same object (here is a cube but you can take a sphere or whatever...) and modify properties on each of them and all of that 60 frames per second bitch.


### How do that 5aitama.. How ???

Simple my friend you just need to ask the GPU : Hey baby show me ~~your boobs~~ 1.000.000 cubes and apply some things on each of them 😈.

## Explanation

Let's explane that. So first of all what we want ? We want to show 1 millions cubes and update there position based on time.

To maximize the performances, we need to store all data as close as possible to our GPU.

So let's allocate some memory in our GPU to store all cubes position. To allocate memory on GPU we need to create a buffer `ComputeBuffer` that will be responsible to store data.

```csharp
// Create a buffer. The buffer was empty for now...
private ComputeBuffer positionsBuffer = default;
```

Now we need to initialize it ! but when we initialize a `ComputeBuffer` we always need to specify how many elements the buffer can store and the size of each element in byte. So let's do that :

```csharp
...

// The amount of element that we want to store in our buffer.
const int MAX_AMOUNT_OF_ELEMENT = 1000 * 1000;

/*
 * This is the size of each element in our buffer.
 *
 * Why sizeof(float) * 3 ? because the position of each cube is
 * in a 3D space so we need 3 floats, 
 * one for x axis, one for y axis and another for the z axis.
 *
 * sizeof(float) just tell us what is the size of a float in bytes.
*/
const int SIZE_OF_EACH_ELEMENT = sizeof(float) * 3;

// Initialize our buffer.
positionsBuffer = new ComputeBuffer(MAX_AMOUNT_OF_ELEMENT, SIZE_OF_EACH_ELEMENT);

...
```