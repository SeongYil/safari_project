using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Resources;

namespace Assets.Resources.Scripts
{

    //피스 생성 매니저
    public class PieceManager
    {
        public static PieceManager instance = null;

        PieceManager()
        {

        }
        public static void CreatePieceManager()
        {
            instance = new PieceManager();
        }

        public GameObject CreatePiece(int pieceID)
        {
            GameObject pieceObj = new GameObject("Piece");
            GameObject backColorObj = new GameObject("backColor");
            GameObject pieceImageObj = new GameObject("pieceImage");

            pieceObj.layer = LayerMask.NameToLayer("Piece");
            //pieceObj.transform.parent = parent;
            pieceObj.transform.localPosition = Vector3.zero;

            backColorObj.transform.parent = pieceObj.transform;
            backColorObj.transform.localPosition = Vector3.zero;

            pieceImageObj.transform.parent = pieceObj.transform;
            pieceImageObj.transform.localPosition = Vector3.zero;

            Piece pieceScript = pieceObj.AddComponent<Piece>();
            pieceScript.transform.localPosition = Vector3.zero;


            //actionPieceObj.transform.parent = pieceObj.transform;

            SpriteRenderer spriteBackground = backColorObj.AddComponent<SpriteRenderer>();

            spriteBackground.sprite = UnityEngine.Resources.Load<Sprite>("images/9-SlicedWithBorder");
            backColorObj.transform.localScale = new Vector3(0.65f, 0.65f, 1f);
            spriteBackground.sortingLayerName = "Piece";
            spriteBackground.sortingOrder = 0;


            SpriteRenderer spritePiece = pieceImageObj.AddComponent<SpriteRenderer>();
            spritePiece.sortingLayerName = "Piece";
            spritePiece.sortingOrder = 1;

            switch (pieceID)
            {
                case Environment.L1:
                case Environment.L2:
                    {
                        pieceImageObj.name = "lion";
                        spritePiece.sprite = UnityEngine.Resources.Load<Sprite>("images/n-piece0");
                        break;
                    }
                case Environment.E1:
                case Environment.E2:
                    {
                        pieceImageObj.name = "elephant";
                        spritePiece.sprite = UnityEngine.Resources.Load<Sprite>("images/n-piece1");
                        break;

                    }
                case Environment.G1:
                case Environment.G2:
                    {
                        pieceImageObj.name = "giraffe";
                        spritePiece.sprite = UnityEngine.Resources.Load<Sprite>("images/n-piece2");
                        break;
                    }
                case Environment.P1:
                case Environment.P2:
                    {
                        pieceImageObj.name = "chick";
                        spritePiece.sprite = UnityEngine.Resources.Load<Sprite>("images/n-piece3");
                        break;

                    }
                case Environment.C1:
                case Environment.C2:
                    {
                        pieceImageObj.name = "chicken";
                        spritePiece.sprite = UnityEngine.Resources.Load<Sprite>("images/n-piece4");
                        break;
                    }

            }

            pieceScript.Initialize(pieceID);

            return pieceObj;
        }
    }
}
