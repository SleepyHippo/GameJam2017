// /********************************************************************
//   filename:  Tree.cs
//   author:    Duke
//   date:      2017/06/17
// 
//   purpose:   
// *********************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace DWM {
    public class Group {
        //branchId * 10000 + groupId;
        public int branchGroupId;

        public int groupId;
        public int value;
        public int hp;
        public List<Cell> cells = new List<Cell>();

        public void AddHp(int _hp) {
            hp = Mathf.Clamp(hp + _hp, 0, 100);
            for (int i = 0; i < cells.Count; ++i) {
                cells[i].hp = Mathf.Clamp(cells[i].hp + _hp, 0, 100);
            }
        }
    }

    public class Branch {
        public int branchId;
        public List<Group> groups = new List<Group>();
    }


    public class Tree {
        public List<Branch> branchs = new List<Branch>();
        public Dictionary<int, Group> branchGroupMap = new Dictionary<int, Group>();
    }
}