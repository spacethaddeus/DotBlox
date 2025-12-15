using Godot;

namespace DotBlox.Scripts;

public partial class GenerateCubemap : Node
{
    public override void _Ready()
    {
        Godot.Collections.Array<Image> images = [];
        for(int i = 0; i < 6; i++)
        {
            var image = Image.CreateEmpty(128, 128, false, Image.Format.Rgb8);

            if(i % 3 == 0)
            {
                image.Fill(new Color());
            } 
            else if(i % 3 == 1)
            {
                image.Fill(new Color());
            }
            else
            {
                image.Fill(new Color());
            }
            images[i] = image;
        }

        Cubemap cubemap = new();
        cubemap.CreateFromImages(images);
        ResourceSaver.Save(cubemap, "res://cubemap.res");
    }
}