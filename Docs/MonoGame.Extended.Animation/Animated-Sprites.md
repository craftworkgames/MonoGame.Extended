With Textures and Sprites we can import Sprite Sheets, collections of maped images paced on one same file, and draw them on screen through SpriteBatch like any other object on our game. Lets see how add "movement" on these sprites using the SpriteSheetAnimation classes from Animations.SpriteSheets package.

# Starting choosing what to animate
So lets add a simple character and some sprites to simulate movement, to import the resources using the pipeline we will need a pair of files here:
The Image file with all the sprites combined in a Map

![](http://i.imgur.com/gVfkoF2.png)

And an xml describing the coordenates and size of each sprite within the map:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<XnaContent>
    <Asset Type="System.Collections.Generic.Dictionary[System.String, Microsoft.Xna.Framework.Rectangle]">
	<Item><Key>33155_001.png</Key><Value>34 401 31 32</Value></Item>
	<Item><Key>33155_002.png</Key><Value>370 349 17 29</Value></Item>
    </Asset>
</XnaContent>
```

Hopefully there are a couple of programs to create shis files. Once you have those just throw them inside the Content Pipeline

![](http://i.imgur.com/hEhyOKh.png)

# Construct the move
Lets initiate our pixels using the Texture imported and then we will use the map to limit and access easily each image.

```c#
var characterTexture = content.Load<Texture2D>("Sprites/kunio");
var characterMap = content.Load<Dictionary<string, Rectangle>>("Sprites/kunioMap");
var characterAtlas = new TextureAtlas("kunio", characterTexture, characterMap);
var characterAnimationFactory = new SpriteSheetAnimationFactory(characterAtlas);

characterAnimationFactory.Add("idle", new SpriteSheetAnimationData(new[] { 0, 1 }, isLooping: true));

var characterSpriteAnimation = new AnimatedSprite(characterAnimationFactory,"idle");
```

Using a SpriteSheetAnimationFactory we can create Animations and set index of frames included. And passing this to AnimatedSprite we can name an autoplay animation. "idle" is the only one we have for now.

# Drawing the final result
On each update you will need to call the animation to make the transitions between frames.

```c#
characterSpriteAnimation.Update(deltaSeconds);
```

And finally on Draw the SpriteBatch will do the rest

```c#
spriteBatch.Begin();
spriteBatch.Draw(characterSpriteAnimation);
spriteBatch.End();
```

![](http://i.imgur.com/MhEocnH.gif)
