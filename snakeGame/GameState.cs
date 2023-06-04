using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace snakeGame
{
    public class GameState
    {
        public int Rows { get; }
        public int Cols { get; }
        public gridValues[,] Grid { get; }
        public Direction Dir { get; private set; }
        public int Score { get; private set; }
        public bool GameOver { get; private set; }

        private readonly LinkedList<Direction> dirChanges = new LinkedList<Direction>();
        private readonly LinkedList<Position> snakePositions = new LinkedList<Position>();
        private readonly Random random = new Random();

        public GameState(int rows,int cols)
        {
            Rows = rows;
            Cols = cols;
            Grid = new gridValues[Rows, Cols];
            Dir = Direction.Right;

            addSnake();
            AddFood();
        }

        private void addSnake()
        {
            int r = Rows / 2;

            for(int c = 1; c <= 3; c++)
            {
                Grid[r, c] = gridValues.Snake;
                snakePositions.AddFirst(new Position(r, c));

            }
        }

        private IEnumerable<Position> EmptyPositions()
        {
            for(int r=0; r< Rows; r++)
            {
                for(int c = 0; c< Cols; c++)
                {
                    if (Grid[r,c] == gridValues.Empty)
                    {
                        yield return new Position(r, c);
                    }
                }
            }
        }

        private void AddFood()
        {
            List<Position> empty = new List<Position>(EmptyPositions());
            
            if(empty.Count == 0)
            {
                return;
            }

            Position pos = empty[random.Next(empty.Count)];
            Grid[pos.Row, pos.Col] = gridValues.Food;
        }

        public Position HeadPosition()
        {
            return snakePositions.First.Value;
        }

        public Position TailPosition()
        {
            return snakePositions.Last.Value;
        }

        public IEnumerable<Position> SnakePositions()
        {
            return snakePositions;
        }

        private void AddHead(Position pos)
        {
            snakePositions.AddFirst(pos);
            Grid[pos.Row, pos.Col] = gridValues.Snake;
        }

        private void RemoveTail()
        {
            Position tail = snakePositions.Last.Value;
            Grid[tail.Row, tail.Col] = gridValues.Empty;
            snakePositions.RemoveLast();
        }

        private Direction GetLastDirection()
        {
            if(dirChanges.Count == 0)
            {
                return Dir;
            }
            return dirChanges.Last.Value;
        }

        private bool CanChangeDirection(Direction newDir)
        {
            //if there are already two directions stored inside the buffer then consider the buffer is full
            if(dirChanges.Count == 2)
            {
                return false;
            }
            Direction lastDir = GetLastDirection();
            return newDir != lastDir && newDir != lastDir.Opposite();
        }

        public void ChangeDirection(Direction dir)
        {
            if(CanChangeDirection(dir))
            {
                dirChanges.AddLast(dir);

            }
        }

        private bool OutsideGrid(Position pos)
        {
            return pos.Row < 0 || pos.Row >= Rows || pos.Col < 0 || pos.Col >= Cols;

        }

        private gridValues WillHit(Position newHeadPos)
        {
            if(OutsideGrid(newHeadPos))
            {
                return gridValues.Outside;
            }
            if(newHeadPos == TailPosition())
            {
                return gridValues.Empty;
            }

            return Grid[newHeadPos.Row, newHeadPos.Col];

        }

        public void move()
        {
            if(dirChanges.Count > 0)
            {
                Dir = dirChanges.First.Value;
                dirChanges.RemoveLast();
            }

            Position newHeadPos = HeadPosition().Translate(Dir);
            gridValues hit = WillHit(newHeadPos);

            if(hit == gridValues.Outside || hit == gridValues.Snake)
            {
                GameOver = true;
            }
            else if(hit == gridValues.Empty)
            {
                RemoveTail();
                AddHead(newHeadPos);
            }
            else if(hit == gridValues.Food)
            {
                AddHead(newHeadPos);
                Score++;
                AddFood();
            }
        }
    }
}
