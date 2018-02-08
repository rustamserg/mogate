REM Building shaders for IOS (CocosSharp)
..\Tools\3.2\2mgfx Shaders\lighting.fx Shaders\lighting.xnb
..\Tools\3.2\2mgfx Shaders\fade.fx Shaders\fade.xnb
copy /Y Shaders\*.xnb ..\mogateIOS\Resources\Content\Shaders

REM Building shaders for Mac and Windows (pure MonoGame)
..\Tools\3.0\2mgfx Shaders\lighting.fx Shaders\lighting.xnb
..\Tools\3.0\2mgfx Shaders\fade.fx Shaders\fade.xnb
copy /Y Shaders\*.xnb ..\mogate.Mac\Resources\Content\Shaders
move /Y Shaders\*.xnb Content\Shaders

REM Building images
..\Tools\3.0\mgcb /outputDir:Content /importer:TextureImporter /processor:TextureProcessor /build:Sprites\sprites.png