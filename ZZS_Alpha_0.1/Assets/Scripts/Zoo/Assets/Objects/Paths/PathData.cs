using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zoo.Assets;
using Zoo.Systems.World;

namespace Zoo.Assets.Objects.Paths
{
    /// <summary>
    /// Facing of a path. For slopes, facing direction is the upper side. If flat, no slope or facing.
    /// </summary>
    public enum PathFacing
    {
        North, South, East, West, Flat
    }

    public class PathData : BuildableObjectData
    {

        [Newtonsoft.Json.JsonIgnore]
        public PathComponent PathComponent;

        /// <summary>
        /// Child object holding the path component. Must always be called "path."
        /// </summary>
        private GameObject _pathGameObject;

        /// <summary>
        /// If true, path is full width version of its type.
        /// </summary>
        public bool FullWidth = false;

        /// <summary>
        /// String keys of any slope cases.
        /// </summary>
        public static readonly string[] SlopeCases = {"slope", "stairs"};

        /// <summary>
        /// True if path type case is a slope case.
        /// </summary>
        public bool IsSlope { get; private set; }

        /// <summary>
        /// True if path is flush with the terrain tile it is built on.
        /// Otherwise, it is elevated or a tunnel.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public bool IsOnTerrain { get; private set; }

        /// <summary>
        /// Difference in height between top & bottom of tile.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public float SlopeHeightDifference => IsSlope ? TopHeight - BottomHeight : 0;

        /// <summary>
        /// Facing of the path. For slopes, facing direction is the upper side.
        /// </summary>
        public PathFacing Facing = PathFacing.Flat;


        //public override Vector3 Position
        //{
        //    //get => GameObject == null ? _position : GameObject.transform.position;
        //    get
        //    {
        //        if (GameObject == null || PathComponent == null)
        //        {
        //            return _position - new Vector3(0.25f, 0, 0.25f);
        //        }
        //        else
        //        {
        //            return GameObject.transform.position - new Vector3(0.25f, 0, 0.25f);
        //        }
        //    }
        //    set
        //    {
        //        var newPos = value + new Vector3(0.25f, 0f, 0.25f);
        //        Debug.Log($"orig is {value} modified is {newPos}");
        //        if (GameObject == null || PathComponent == null)
        //        {
        //            _position = newPos;
        //        }
        //        else
        //        {
        //            _position = newPos;
        //            GameObject.transform.position = newPos;
        //        }
        //    }
        //}
        
        //private Vector3 _position;

        public override Vector3 RotationEuler
        {
            //get => GameObject == null ? _rotationEuler : GameObject.transform.rotation.eulerAngles;
            get => new Vector3();
            set
            {
                if (GameObject == null)
                {
                    _rotationEuler = value;
                }
                else
                {
                    _rotationEuler = value;
                    GameObject.transform.rotation = Quaternion.Euler(value);
                }
            }
        }

        private Vector3 _rotationEuler;

        /// <summary>
        /// Tile upon which this path is built.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public Tile ParentTile => TileMapManager.Instance.GetTile(new Vector2(Position.x, Position.z));

        /// <summary>
        /// Height of path. Will return bottom height if slope.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public float PathHeight
        {
            get => _pathHeight[0];
            private set => _pathHeight[0] = value;
        }

        /// <summary>
        /// Height of lower side of path, if slope.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public float BottomHeight
        {
            get => _pathHeight[0];
            private set => _pathHeight[0] = value;
        }

        /// <summary>
        /// Height of upper side of path, if slope.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public float TopHeight
        {
            get => _pathHeight[1];
            private set => _pathHeight[1] = value;
        }

        /// <summary>
        /// Height of path in a 2 member array. Index 0 is bottom path height; index 1 is top height.
        /// </summary>
        [Newtonsoft.Json.JsonRequired]
        private float[] _pathHeight = new float[2];


        [Newtonsoft.Json.JsonConstructor]
        public PathData() : base()
        {
        }

        public override void Initialize(GameAsset parentAsset)
        {
            base.Initialize(parentAsset);
            InitializePathComponent();
            RegisterToParentTileEvents();
           // Debug.Log($"Post init pos {Position} & rot {RotationEuler}");
        }
        public override void InitializeFromSave()
        {
            base.InitializeFromSave();
            GameObject.transform.rotation = new Quaternion();
            InitializePathComponent();
            RegisterToParentTileEvents();

        }
        public override void InitializeHologram(GameAsset parentAsset)
        {
            base.InitializeHologram(parentAsset);
            InitializePathComponent();
        }
        private void RegisterToParentTileEvents()
        {
            ParentTile.TileHeightChanged += OnParentTileHeightChanged;
        }

        private void RemoveFromParentTileEvents()
        {
            ParentTile.TileHeightChanged -= OnParentTileHeightChanged;

        }

        /// <summary>
        /// Called any time parent tile height has changed. Should handle
        /// any checks that need to happen when this occurs.
        /// </summary>
        /// <param name="parentTile"></param>
        private void OnParentTileHeightChanged(Tile parentTile)
        {
            Debug.Log("This worked, woah. Sorry, David. I added this, but then" +
                      "decided I didn't want to test it because it was 8:43 PM. 2/7/19," +
                      " if you were wondering. Anyway, this is located in PathData.cs." +
                      "That said, you probably won't see this before I delete it." +
                      "Unless I leave it here.");
        }

        public void SetPathHeight()
        {
            PathHeight = Position.y;
        }

        /// <summary>
        /// Set this path to a slope with the given facing and heights.
        /// </summary>
        public void SetToSlope(PathFacing facing, float bottomHeight, float topHeight)
        {
            IsSlope = true;
            Facing = facing;
            BottomHeight = bottomHeight;
            TopHeight = topHeight;
            PathComponent.InitializePath();
        }

        private void InitializePathComponent()
        {
            if (GameObject == null) return;
            _pathGameObject = GameObject.transform.Find("path").gameObject;
            var component = _pathGameObject.GetComponent<PathComponent>();
            if (component == null)
            {
                Debug.Log("Path component not found. Creating new.");
                component = _pathGameObject.AddComponent<PathComponent>();
            }
            component.ParentPathData = this;
            PathComponent = component;
            PathComponent.PivotPoint = GameObject;
            SetPathHeight();
            PathComponent.InitializePath();
        }

        public override void Destroy()
        {
            base.Destroy();
            PathComponent.DestroyPathComponent();
            RemoveFromParentTileEvents();
        }
    }
}
