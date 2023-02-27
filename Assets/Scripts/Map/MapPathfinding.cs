using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPathfinding
{
    private Map m_Map;

    public MapPathfinding(Map map)
    {
        m_Map = map;
    }

    public struct Node
    {
        public Node(int cost, Vector2Int pos, Vector2Int from, int jumpHeight, int dropHeight, int airMove)
        {
            currCost = cost;
            position = pos;
            pathfrom = from;

            currJumpHeight = jumpHeight;
            currDropHeight = dropHeight;

            currAirMove = airMove;
        }

        public int currCost;

        public Vector2Int position;
        public Vector2Int pathfrom;
        
        public int currJumpHeight;
        public int currDropHeight;

        public int currAirMove;
    }

    public Dictionary<Vector2Int, Node> GetAllMovePossibility(Vector2Int leftBotObject, Vector2Int sizeObject, int jumpHeight, int airMoveSpeed)
    {
        EnumBlocks[,] grid = m_Map.GetGrid().GetGrid();
        Dictionary<EnumBlocks, EnumBlocks> typeBlockCanGo = m_Map.GetGrid().GetBackGroundDict();

        Dictionary<Vector2Int, Node> nodes = new Dictionary<Vector2Int, Node>();

        List<Node> nodeToGo = new List<Node>();
        nodeToGo.Add(new Node(0, leftBotObject, Vector2Int.zero, 0, 0, 0));

        while (true)
        {
            //si il na plus de node a visiter arrete
            if (nodeToGo.Count == 0)
            {
                break;
            }

            //prend le prochain movement
            Node currNode = nodeToGo[0];
            nodeToGo.RemoveAt(0);

            //si le movement existe deja regarde sont cost et le change si il a a etre change
            if (nodes.ContainsKey(currNode.position))
            {
                if (currNode.currCost < nodes[currNode.position].currCost)
                {
                    nodes[currNode.position] = currNode;
                }
                else
                {
                    continue;
                }
            }

            //si la node n'a pas ete visiter l'ajoute
            if (!nodes.ContainsKey(currNode.position))
            {
                nodes.Add(currNode.position, currNode);
            }
            

            //regarde si il a un chemin en bas
            bool goBot = true;
            for (int x = currNode.position.x; x < currNode.position.x + sizeObject.x; x++)
            {
                if (!typeBlockCanGo.ContainsKey(grid[x, currNode.position.y - 1]))
                {
                    goBot = false;
                }
            }

            //regarde si il a un chemin en haut
            bool goTop = true;
            for (int x = currNode.position.x; x < currNode.position.x + sizeObject.x; x++)
            {
                if (!typeBlockCanGo.ContainsKey(grid[x, currNode.position.y + sizeObject.y]))
                {
                    goTop = false;
                }
            }

            //regarde si il a un chemin a droit
            bool goRight = true;
            for (int y = currNode.position.y; y < currNode.position.y + sizeObject.y; y++)
            {
                if (!typeBlockCanGo.ContainsKey(grid[currNode.position.x + sizeObject.x, y]))
                {
                    goRight = false;
                }
            }

            //regarde si il a un chemin a gauche
            bool goLeft = true;
            for (int y = currNode.position.y; y < currNode.position.y + sizeObject.y; y++)
            {
                if (!typeBlockCanGo.ContainsKey(grid[currNode.position.x - 1, y]))
                {
                    goLeft = false;
                }
            }

            //si il est sur le sol reset les valeur de la node actuel
            if(!goBot)
            {
                currNode.currJumpHeight = 0;
                currNode.currDropHeight = 0;
                currNode.currAirMove = 0;
                nodes[currNode.position] = currNode;
            }

            //si il peut aller au bot
            if(goBot)
            {
                if (currNode.currDropHeight == 0 && currNode.currJumpHeight == 0)
                {
                    goTop = false;
                    currNode.currAirMove++;
                    nodes[currNode.position] = currNode;
                }

                //ajoute un node en bas
                nodeToGo.Add(new Node(currNode.currCost + 2, new Vector2Int(currNode.position.x, currNode.position.y - 1), currNode.position, 0, currNode.currDropHeight + 1, 0));
            }

            //si il peut aller au top
            if(goTop)
            {
                //si il peut monter
                if(currNode.currJumpHeight < jumpHeight && currNode.currDropHeight == 0)
                {
                    //ajoute un node
                    nodeToGo.Add(new Node(currNode.currCost + 2, new Vector2Int(currNode.position.x, currNode.position.y + 1), currNode.position, currNode.currJumpHeight + 1, 0, 0));
                }
            }

            //si il peut aller a droit
            if(goRight)
            {
                //si il est dans les air
                if(currNode.currJumpHeight != 0 || currNode.currDropHeight != 0 || currNode.currAirMove != 0)
                {
                    if(currNode.currAirMove <= airMoveSpeed)
                    {
                        nodeToGo.Add(new Node(currNode.currCost + 2, new Vector2Int(currNode.position.x + 1, currNode.position.y), currNode.position, currNode.currJumpHeight, currNode.currDropHeight, currNode.currAirMove + 1));
                    }
                }
                //si il est sur le sol
                else
                {
                    nodeToGo.Add(new Node(currNode.currCost + 1, new Vector2Int(currNode.position.x + 1, currNode.position.y), currNode.position, 0, 0, 0));
                }
            }

            //si il peut peut aller a gauche
            if(goLeft)
            {
                //si il est dans les air
                if (currNode.currJumpHeight != 0 || currNode.currDropHeight != 0 || currNode.currAirMove != 0)
                {
                    if (currNode.currAirMove <= airMoveSpeed)
                    {
                        nodeToGo.Add(new Node(currNode.currCost + 2, new Vector2Int(currNode.position.x - 1, currNode.position.y), currNode.position, currNode.currJumpHeight, currNode.currDropHeight, currNode.currAirMove + 1));
                    }
                }
                //si il est sur le sol
                else
                {
                    nodeToGo.Add(new Node(currNode.currCost + 1, new Vector2Int(currNode.position.x - 1, currNode.position.y), currNode.position, 0, 0, 0));
                }
            }
        }

        return nodes;
    }

}
