using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Resources.Scripts
{
    public class SortingLayerInMeshRenderer : MonoBehaviour
    {

        public string sortingLayerName;
        public int sortingOrder;

        void Start()
        {
            MeshRenderer mesh = GetComponent<MeshRenderer>();
            mesh.sortingLayerName = sortingLayerName;
            mesh.sortingOrder = sortingOrder;
        }
    }
}
