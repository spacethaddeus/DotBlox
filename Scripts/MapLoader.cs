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

	Node3D WorkspaceNode = new();

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
		//RobloxFile map = RobloxFile.Open($"{OS.GetExecutablePath().GetBaseDir()}/Classic-Crossroads.rbxlx");

		Workspace workspace = map.FindFirstChildWhichIsA<Workspace>();

		if (workspace != null)
		{
			foreach (Instance inst in workspace.GetChildren())
			{
				Console.WriteLine($"{inst.Name}, Parent {inst.Parent}");
				switch(inst)
				{
					case Part:
						WorkspaceNode.AddChild(HandlePart((Part)inst));
						break;
					case Model:
						WorkspaceNode.AddChild(HandleModel((Model)inst));
						break;
				}
			}
		} 
		AddChild(WorkspaceNode);
	}

	public Node3D HandleModel(Model model)
	{
        Node3D ModelNode = new()
        {
            Name = model.Name
        };
		Console.WriteLine($"{ModelNode.Name}, Parent {model.Parent}");
        foreach (Instance inst in model.GetChildren())
		{
			switch(inst)
			{
				case Part:
					ModelNode.AddChild(HandlePart((Part)inst));
					break;
				case Model:
					ModelNode.AddChild(HandleModel((Model)inst));
					break;
			}
		}
		return ModelNode;
	}

	public Node3D HandlePart(Part part)
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
		PartPrefab.Position = new(part.CFrame.Position.X * 0.28F, part.CFrame.Position.Y * 0.28F, part.CFrame.Position.Z * 0.28F);

		Basis basis = new(part.CFrame.XVector.X, 
		part.CFrame.YVector.X, part.CFrame.ZVector.X, 
		part.CFrame.XVector.Y, part.CFrame.YVector.Y, 
		part.CFrame.ZVector.Y, part.CFrame.XVector.Z, 
		part.CFrame.YVector.Z, part.CFrame.ZVector.Z);

		Godot.Vector3 rotation = basis.GetEuler();

		PartPrefab.Rotation = rotation;

		MeshInstance3D meshInstance = (MeshInstance3D)PartPrefab.GetNode("MeshInstance3D");
		CollisionShape3D collisionObject3D = (CollisionShape3D)PartPrefab.GetNode("CollisionShape3D");

		StandardMaterial3D material = new()
		{
			AlbedoColor = new(part.BrickColor.R, part.BrickColor.G, part.BrickColor.B)
		};
		if(part.Shape is PartType.Block)
		{
			BoxMesh mesh = new()
			{
				Size = new(part.Size.X * 0.28F, part.Size.Y * 0.28F, part.Size.Z * 0.28F),
				Material = material
			};
			meshInstance.Mesh = mesh;

			BoxShape3D boxShape3D = new()
			{
				Size = new(part.Size.X * 0.28F, part.Size.Y * 0.28F, part.Size.Z * 0.28F)
			};

			collisionObject3D.Shape = boxShape3D;
		}
		else if (part.Shape is PartType.Ball)
		{
			SphereMesh mesh = new()
			{
				Radius = (part.Size.X * 0.28F) * 0.5f,
				Height = ((part.Size.X * 0.28F) * 0.5f) * 2,
				Material = material
			};
			meshInstance.Mesh = mesh;

			SphereShape3D sphereShape3D = new()
			{
				Radius = (part.Size.X * 0.28F) * 0.5f,
			};

			collisionObject3D.Shape = sphereShape3D;
		}
	
		return PartPrefab;
	}
}
