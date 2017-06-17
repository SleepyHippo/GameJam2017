// /********************************************************************
//   filename:  Map.cs
//   author:    Duke
//   date:      2017/06/17
// 
//   purpose:   
// *********************************************************************/

using UnityEngine;

namespace DWM {
//    public enum Owner {
//        Down = 0,
//        Up = 1,
//    }

    public struct Cell {
        /// <summary>
        /// 唯一id，用于上下对应，构造函数时赋值，不要修改！
        /// </summary>
        public int id;

        /// <summary>
        /// 树枝id，[1,4]
        /// </summary>
        public int branchId;

        /// <summary>
        /// 树枝中的节点id，[0, infinite)
        /// </summary>
        public int branchCellId;

        /// <summary>
        /// 这个Cell的价值
        /// </summary>
        public int value;

        /// <summary>
        /// 当前的生命值，最大100，0表示被挖断了
        /// </summary>
        public int hp;

        public Cell(int _branchId, int _branchCellId, int _hp, int _value) {
            id = _branchId * 10000 + _branchCellId;
            branchId = _branchId;
            this.branchCellId = _branchCellId;
//            this.owner = _owner;
            this.hp = _hp;
            this.value = _value;
        }
    }

    public class Map {
        public int width;
        public int height;
        public int faceWidth;
        public int faceCount;
        public int landHeight;
        public Cell[] cells;

        public Map(int _faceWidth, int _height, int _faceCount = 4) {
            faceWidth = _faceWidth;
            height = _height;
            faceCount = _faceCount;
            width = faceWidth * _faceCount;
            landHeight = _height / 2;
            cells = new Cell[width * height];
        }

        public void SetCell(int _x, int _y, int _branchId, int _branchCellId, int _hp, int _value) {
            if (_x < 0 || _x >= width || _y < 0 || _y >= height) {
                Debug.LogError("Cell out of range: " + _x + "  " + _y);
                return;
            }
            Cell cell = new Cell(_branchId, _branchCellId, _hp, _value);
            cells[GetCellIndex(_x, _y)] = cell;
        }

        public int GetFaceIndex(int _x) {
            return _x / faceWidth;
        }

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