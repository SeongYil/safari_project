using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Resources.Scripts
{

    //보드판을 그리는 클래스
    public class Board
    {
        private float startX = -3f;
        private float startY = 4;

        private int X = 3;
        private int Y = 4;
        static public float size = 2.0f;

        private float line_width = 0.05f;

        List<Vector3> piecePositions = new List<Vector3>();


        private void SetupLineOption(LineRenderer line)
        {
            line.startWidth = line_width;
            line.endWidth = line_width;

            line.startColor = Color.black;
            line.endColor = Color.black;

            line.positionCount = 2;
        }

        public Vector3 GetPieceRenderPositions(int x, int y)
        {
            return piecePositions[y * 3 + x];
        }

        // Start is called before the first frame update
        public void Initialize(Transform parentTransform)
        {


            GameObject boardObject = new GameObject("board_Lines");

            boardObject.transform.parent = parentTransform;

            Vector3 parentPos = boardObject.transform.parent.position;

            //vertical line render
            for (int i = 0; i < X + 1; ++i)
            {
                GameObject vertical = new GameObject("vertical" + i);
                vertical.transform.parent = boardObject.transform;
                LineRenderer line = vertical.AddComponent<LineRenderer>();
                SetupLineOption(line);


                line.SetPosition(0, new Vector3(parentPos.x + startX + (size * i), parentPos.y + startY, 0));
                line.SetPosition(1, new Vector3(parentPos.x + startX + (size * i), parentPos.y + startY - (size * Y), 0));


                line.material = GameManager.instance.DefaultLineMaterial;

            }

            //horizontal line render
            for (int i = 0; i < Y + 1; ++i)
            {
                GameObject horizontal = new GameObject("horizontal" + i);
                horizontal.transform.parent = boardObject.transform;
                LineRenderer line = horizontal.AddComponent<LineRenderer>();
                SetupLineOption(line);


                line.SetPosition(0, new Vector3(parentPos.x + startX, parentPos.y + startY - (size * i), 0));
                line.SetPosition(1, new Vector3(parentPos.x + startX + (size * X), parentPos.y + startY - (size * i), 0));

                line.material = GameManager.instance.DefaultLineMaterial;

            }

            for (int y = 0; y < Y; ++y)
            {
                for (int x = 0; x < X; ++x)
                {
                    piecePositions.Add(new Vector3((startX + 1) + (x * size), (startY - 1) - (y * size), 0));
                }
            }

        }
    }
}

