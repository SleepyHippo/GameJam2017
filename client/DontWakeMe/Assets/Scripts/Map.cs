// /********************************************************************
//   filename:  Map.cs
//   author:    Duke
//   date:      2017/06/17
// 
//   purpose:   
// *********************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace DWM {
    public enum CellType {
//        NULL = 0,
//        Dirt = 1,
        Root = 2,
        Branch = 3,
        Ladder = 4,
        Item = 5,
    }

    [Serializable]
    public class Cell {
        public int x;
        public int y;

        /// <summary>
        /// 格子的类型
        /// </summary>
        public CellType type;

        /// <summary>
        /// nodeId，用于对应Root和Branch，构造函数时赋值，不要修改！
        /// </summary>
        public int nodeId;

        /// <summary>
        /// 树枝id，[1,4]
        /// </summary>
        public int branchId;

        /// <summary>
        /// 树枝中的组id，[1, infinite)
        /// </summary>
        public int groupId;

        /// <summary>
        /// 这个Cell的价值
        /// </summary>
        public int value;

        /// <summary>
        /// 当前的生命值，最大100，0表示被挖断了
        /// </summary>
        public int hp;

        public Cell(Cell other) : this(other.x, other.y, other.type, other.branchId, other.groupId, other.hp, other.value) { }

        public Cell(int _x, int _y, CellType _type, int _branchId, int _groupId, int _hp, int _value) {
            Set(_x, _y, _type, _branchId, _groupId, _hp, _value);
        }

        public void Set(int _x, int _y, CellType _type, int _branchId, int _groupId, int _hp, int _value) {
            this.x = _x;
            this.y = _y;
            this.type = _type;
            nodeId = _branchId * 10000 + _groupId;
            branchId = _branchId;
            this.groupId = _groupId;
            //            this.owner = _owner;
            this.hp = _hp;
            this.value = _value;
        }
    }

    public class Map {
        public int width;
        public int height;
        public int landHeight;
        public List<Cell> cells = new List<Cell>();
        public Dictionary<int, Cell> cellIndexMap = new Dictionary<int, Cell>();

        public Tree upTree;
        public Tree botTree;

        public Map(MapData _data) {
            if (_data != null) {
                Deserilize(_data);
            }
        }

        public void InitTree() {
            upTree = new Tree();
            botTree = new Tree();
            for (int i = 0; i < cells.Count; ++i) {
                Cell cell = cells[i];
                if (cell.y > landHeight) {
                    AddCellToTree(cell, upTree);
                }
                else if (cell.y < landHeight) {
                    AddCellToTree(cell, botTree);
                }
            }
            SetTreeBranchGroupMap(upTree);
            SetTreeBranchGroupMap(botTree);
        }

        void AddCellToTree(Cell _cell, Tree _tree) {
            while (_tree.branchs.Count < _cell.branchId) {
                _tree.branchs.Add(new Branch());
            }
            Branch branch = _tree.branchs[_cell.branchId - 1];
            branch.branchId = _cell.branchId;

            while (branch.groups.Count < _cell.groupId) {
                branch.groups.Add(new Group());
            }
            Group group = branch.groups[_cell.groupId];
            group.branchGroupId = _cell.nodeId;
            group.groupId = _cell.groupId;
            group.value = _cell.value;
            group.hp = _cell.hp;
            group.cells.Add(_cell);
        }

        void SetTreeBranchGroupMap(Tree _tree) {
            _tree.branchGroupMap.Clear();
            for (int i = 0; i < _tree.branchs.Count; ++i) {
                Branch branch = _tree.branchs[i];
                for (int j = 0; j < branch.groups.Count; ++j) {
                    Group group = branch.groups[j];
                    _tree.branchGroupMap.Add(group.branchGroupId, group);
                }
            }
        }

        public Group AddHp(int _x, int _y, int _hp) {
            Cell cell;
            if (cellIndexMap.TryGetValue(GetCellIndex(_x, _y), out cell)) {
                if (cell.y > landHeight) {
                    Group group = upTree.branchGroupMap[cell.nodeId];
                    group.AddHp(_hp);
                    return group;
                }
                else if (cell.y < landHeight) {
                    Group group = botTree.branchGroupMap[cell.nodeId];
                    group.AddHp(_hp);
                    return group;
                }
            }
            return null;
        }

//        public Map(int _faceWidth, int _height, int _faceCount = 4) {
//            faceWidth = _faceWidth;
//            height = _height;
//            faceCount = _faceCount;
//            width = faceWidth * _faceCount;
//            landHeight = _height / 2;
//            cells = new Cell[width * height];
//        }

        public void Deserilize(MapData _data) {
            this.width = _data.width;
            this.height = _data.height;
            this.landHeight = _data.height / 2;

//            cells = new Cell[width * height];
            cells.Clear();
            cellIndexMap.Clear();
            for (int i = 0; i < _data.cells.Count; ++i) {
                Cell cell = _data.cells[i];
                Cell newCell = new Cell(cell);
                cells.Add(newCell);
                cellIndexMap.Add(GetCellIndex(newCell.x, newCell.y), newCell);
            }
        }

//        public MapData Serilize() {
//            MapData data = ScriptableObject.CreateInstance<MapData>();
//            data.width = width;
//            data.height = height;
//            data.cells = cells;
//            return data;
//        }

        public void SetCell(int _x, int _y, CellType _type, int _branchId, int _groupId, int _hp, int _value) {
            if (_x < 0 || _x >= width || _y < 0 || _y >= height) {
                Debug.LogError("Cell out of range: " + _x + "  " + _y);
                return;
            }
            int index = GetCellIndex(_x, _y);
            Cell cell;
            if (!cellIndexMap.TryGetValue(index, out cell)) {
                cell = new Cell(_x, _y, _type, _branchId, _groupId, _hp, _value);
                cells.Add(cell);
                cellIndexMap.Add(index, cell);
            }
            else {
                cell.Set(_x, _y, _type, _branchId, _groupId, _hp, _value);
            }
//            cells[GetCellIndex(_x, _y)] = cell;
        }

        public void RemoveCell(int _x, int _y) {
            if (_x < 0 || _x >= width || _y < 0 || _y >= height) {
                Debug.LogError("Cell out of range: " + _x + "  " + _y);
                return;
            }
            int index = GetCellIndex(_x, _y);
            if (cellIndexMap.ContainsKey(index)) {
                cells.Remove(cellIndexMap[index]);
                cellIndexMap.Remove(index);
            }
        }

        public Cell GetCell(int _x, int _y) {
            if (_x < 0 || _x >= width || _y < 0 || _y >= height) {
                Debug.LogError("Cell out of range: " + _x + "  " + _y);
                return null;
            }
            int index = GetCellIndex(_x, _y);
            Cell cell = null;
            cellIndexMap.TryGetValue(index, out cell);
            return cell;
        }

//        public bool IsCellEmpty(int _x, int _y) {
//            return GetCell(_x, _y) == null;
//        }

//        public int GetFaceIndex(int _x) {
//            return _x / faceWidth;
//        }

        public int GetCellIndex(int _x, int _y) {
            return _x + _y * width;
        }

        public int GetX(int _cellIndex) {
            return _cellIndex % width;
        }

        public int GetY(int _cellIndex) {
            return _cellIndex / width;
        }
    }
}