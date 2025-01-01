using System.Numerics;
using Raylib_cs;

namespace Game;

public class LevelDefinitionPoints
{
    public LevelDefinitionPoints()
    {
        Points = new List<Vector3>();
        Indices = new List<short>();
        Normals = new List<Vector3>();
        TextureCoordinates = new List<Vector2>();
        BoundingBoxes = new List<BoundingBox>();
    }

    public List<Vector2> TextureCoordinates { get; set; }

    public List<Vector3> Normals { get; set; }


    public List<Vector3> Points { get; set; }
    public List<short> Indices { get; set; }
    public List<BoundingBox> BoundingBoxes { get; set; }
}

public class LevelDefinition
{
    private readonly string _levelDefinitionString;

    public LevelDefinition(string levelDefinitionString)
    {
        _levelDefinitionString = levelDefinitionString;
    }
    
    private int AddRoof(LevelDefinitionPoints points,float currentPos, float currentDepth, float widthStep, float depthStep,int index)
    {
        var list = new List<Vector3>();
        list.Add(new Vector3(-0.5f, 1.0f, 0.5f));    // Bottom left
        list.Add(new Vector3(0.5f, 1.0f, 0.5f));     // Bottom right
        list.Add(new Vector3(-0.5f, 1.0f, -0.5f));   // Top left
        list.Add(new Vector3(0.5f, 1.0f, -0.5f));  
        
        points.Points.AddRange(list.Select(p => new Vector3(currentPos+((p.X+0.5f)*widthStep), p.Y, currentDepth+((p.Z+0.5f)*depthStep))));
        Vector3[] normals = [new Vector3(0, -1, 0), new Vector3(0,-1, 0), new Vector3(0, -1, 0), new Vector3(0, -1, 0)];
        points.Normals.AddRange(normals);
        points.TextureCoordinates.AddRange([
            new Vector2(0.51f, 0.01f),         // Bottom left
            new Vector2(0.99f, 0.01f),     // Bottom right
            new Vector2(0.51f, 0.49f),             // Top left
            new Vector2(0.99f, 0.49f)          // Top right
        ]);
        points.Indices.Add((short)index);
        points.Indices.Add((short)(index+2));
        points.Indices.Add((short)(index+1));
        points.Indices.Add((short)(index+1));
        points.Indices.Add((short)(index+2));
        points.Indices.Add((short)(index+3));
        return index+4;
    }    

