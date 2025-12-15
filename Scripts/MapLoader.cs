using RobloxFiles;
using Godot;
using System;
using RobloxFiles.DataTypes;
using RobloxFiles.Enums;

public partial class MapLoader : Node3D
{
	// Called when the node enters the scene tree for the first time.
	[Export]
	PackedScene PartInstanceUnanchored;
	[Export]
	PackedScene PartInstanceAnchored;

	public void GenerateCubemap()
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
            images.Add(image);
        }

        Cubemap cubemap = new();
        cubemap.CreateFromImages(images);
        ResourceSaver.Save(cubemap, "res://cubemap.res");
	}

	public override void _Ready()
	{
		RobloxFile map = RobloxFile.Open(@"/Users/tadwalter/Downloads/Classic-Crossroads.rbxlx");

		Workspace workspace = map.FindFirstChildWhichIsA<Workspace>();

		if (workspace != null)
		{
			foreach (Instance inst in workspace.GetDescendants())
			{
				if(inst is Part part)
				{
					Node3D PartPrefab;
					if(part.Anchored == true)
					{
						PartPrefab = (StaticBody3D)PartInstanceAnchored.Instantiate();
					}
                    else
					{
						PartPrefab = (RigidBody3D)PartInstanceUnanchored.Instantiate();
					}

					PartPrefab.Name = part.Name;
					PartPrefab.Position = new(part.CFrame.Position.X * 0.05F, part.CFrame.Position.Y * 0.05F, part.CFrame.Position.Z * 0.05F);

					MeshInstance3D meshInstance = (MeshInstance3D)PartPrefab.GetNode("MeshInstance3D");
					CollisionShape3D collisionObject3D = (CollisionShape3D)PartPrefab.GetNode("CollisionShape3D");

                    StandardMaterial3D material = new()
                    {
                        AlbedoColor = new(part.BrickColor.R, part.BrickColor.G, part.BrickColor.B)
                    };

                    BoxMesh mesh = new();
					mesh = new()
					{
						Size = new(part.Size.X * 0.05F, part.Size.Y * 0.05F, part.Size.Z * 0.05F),
						Material = material
					};

                    BoxShape3D boxShape3D = new()
                    {
                        Size = new(part.Size.X * 0.05F, part.Size.Y * 0.05F, part.Size.Z * 0.05F)
                    };


                    meshInstance.Mesh = mesh;
					collisionObject3D.Shape = boxShape3D;
                    AddChild(PartPrefab);
				}
			}
		}  
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
