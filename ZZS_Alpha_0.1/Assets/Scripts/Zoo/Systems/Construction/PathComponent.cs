using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zoo.Assets;
using Zoo.Assets.Objects.Paths;
using Zoo.Utilities.PathMeshGenerator;
using Debug = UnityEngine.Debug;
using Tile = Zoo.Systems.World.Tile;

public class PathComponent : MonoBehaviour
{
    

    public GameAsset PathAssetType
    {
        get => AssetManager.GetAssetByStringID(ParentPathData.AssetTypeID);
        set
        {
            ParentPathData.AssetTypeID = value.AssetStringID;
            UpdatePathType();
        }
    }

    public PathData ParentPathData;

    /// <summary>
    /// Neighboring paths, where [0,0] is the bottom left (SW).
    /// </summary>
    public PathData[,] Neighbors = new PathData[,]
    {
        {null, null, null},
        {null, null, null},
        {null, null, null}
    };


    public int NumberOfNeigbours { get; private set; }

    public string CurrentPathCaseKey { get; private set; }

#if UNITY_EDITOR
    /// <summary>
    /// Exists to show path case in editor.
    /// </summary>
    public string PathCaseKeyDebugField;
#endif
    private MeshRenderer _meshRenderer;

    private MeshFilter _meshFilter;

    private BoxCollider _collider;

    internal GameObject PivotPoint;

    void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshFilter = GetComponent<MeshFilter>();
        _collider = GetComponent<BoxCollider>();

    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Finds neighbor paths & updates mesh accordingly.
    /// Also tells neighbors it exists & to update.
    /// </summary>
    public void InitializePath() 
    {
        UpdatePath();
        UpdateNeighbors();
    }

    /// <summary>
    /// Call as part of deleting this path asset.
    /// </summary>
    public void DeletePath()
    {

    }