    private int AddFloor(LevelDefinitionPoints points,float currentPos, float currentDepth, float widthStep, float depthStep,int index)
    {
        var list = new List<Vector3>();
        list.Add(new Vector3(-0.5f, 0.1f, 0.5f));    // Bottom left
        list.Add(new Vector3(0.5f, 0.1f, 0.5f));     // Bottom right
        list.Add(new Vector3(-0.5f, 0.1f, -0.5f));   // Top left
        list.Add(new Vector3(0.5f, 0.1f, -0.5f));  
        
        points.Points.AddRange(list.Select(p => new Vector3(
            currentPos + (p.X * widthStep), 
            p.Y, 
            currentDepth + (p.Z * depthStep))));
        Vector3 v1 = list[1] - list[0];  // bottom edge
        Vector3 v2 = list[2] - list[0];  // side edge
        Vector3 normal = Vector3.Cross(v1, v2);
        normal = Vector3.Normalize(normal);
        Vector3[] normals = [normal,normal,normal,normal];
        points.Normals.AddRange(normals);
        points.TextureCoordinates.AddRange([
            new Vector2(0.01f, 0.5f),         // Bottom left
            new Vector2(0.48f, 0.5f),     // Bottom right
            new Vector2(0.01f, 1f),             // Top left
            new Vector2(0.48f, 1f)          // Top right
        ]);
        points.Indices.Add((short)index);
        points.Indices.Add((short)(index+1));
        points.Indices.Add((short)(index+2));
        points.Indices.Add((short)(index+1));
        points.Indices.Add((short)(index+3));
        points.Indices.Add((short)(index+2));

        return index+4;
    }
    
private int AddWall(LevelDefinitionPoints points, float currentPos, float currentDepth, float widthStep, float depthStep, float height, int index, float yrotation)
{
    float thickness = 0.1f;
    float angleInRadians = MathF.PI * yrotation / 180.0f;
    var rotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), angleInRadians);
    
    // Create vertices for all faces (24 vertices total - 4 for each face)
    var vertices = new List<Vector3>();
    
    // Front face
    vertices.AddRange(new[] {
        new Vector3(-0.5f, 0, 0),
        new Vector3(0.5f, 0, 0),
        new Vector3(-0.5f, height, 0),
        new Vector3(0.5f, height, 0)
    });
    
    // Back face
    vertices.AddRange(new[] {
        new Vector3(-0.5f, 0, -thickness),
        new Vector3(0.5f, 0, -thickness),
        new Vector3(-0.5f, height, -thickness),
        new Vector3(0.5f, height, -thickness)
    });
    
    // Top face
    vertices.AddRange(new[] {
        new Vector3(-0.5f, height, 0),
        new Vector3(0.5f, height, 0),
        new Vector3(-0.5f, height, -thickness),
        new Vector3(0.5f, height, -thickness)
    });
    
    // Bottom face
    vertices.AddRange(new[] {
        new Vector3(-0.5f, 0, 0),
        new Vector3(0.5f, 0, 0),
        new Vector3(-0.5f, 0, -thickness),
        new Vector3(0.5f, 0, -thickness)
    });
    
    // Left face
    vertices.AddRange(new[] {
        new Vector3(-0.5f, 0, 0),
        new Vector3(-0.5f, 0, -thickness),
        new Vector3(-0.5f, height, 0),
        new Vector3(-0.5f, height, -thickness)
    });
    
    // Right face
    vertices.AddRange(new[] {
        new Vector3(0.5f, 0, 0),
        new Vector3(0.5f, 0, -thickness),
        new Vector3(0.5f, height, 0),
        new Vector3(0.5f, height, -thickness)
    });
    
    // Rotate all vertices
    vertices = vertices.Select(v => Vector3.Transform(v, rotation)).ToList();
    
    // Transform to world space
    var finalVertices = vertices.Select(v => 
        new Vector3(currentPos + ((v.X + 0.5f) * widthStep), 
                   v.Y, 
                   currentDepth + (v.Z * depthStep))).ToList();

    var minX = finalVertices.Min(v => v.X);
    var minY = finalVertices.Min(v => v.Y);
    var minZ = finalVertices.Min(v => v.Z);
    
    var maxX = finalVertices.Max(v => v.X);
    var maxY = finalVertices.Max(v => v.Y);
    var maxZ = finalVertices.Max(v => v.Z);
    BoundingBox bb = new BoundingBox(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
    points.BoundingBoxes.Add(bb);
    
    // Add vertices
    points.Points.AddRange(finalVertices);
    
    // Create normals for each face
    var normals = new List<Vector3>();
    
    // Front face normal
    normals.AddRange(Enumerable.Repeat(Vector3.Transform(new Vector3(0, 0, 1), rotation), 4));
    // Back face normal
    normals.AddRange(Enumerable.Repeat(Vector3.Transform(new Vector3(0, 0, -1), rotation), 4));
    // Top face normal
    normals.AddRange(Enumerable.Repeat(Vector3.Transform(new Vector3(0, 1, 0), rotation), 4));
    // Bottom face normal
    normals.AddRange(Enumerable.Repeat(Vector3.Transform(new Vector3(0, -1, 0), rotation), 4));
    // Left face normal
    normals.AddRange(Enumerable.Repeat(Vector3.Transform(new Vector3(-1, 0, 0), rotation), 4));
    // Right face normal
    normals.AddRange(Enumerable.Repeat(Vector3.Transform(new Vector3(1, 0, 0), rotation), 4));
    
    points.Normals.AddRange(normals);
    
    var texCoords = new List<Vector2>();

// Add the same UV coordinates for each face
    for (int i = 0; i < 6; i++)  // 6 faces
    {
        texCoords.AddRange(new[] {
            new Vector2(0, 0),     // Bottom left
            new Vector2(0.5f, 0),  // Bottom right
            new Vector2(0, 0.5f),  // Top left
            new Vector2(0.5f, 0.5f) // Top right
        });
    }

    points.TextureCoordinates.AddRange(texCoords);
    
    // Indices for all faces
    for (int i = 0; i < 6; i++)
    {
        int baseIndex = index + (i * 4);
        points.Indices.AddRange(new short[] {
            (short)(baseIndex + 0),
            (short)(baseIndex + 1),
            (short)(baseIndex + 2),
            (short)(baseIndex + 1),
            (short)(baseIndex + 3),
            (short)(baseIndex + 2)
        });
    }
    
    // We added 24 vertices (4 vertices * 6 faces)
    return index + 24;
}     
    public LevelDefinitionPoints Parse(float minX, float minZ, float maxX, float maxZ)
    {
        var points = new LevelDefinitionPoints();
        var lines = _levelDefinitionString.Split("\n");
        var currentDepth = minZ;
        var depthStep = (float)(maxZ-minZ) / lines.Length;
        var index = 0;
        foreach (var line in lines)
        {
            var currentPos = minX;
            var widthStep = (float)(maxZ-minX) / line.Length;
            foreach (var character in line)
            {
                index=AddFloor(points, currentPos, currentDepth, widthStep, depthStep, index);
                index=AddRoof(points, currentPos, currentDepth, widthStep, depthStep, index);
                if (character == 'b' || character == 'c')
                {
                    // Først lager vi fire punkter i en firkant 
                    // 0,0 1,0 0,1 1,1 (z er bare satt til dybden så lenge det er en bakvegg, men på andre vil det være annerledes)
                    // Deretter lager vi trekanter av dem. Det gjøres med indexer. 0 er da 0,0 1 er 1,0 osv
                    
                    index=AddWall(points, currentPos, currentDepth, widthStep, depthStep, 1.0f, index,0f);
                }

                if (character == 'B' || character == 'C')
                {
                    index=AddWall(points, currentPos, currentDepth, widthStep, depthStep, 0.5f, index,0f);
                }

                if (character == 'l' || character == 'c')
                {
                    index=AddWall(points, currentPos, currentDepth, widthStep, depthStep, 1.0f, index,-90f);
                }
                
                /*if (character == 'L' || character == 'C')
                {
                    index=AddWall(points, currentPos, currentDepth, widthStep, depthStep, 0.5f, index,90f);
                    index=AddWall(points, currentPos, currentDepth-0.1f, widthStep, depthStep, 0.5f, index,-90f);
                }*/

                
                if (character == ' ')
                {
                    
                }
                currentPos += widthStep;
            }
            currentDepth += depthStep;
        }
        return points;
    }
    
    
}