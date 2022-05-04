using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Resources.Scripts
{
    public class BoardStock : MonoBehaviour
    {
        public SpriteRenderer NumberRenderer = null;
        public Piece PieceScript = null;
        private AnimalRuleManager ruleManager = null;

        public int PieceID = 0;
        public int Count = 0;
        private SharedDataType.EColor eColor = SharedDataType.EColor.Count;

        public (int color, int piece) StockID = (0, 0);

        static public (int, int) GetStockID(int pieceID)
        {
            int color = 0;
            int piece = 0;

            if (pieceID > 5)
            {
                color = 1;
            }
            else
            {
                color = 0;
            }

            switch (pieceID)
            {
                case Environment.P1:
                case Environment.P2:
                case Environment.C1:
                case Environment.C2:
                    {
                        piece = 2;
                        break;
                    }
                case Environment.E1:
                case Environment.E2:
                    {
                        piece = 0;
                        break;
                    }
                case Environment.G1:
                case Environment.G2:
                    {
                        piece = 1;
                        break;
                    }
            }

            (int, int) stockID = (color, piece);

            return stockID;
        }

        public void Initialize(int pieceID, int count ,AnimalRuleManager ruleManager, SpriteRenderer numberRenderer, GameObject pieceObj)
        {
            this.ruleManager = ruleManager;
            this.NumberRenderer = numberRenderer;
            PieceID = pieceID;
            this.Count = count;
            this.PieceScript = pieceObj.GetComponent<Piece>();

            StockID = GetStockID(pieceID);

            eColor = (SharedDataType.EColor)StockID.color;


        }

        public Piece GetPiece()
        {
            return PieceScript;
        }

        public void SetCount(int count)
        {
            NumberRenderer.sprite = GameManager.instance.GetSpriteNumber(count);
            Count = count;
        }

        public void SetImage(int count)
        {
            NumberRenderer.sprite = GameManager.instance.GetSpriteNumber(count);
        }

        private void OnMouseOver()
        {
            if (ruleManager.CheckTurn(eColor) == false)
            {
                return;
            }


            if (Input.GetMouseButtonDown(0))
            {
                int reducedCount = ruleManager.env.BoardStocks[StockID.color, StockID.piece].Count - 1;

                if(reducedCount < 0)
                {
                    return;
                }

                //1 감소
                SetImage(reducedCount);

                ruleManager.SelectStock(this);
            }

        }

        private void OnMouseUp()
        {
            ruleManager.InactiveVisualizeMovePath();
            SetImage(ruleManager.env.BoardStocks[StockID.color, StockID.piece].Count);

            ruleManager.DestorySelectedPiece();



            //Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Vector2 screenPos = new Vector2(worldPos.x, worldPos.y);
            //Ray2D ray = new Ray2D(screenPos, Vector2.zero);
            //RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            //if (hit.collider == null || hit.transform.gameObject.layer != 9   )
            //{
            //    //원복
            //    SetCount(ruleManager.env.BoardStocks[StockID.color, StockID.piece].Count);
            //    ruleManager.DestorySelectedPiece();
            //}

        }


    }
}