    /// <summary>
    /// Finds neighboring, connected paths, and register them to this path.
    /// </summary>
    void FindNeighbors()
    {
        // Find all colliding gameObjects that have path components
        // Force physics update before continuing.
        Physics.SyncTransforms();

        var pathsToCheck = Physics.OverlapBox(transform.TransformPoint(_collider.center), _collider.size / 2)
            .Where(c => c.gameObject.GetComponentInChildren<PathComponent>())
            .Select(g => g.GetComponentInChildren<PathComponent>().ParentPathData).ToList();
        //Debug.Log($"Checking Parent pos {ParentPathData.Position}; ID is {ParentPathData.AssetID}; Parent GO name {ParentPathData.GameObject.name}");
        //Debug.Log($"pathstoCheck count {pathsToCheck.Count}");
        //foreach (var path in pathsToCheck)
        //{
        //    Debug.Log($"Possible neighbor pos {path.Position}; ID is {path.AssetID}; GO name {path.GameObject.name}");
        //    Debug.Log($"Possible neighbor pos2 {path.PathComponent.PivotPoint.transform.position}; ID is {path.AssetID}; GO name {path.GameObject.name}");
        //}
        pathsToCheck = pathsToCheck.Where(p => p.AssetID != ParentPathData.AssetID).ToList();
        //Debug.Log("Parent path should be removed...");
        //Debug.Log($"pathstoCheck count {pathsToCheck.Count}");
        if (pathsToCheck.Count == 0)
        {
            return;
        }
        //foreach (var path in pathsToCheck)
        //{
        //    Debug.Log($"Possible neighbor pos {path.Position}; ID is {path.AssetID}; GO name {path.GameObject.name} ");
        //}
        float parentX = PivotPoint.transform.position.x, parentZ = PivotPoint.transform.position.z;
        //Debug.Log($"Parent Pos is {parentX},{parentZ}");
        // Check for each possible neighbor
        // // Find SW/NW/NE/SE -- These can be ruled out by the same criteria.
        var candidatesSW = pathsToCheck.Where(p => (p.PathComponent.PivotPoint.transform.position.x == parentX - 0.5f)
                                                   && (p.PathComponent.PivotPoint.transform.position.z == parentZ - 0.5f)).ToList();
        var candidatesNW = pathsToCheck.Where(p => (p.PathComponent.PivotPoint.transform.position.x == parentX - 0.5f)
                                                   && (p.PathComponent.PivotPoint.transform.position.z == parentZ + 0.5f)).ToList();
        var candidatesNE = pathsToCheck.Where(p => (p.PathComponent.PivotPoint.transform.position.x == parentX + 0.5f)
                                                   && (p.PathComponent.PivotPoint.transform.position.z == parentZ + 0.5f)).ToList();
        var candidatesSE = pathsToCheck.Where(p => (p.PathComponent.PivotPoint.transform.position.x == parentX + 0.5f)
                                                  && (p.PathComponent.PivotPoint.transform.position.z == parentZ - 0.5f)).ToList();
  
        //if (candidatesSW.Count > 0) Debug.Log($"SW count {candidatesSW.Count}");
        //if (candidatesNW.Count > 0) Debug.Log($"NW count {candidatesNW.Count}");
        //if (candidatesNE.Count > 0) Debug.Log($"NE count {candidatesNE.Count}");
        //if (candidatesSE.Count > 0) Debug.Log($"SE count {candidatesSE.Count}");

        if (candidatesSW.Count > 0)
            Neighbors[0, 0] = !candidatesSW[0].IsSlope ? candidatesSW[0] : null;
        else
            Neighbors[0, 0] = null;

        if (candidatesNW.Count > 0)
            Neighbors[0, 2] = !candidatesNW[0].IsSlope ? candidatesNW[0] : null;
        else
            Neighbors[0, 2] = null;

        if (candidatesNE.Count > 0)
            Neighbors[2, 2] = !candidatesNE[0].IsSlope ? candidatesNE[0] : null;
        else
            Neighbors[2, 2] = null;

        if (candidatesSE.Count > 0)
            Neighbors[2, 0] = !candidatesSE[0].IsSlope ? candidatesSE[0] : null;
        else
            Neighbors[2, 0] = null;

        // Find W/N/E/S
        var candidatesW = pathsToCheck.Where(p => (p.PathComponent.PivotPoint.transform.position.x == parentX - 0.5f)
                                                   && (p.PathComponent.PivotPoint.transform.position.z == parentZ)).ToList();
        var candidatesN = pathsToCheck.Where(p => (p.PathComponent.PivotPoint.transform.position.x == parentX)
                                                   && (p.PathComponent.PivotPoint.transform.position.z == parentZ + 0.5f)).ToList();
        var candidatesE = pathsToCheck.Where(p => (p.PathComponent.PivotPoint.transform.position.x == parentX + 0.5f)
                                                   && (p.PathComponent.PivotPoint.transform.position.z == parentZ)).ToList();
        var candidatesS = pathsToCheck.Where(p => (p.PathComponent.PivotPoint.transform.position.x == parentX)
                                                   && (p.PathComponent.PivotPoint.transform.position.z == parentZ - 0.5f)).ToList();

        //if (candidatesW.Count > 0) Debug.Log($"W count {candidatesW.Count}");
        //if (candidatesN.Count > 0) Debug.Log($"N count {candidatesN.Count}");
        //if (candidatesE.Count > 0) Debug.Log($"E count {candidatesE.Count}");
        //if (candidatesS.Count > 0) Debug.Log($"S count {candidatesS.Count}");

        // Check each
        Neighbors[0, 1] = candidatesW.Count > 0 ? CheckIfValidCardinalNeighbor("W", candidatesW[0]) ? candidatesW[0] : null : null;

        Neighbors[1, 2] = candidatesN.Count > 0 ? CheckIfValidCardinalNeighbor("N", candidatesN[0]) ? candidatesN[0] : null : null;

        Neighbors[2, 1] = candidatesE.Count > 0 ? CheckIfValidCardinalNeighbor("E", candidatesE[0]) ? candidatesE[0] : null : null;

        Neighbors[1, 0] = candidatesS.Count > 0 ? CheckIfValidCardinalNeighbor("S", candidatesS[0]) ? candidatesS[0] : null : null;


        NumberOfNeigbours = 0;
        NumberOfNeigbours += Neighbors[0, 0] != null ? 1 : 0;
        NumberOfNeigbours += Neighbors[1, 0] != null ? 1 : 0;
        NumberOfNeigbours += Neighbors[2, 0] != null ? 1 : 0;
        NumberOfNeigbours += Neighbors[0, 1] != null ? 1 : 0;
        NumberOfNeigbours += Neighbors[2, 1] != null ? 1 : 0;
        NumberOfNeigbours += Neighbors[0, 2] != null ? 1 : 0;
        NumberOfNeigbours += Neighbors[1, 2] != null ? 1 : 0;
        NumberOfNeigbours += Neighbors[2, 2] != null ? 1 : 0;

        //Debug.Log($"Tile at {ParentPathData.Position} has {NumberOfNeigbours} neighbors");
        
        //Debug.Log(NumberOfNeigbours);

    }

    /// <summary>
    /// Returns bit mask representing number & combination of neighbors.
    /// </summary>
    /// <returns></returns>
    private int GetNeighborsIntMask()
    {

        // Add each 8 neighbor tiles clockwise, starting at N.
        var mask = Neighbors[1, 2] != null ? 1 : 0;   // N  = 1
        mask += Neighbors[2, 2] != null ? 2 : 0;      // NE = 2
        mask += Neighbors[2, 1] != null ? 4 : 0;      // E  = 4
        mask += Neighbors[2, 0] != null ? 8 : 0;     // SE = 8
        mask += Neighbors[1, 0] != null ? 16 : 0;    // S  = 16
        mask += Neighbors[0, 0] != null ? 32 : 0;   // SW = 32
        mask += Neighbors[0, 1] != null ? 64 : 0;    // W  = 64
        mask += Neighbors[0, 2] != null ? 128 : 0;   // NW = 128
        return mask;

    }

    /// <summary>
    /// Returns true if path in given direction is a valid neighbor to this path.
    /// </summary>
    /// <param name="directionOfNeighbor"></param>
    /// <returns></returns>
    bool CheckIfValidCardinalNeighbor(string directionOfNeighbor, PathData pathDataToCheck)
    {
        var pathToCheck = pathDataToCheck;
       // Debug.Log($"Got here with {pathDataToCheck.GameObject.name}");
        switch (directionOfNeighbor)
        {
            case "N":
                {
                    // If both are flat...
                    if (!pathToCheck.IsSlope && !ParentPathData.IsSlope)
                    {
                        //Debug.Log($"Flat check: {pathToCheck.BottomHeight} == {ParentPathData.BottomHeight}?");
                        return pathToCheck.BottomHeight == ParentPathData.BottomHeight;
                    }
                    // If not, do lots of annoying shit
                    // Check for slope compatibility...
                    if (pathToCheck.IsSlope)
                    {
                        // If facing is not correct, set to null.
                        switch (pathToCheck.Facing)
                        {
                            case PathFacing.North:
                                {
                                    // If the parent is also a slope..
                                    if (ParentPathData.IsSlope && ParentPathData.Facing == PathFacing.South)
                                    {
                                        if (pathToCheck.BottomHeight != ParentPathData.BottomHeight) return false;
                                    }
                                    else if (ParentPathData.IsSlope && ParentPathData.Facing == PathFacing.North)
                                    {
                                        if (pathToCheck.BottomHeight != ParentPathData.TopHeight) return false;
                                    }
                                    else if (!ParentPathData.IsSlope)
                                    {
                                        if (pathToCheck.BottomHeight != ParentPathData.BottomHeight) return false;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                    break;
                                }
                            case PathFacing.South:
                                {
                                    // If the parent is also a slope..
                                    if (ParentPathData.IsSlope && ParentPathData.Facing == PathFacing.South)
                                    {
                                        if (pathToCheck.TopHeight != ParentPathData.BottomHeight) return false;
                                    }
                                    else if (ParentPathData.IsSlope && ParentPathData.Facing == PathFacing.North)
                                    {
                                        if (pathToCheck.TopHeight != ParentPathData.TopHeight) return false;
                                    }
                                    else if (!ParentPathData.IsSlope)
                                    {
                                        if (pathToCheck.TopHeight != ParentPathData.BottomHeight) return false;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                    break;
                                }
                            case PathFacing.East:
                            case PathFacing.West:
                            default: return false;
                        }
                    }
                    // If not a slope  And parent is a slope..
                    else if (ParentPathData.IsSlope)
                    {
                        switch (ParentPathData.Facing)
                        {
                            case PathFacing.North:
                                {
                                    if (pathToCheck.BottomHeight != ParentPathData.TopHeight) return false;
                                    break;
                                }
                            case PathFacing.South:
                                {
                                    if (pathToCheck.BottomHeight != ParentPathData.BottomHeight) return false;
                                    break;
                                }
                            case PathFacing.East:
                            case PathFacing.West:
                            default: return false;
                        }

                    }
                    break;
                }
            case "W":
                {
                    // Check W tile

                    // If both are flat...
                    if (!pathToCheck.IsSlope && !ParentPathData.IsSlope)
                    {
                        return pathToCheck.BottomHeight == ParentPathData.BottomHeight;
                    }
                    // If not, do lots of annoying shit
                    // Check for slope compatibility...
                    if (pathToCheck.IsSlope)
                    {
                        // If facing is not correct, set to null.
                        switch (pathToCheck.Facing)
                        {
                            case PathFacing.West:
                                {
                                    // If the parent is also a slope..
                                    if (ParentPathData.IsSlope && ParentPathData.Facing == PathFacing.East)
                                    {
                                        if (pathToCheck.BottomHeight != ParentPathData.BottomHeight) return false;
                                    }
                                    else if (ParentPathData.IsSlope && ParentPathData.Facing == PathFacing.West)
                                    {
                                        if (pathToCheck.BottomHeight != ParentPathData.TopHeight) return false;
                                    }
                                    else if (!ParentPathData.IsSlope)
                                    {
                                        if (pathToCheck.TopHeight != ParentPathData.BottomHeight) return false;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                    break;
                                }
                            case PathFacing.East:
                                {
                                    // If the parent is also a slope..
                                    if (ParentPathData.IsSlope && ParentPathData.Facing == PathFacing.East)
                                    {
                                        if (pathToCheck.TopHeight != ParentPathData.BottomHeight) return false;
                                    }
                                    else if (ParentPathData.IsSlope && ParentPathData.Facing == PathFacing.West)
                                    {
                                        if (pathToCheck.TopHeight != ParentPathData.TopHeight) return false;
                                    }
                                    else if (!ParentPathData.IsSlope)
                                    {
                                        if (pathToCheck.TopHeight != ParentPathData.BottomHeight) return false;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                    break;
                                }
                            case PathFacing.North:
                            case PathFacing.South:
                            default: return false;
                        }
                    }
                    // If not a slope  And parent is a slope..
                    else if (ParentPathData.IsSlope)
                    {
                        switch (ParentPathData.Facing)
                        {
                            case PathFacing.West:
                                {
                                    if (pathToCheck.BottomHeight != ParentPathData.TopHeight) return false;
                                    break;
                                }
                            case PathFacing.East:
                                {
                                    if (pathToCheck.BottomHeight != ParentPathData.BottomHeight) return false;
                                    break;
                                }
                            case PathFacing.North:
                            case PathFacing.South:
                            default: return false;
                        }

                    }

                    break;

                }
            case "S":
                {
                    // Check S tile
                    // If both are flat...
                    if (!pathToCheck.IsSlope && !ParentPathData.IsSlope)
                    {
                        return pathToCheck.BottomHeight == ParentPathData.BottomHeight;
                    }
                    // If not, do lots of annoying shit
                    // Check for slope compatibility...
                    if (pathToCheck.IsSlope)
                    {
                        // If facing is not correct, set to null.
                        switch (pathToCheck.Facing)
                        {
                            case PathFacing.North:
                                {
                                    // If the parent is also a slope..
                                    if (ParentPathData.IsSlope && ParentPathData.Facing == PathFacing.South)
                                    {
                                        if (pathToCheck.TopHeight != ParentPathData.TopHeight) return false;
                                    }
                                    else if (ParentPathData.IsSlope && ParentPathData.Facing == PathFacing.North)
                                    {
                                        if (pathToCheck.TopHeight != ParentPathData.BottomHeight) return false;
                                    }
                                    else if (!ParentPathData.IsSlope)
                                    {
                                        if (pathToCheck.TopHeight != ParentPathData.BottomHeight) return false;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                    break;
                                }
                            case PathFacing.South:
                                {
                                    // If the parent is also a slope..
                                    if (ParentPathData.IsSlope && ParentPathData.Facing == PathFacing.South)
                                    {
                                        if (pathToCheck.BottomHeight != ParentPathData.TopHeight) return false;
                                    }
                                    else if (ParentPathData.IsSlope && ParentPathData.Facing == PathFacing.North)
                                    {
                                        if (pathToCheck.BottomHeight != ParentPathData.BottomHeight) return false;
                                    }
                                    else if (!ParentPathData.IsSlope)
                                    {
                                        if (pathToCheck.TopHeight != ParentPathData.BottomHeight) return false;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                    break;
                                }
                            case PathFacing.East:
                            case PathFacing.West:
                            default: return false;
                        }
                    }
                    // If not a slope  And parent is a slope..
                    else if (ParentPathData.IsSlope)
                    {
                        switch (ParentPathData.Facing)
                        {
                            case PathFacing.North:
                                {
                                    if (pathToCheck.TopHeight != ParentPathData.BottomHeight) return false;
                                    break;
                                }
                            case PathFacing.South:
                                {
                                    if (pathToCheck.BottomHeight != ParentPathData.BottomHeight) return false;
                                    break;
                                }
                            case PathFacing.East:
                            case PathFacing.West:
                            default: return false;
                        }

                    }
                    break;
                }
            case "E":
                {
                    // Check E tile


                    // If both are flat...
                    if (!pathToCheck.IsSlope && !ParentPathData.IsSlope)
                    {
                        return pathToCheck.BottomHeight == ParentPathData.BottomHeight;
                    }
                    // If not, do lots of annoying shit
                    // Check for slope compatibility...
                    if (pathToCheck.IsSlope)
                    {
                        // If facing is not correct, set to null.
                        switch (pathToCheck.Facing)
                        {
                            case PathFacing.West:
                                {
                                    // If the parent is also a slope..
                                    if (ParentPathData.IsSlope && ParentPathData.Facing == PathFacing.West)
                                    {
                                        if (pathToCheck.TopHeight != ParentPathData.BottomHeight) return false;
                                    }
                                    else if (ParentPathData.IsSlope && ParentPathData.Facing == PathFacing.East)
                                    {
                                        if (pathToCheck.TopHeight != ParentPathData.TopHeight) return false;
                                    }
                                    else if (!ParentPathData.IsSlope)
                                    {
                                        if (pathToCheck.TopHeight != ParentPathData.BottomHeight) return false;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                    break;
                                }
                            case PathFacing.East:
                                {
                                    // If the parent is also a slope..
                                    if (ParentPathData.IsSlope && ParentPathData.Facing == PathFacing.West)
                                    {
                                        if (pathToCheck.BottomHeight != ParentPathData.BottomHeight) return false;
                                    }
                                    else if (ParentPathData.IsSlope && ParentPathData.Facing == PathFacing.East)
                                    {
                                        if (pathToCheck.BottomHeight != ParentPathData.TopHeight) return false;
                                    }
                                    else if (!ParentPathData.IsSlope)
                                    {
                                        if (pathToCheck.BottomHeight != ParentPathData.BottomHeight) return false;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                    break;
                                }
                            case PathFacing.North:
                            case PathFacing.South:
                            default: return false;
                        }
                    }
                    // If not a slope  And parent is a slope..
                    else if (ParentPathData.IsSlope)
                    {
                        switch (ParentPathData.Facing)
                        {
                            case PathFacing.East:
                                {
                                    if (pathToCheck.BottomHeight != ParentPathData.TopHeight) return false;
                                    break;
                                }
                            case PathFacing.West:
                                {
                                    if (pathToCheck.BottomHeight != ParentPathData.BottomHeight) return false;
                                    break;
                                }
                            case PathFacing.North:
                            case PathFacing.South:
                            default: return false;
                        }

                    }

                    break;
                }
        }

        return true;
    }

    /// <summary>
    /// Trigger mesh & neighbor updates on all neighbor paths.
    /// </summary>
    public void UpdateNeighbors(bool updateNeighbors = false)
    {
        //Debug.Log($"Checking for neighbors of Parent {this.ParentPathData.Position}; ID of {this.ParentPathData.AssetID}; Parent GO name {this.ParentPathData.GameObject.name}");
        //Debug.Log($"Num neighbors {NumberOfNeigbours}");
        foreach (var neighbor in Neighbors)
        {
            if (neighbor == null) continue;
           // Debug.Log($"Got here; Neighbor Pos is {neighbor.Position}; ID is {neighbor.AssetID}; GO name {neighbor.GameObject.name}");
            //neighbor.PathComponent.FindNeighbors();
            neighbor.PathComponent.UpdatePath();
            if (updateNeighbors)
                neighbor.PathComponent.UpdateNeighbors();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void UpdatePath()
    {
        FindNeighbors();
        //if (!ParentPathData.IsSlope)
        //{
        //    UpdatePathMesh();
        //}
        //else
        //{
        //    UpdateSlopeMesh();
        //}
        UpdatePathMesh();

        // Handle elevated/tunnel/etc here if applicable
        if (ParentPathData.IsOnTerrain)
        {
            // Do stuff here
        }
        else
        {
            // Do diff stuff here
        }

        //UpdateNeighbors();
    }


    /// <summary>
    /// Call when path type changes.
    /// </summary>
    private void UpdatePathType()
    {

    }
    
    /// <summary>
    /// Updates path mesh based on slope grade & facing.
    /// </summary>
    private void UpdateSlopeMesh()
    {
        var meshKey = ParentPathData.SlopeHeightDifference > Tile.MaxSlopeGrade / 2 ? "stairs" : "slope";
        _meshFilter.mesh = PathMeshGenerationTool.PathMeshDictionary[meshKey];
        PivotPoint.transform.rotation = GetMeshRotation(ParentPathData.Facing);
        // if we modify the mesh at runtime like a crazy mofo, we do it here.
    }

    /// <summary>
    /// Finds & updates path with new mesh type. Should only be used if flat.
    /// </summary>
    private void UpdatePathMesh()
    {
        var intMask = GetNeighborsIntMask();
        CurrentPathCaseKey = GetMeshKey(intMask);
        PathCaseKeyDebugField = CurrentPathCaseKey;
        _meshFilter.mesh = PathMeshGenerationTool.PathMeshDictionary[CurrentPathCaseKey];
        PivotPoint.transform.rotation = GetMeshRotation(intMask);
        //transform.rotation = GetMeshRotation(intMask);
    }

    /// <summary>
    /// Returns the mesh key for this tile based on neighbours.
    /// </summary>
    /// <returns></returns>
    private static string GetMeshKey(int neighborsIntMask)
    {
        //Debug.Log($"Setting mask to {neighborsIntMask}");
        

        switch (neighborsIntMask)
        {
            case 0: return "single";
            // End
            // If North
            case 11:
            case 161:
            case 1:
            case 129:
            case 33:
            case 9:
            case 3:
            case 169:
            case 43:
            case 171:
            case 41:
            case 139:
            case 163:
            case 131:
            // If South
            case 26:
            case 176:
            case 16:
            case 144:
            case 48:
            case 24:
            case 18:
            case 184:
            case 58:
            case 186:
            case 56:
            case 154:
            case 178:
            case 146:
            // If West
            case 224:
            case 74:
            case 64:
            case 192:
            case 96:
            case 72:
            case 66:
            case 232:
            case 106:
            case 234:
            case 104:
            case 202:
            case 226:
            case 194:
            // If East
            case 164:
            case 14:
            case 132:
            case 36:
            case 4:
            case 12:
            case 6:
            case 172:
            case 46:
            case 174:
            case 44:
            case 142:
            case 166:
            case 134: return "end";
            // End
            // N/S
            case 27:
            case 177:
            case 17:
            case 145:
            case 49:
            case 25:
            case 19:
            case 185:
            case 59:
            case 187:
            case 57:
            case 155:
            case 179:
            case 147:
            case 153:
            case 51:
            // E/W
            case 78:
            case 228:
            case 68:
            case 196:
            case 100:
            case 76:
            case 70:
            case 236:
            case 110:
            case 238:
            case 108:
            case 206:
            case 230:
            case 102:
            case 204:
            case 198: return "straight";
            // Corners
            // N/E
            case 5:
            case 133:
            case 37:
            case 13:
            case 173:
            case 45:
            // E/S
            case 20:
            case 148:
            case 52:
            case 22:
            case 182:
            case 150:
            // S/W
            case 80:
            case 208:
            case 88:
            case 82:
            case 218:
            case 210:
            // W/N
            case 65:
            case 97:
            case 73:
            case 67:
            case 107:
            case 105: return "corner";
            // Full Corners
            // N/E
            case 7:
            case 47:
            case 167:
            case 135:
            case 39:
            // E/S
            case 28:
            case 188:
            case 60:
            case 158:
            case 156:
            // S/W
            case 112:
            case 122:
            case 120:
            case 242:
            case 114:
            // W/N
            case 193:
            case 233:
            case 203:
            case 195:
            case 201:
            case 165: return "fullcorner";
            // T Junction
            // W
            case 81:
            case 89:
            case 83:
            case 91:
            // N
            case 69:
            case 101:
            case 109:
            case 77:
            // E
            case 21:
            case 149:
            case 53:
            case 181:
            // S
            case 84:
            case 212:
            case 214:
            case 86: return "tjunction";
            // Quarter T Junction Top
            // W
            case 113:
            case 123:
            case 121:
            case 115:
            // N
            case 197:
            case 237:
            case 229:
            case 205:
            // E
            case 23:
            case 183:
            case 151:
            case 55:
            // S
            case 92:
            case 222:
            case 220:
            case 94: return "quartertjunctiontop";
            // Quarter T Bottom
            // W
            case 209:
            case 219:
            case 211:
            case 217:
            // N
            case 71:
            case 111:
            case 79:
            case 103:
            // E
            case 29:
            case 189:
            case 61:
            case 157:
            // S
            case 116:
            case 244:
            case 246:
            case 118: return "quartertjunctionbottom";
            // Half T Junction
            // W
            case 241:
            case 251:
            case 249:
            case 243:
            // N
            case 199:
            case 231:
            case 207:
            case 239:
            // E
            case 31:
            case 63:
            case 191:
            case 159:
            // S
            case 252:
            case 126:
            case 254:
            case 124: return "halftjunction";
            // Curved
            // N/E
            case 143:
            case 175:
            // E/S
            case 62:
            case 190:
            // S/W
            case 248:
            case 250:
            // W/N
            case 227:
            case 235: return "outsidecorner";
            // Four way
            case 85: return "fourway";
            // Quarter four ways
            // N/E
            case 87:
            // E/S
            case 93:
            // S/W
            case 117:
            // W/N
            case 213: return "quarterfourway";
            // S
            // Half four ways
            // W
            case 95:
            // N
            case 215:
            // E
            case 245:
            // S
            case 125: return "halffourway";
            // diagonal four ways
            // SW/NE
            case 221:
            // NW/SE
            case 119: return "diagfourway";
            // Three quarter four ways
            // N/E
            case 223:
            // E/S
            case 127:
            // S/W
            case 253:
            // W/N
            case 247: return "3quarterfourway";
            // Full
            case 255: return "full";
            default:
            {
                return "single";
                }
        }
    }

    /// <summary>
    /// Returns the rotation for this tile based on neighbors.
    /// </summary>
    /// <returns></returns>
    private static Quaternion GetMeshRotation(int neighborsIntMask)
    {
        switch (neighborsIntMask)
        {

            // If North
            // End
            case 11:
            case 161:
            case 1:
            case 129:
            case 33:
            case 9:
            case 3:
            case 169:
            case 43:
            case 171:
            case 41:
            case 139:
            case 163:
            case 131:
            // Straight
            case 27:
            case 177:
            case 17:
            case 145:
            case 49:
            case 25:
            case 19:
            case 185:
            case 59:
            case 187:
            case 57:
            case 155:
            case 179:
            case 147:
            case 153:
            case 51:
            // Corner
            case 5:
            case 133:
            case 37:
            case 13:
            case 173:
            case 45:
            // Full Corner
            case 7:
            case 47:
            case 167:
            case 135:
            case 39:
            // T Junction
            case 69:
            case 101:
            case 109:
            case 77:
            // Quarter T Top
            case 197:
            case 237:
            case 229:
            case 205:
            // Quarter T Bottom
            case 71:
            case 111:
            case 79:
            case 103:
            // Half T
            case 199:
            case 231:
            case 207:
            case 239:
            // 3 Quarter T

            // Curved
            case 62:
            case 190:

            // Quarter 4 way
            case 87:
            // Half 4 way
            case 215:
            // Diag 4 way
            case 119:
            // 3 quarter 4 way
            case 223: return Quaternion.Euler(0, 90, 0);
            // If South
            // End
            case 26:
            case 176:
            case 16:
            case 144:
            case 48:
            case 24:
            case 18:
            case 184:
            case 58:
            case 186:
            case 56:
            case 154:
            case 178:
            case 146:
            // Straight -- done in North
            // Corner
            case 80:
            case 208:
            case 88:
            case 82:
            case 218:
            case 210:
            // Full Corner
            case 112:
            case 122:
            case 120:
            case 242:
            case 114:
            // T Junction
            case 84:
            case 212:
            case 214:
            case 86:
            // Quarter T top
            case 92:
            case 222:
            case 220:
            case 94:
            // Quarter T bottom
            case 116:
            case 244:
            case 246:
            case 118:
            // Half T
            case 252:
            case 126:
            case 254:
            case 124:
            // 3 Quarter T

            // Curved
            case 227:
            case 235:
            // Quarter 4 way
            case 117:
            // Half 4 way
            case 125:
            // 3 quarter 4 way
            case 253: return Quaternion.Euler(0, 270, 0);
            // If West
            // End
            case 224:
            case 74:
            case 64:
            case 192:
            case 96:
            case 72:
            case 66:
            case 232:
            case 106:
            case 234:
            case 104:
            case 202:
            case 226:
            case 194:
            // Straight
            case 78:
            case 228:
            case 68:
            case 196:
            case 100:
            case 76:
            case 70:
            case 236:
            case 110:
            case 238:
            case 108:
            case 206:
            case 230:
            case 102:
            case 204:
            case 198:
            // Corner
            case 65:
            case 97:
            case 73:
            case 67:
            case 107:
            case 105:
            // Full Corner
            case 193:
            case 233:
            case 203:
            case 195:
            case 201:
            case 165:
            // T Junction
            case 81:
            case 89:
            case 83:
            case 91:
            // Quarter T Top
            case 113:
            case 123:
            case 121:
            case 115:
            // Quarter T Bottom
            case 209:
            case 219:
            case 211:
            case 217:
            // Half T
            case 241:
            case 251:
            case 249:
            case 243:
            // 3 Quarter T

            // Curved
            case 143:
            case 175:
            // Quarter 4 way
            case 213:
            // Half 4 way
            case 95:
            // 3 quarter 4 way
            case 247: return Quaternion.Euler(0, 0, 0);
            // If East
            // End
            case 164:
            case 14:
            case 132:
            case 36:
            case 4:
            case 12:
            case 6:
            case 172:
            case 46:
            case 174:
            case 44:
            case 142:
            case 166:
            case 134:
            // Straight handled in West

            // Corner
            case 20:
            case 148:
            case 52:
            case 22:
            case 182:
            case 150:
            // Full Corner
            case 28:
            case 188:
            case 60:
            case 158:
            case 156:
            // T Junction
            case 21:
            case 149:
            case 53:
            case 181:
            // Quarter T Top
            case 23:
            case 183:
            case 151:
            case 55:
            // Quarter T Bottom
            case 29:
            case 189:
            case 61:
            case 157:
            // Half T
            case 31:
            case 63:
            case 191:
            case 159:
            // 3 Quarter T

            // Curved
            case 248:
            case 250:

            // Quarter 4 way
            case 93:
            // Half 4 way
            case 245:
            // 3 quarter 4 way
            case 127: return Quaternion.Euler(0, 180, 0);

            default:
                {
                    return Quaternion.Euler(0, 0, 0);
                }
        }
    }

    /// <summary>
    /// Return rotation based on facing.
    /// </summary>
    /// <param name="facing"></param>
    /// <returns></returns>
    private static Quaternion GetMeshRotation(PathFacing facing)
    {
        switch (facing)
        {
            case PathFacing.East:
            {
                return Quaternion.Euler(0, 90, 0);
            }
            case PathFacing.West:
            {
                return Quaternion.Euler(0, 270, 0);
            }
            case PathFacing.North:  
            {
                return Quaternion.Euler(0,0,0);
            }
            case PathFacing.South:
            {
                return Quaternion.Euler(0, 180, 0);
            }
            case PathFacing.Flat:
            {
                Debug.Log("Wtf this is supposed to be a slope");
                return Quaternion.Euler(0, 0, 0);
                }
            default:
            {
                return Quaternion.Euler(0, 0, 0);
                }
        }

    }

    private void SetSlopeAngle()
    {
        var mesh = _meshFilter.mesh;
        //mesh.vertices[2].y = 
    }

    public void DestroyPathComponent()
    {
        Destroy(PivotPoint);
        Destroy(this);
        UpdateNeighbors();
    }

}
