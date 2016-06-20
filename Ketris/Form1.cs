using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace Ketris
{
    public partial class Form1 : Form
    {
        Graphics g;
        Block[] Blocks;
        Block[] PrevBlocks; // Preview blocks
        Piece CurrentPiece, NextPiece;
        int NextPieceIndex;
        Random RandomPiece;
        Checker MyChecker;
        public const int StartingPoint = 0; // I'll keep this in case in the future I want to draw the board on a different control
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RandomPiece = new Random();
            NextPieceIndex = RandomPiece.Next(1, 8);
            g = panel1.CreateGraphics();
            Graphics g2 = panel2.CreateGraphics();
            Blocks = new Block[200];
            PrevBlocks = new Block[40];
            MyChecker = new Checker(Blocks, g);

            for (int i = 0; i < 200; i++)
                Blocks[i] = new Block(g, i, StartingPoint, 20);
            for (int i = 0; i < 40; i++)
                PrevBlocks[i] = new Block(g2, i, StartingPoint + 3, 15);
        }                                                          


        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            foreach (Block b in Blocks)
                b.Draw();
            foreach (Block b in PrevBlocks)
                b.Draw();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (CurrentPiece == null)
            {
                timer1.Enabled = false;
                MessageBox.Show("Game Over!");
                return;
            }
            if (CurrentPiece.Move(Direction.Down) == false)
            {
                timer1.Stop();
                MyChecker.CheckBoard();
                label1.Text = "Lines: " + MyChecker.Count.ToString();
                timer1.Interval = 800 - (MyChecker.Count / 10 * 50);
                label2.Text = "Level: " + ((800 - timer1.Interval) / 100 + 1);
                GetNewPiece();
                timer1.Start();
            }
        }

        private void GetNewPiece()
        {
            if (Blocks[5].Drawn)
            {
                CurrentPiece = null;
                return;
            }

            int p = NextPieceIndex;
            NextPieceIndex = RandomPiece.Next(1, 8);
            if (NextPiece != null)
                NextPiece.Visible = false;
            
            switch (NextPieceIndex)
            {
                case 1:
                    NextPiece = new PieceD(PrevBlocks, 20);
                    break;
                case 2:
                    NextPiece = new PieceI(PrevBlocks, 21);
                    break;
                case 3:
                    NextPiece = new PieceJ(PrevBlocks, 31);
                    break;
                case 4:
                    NextPiece = new PieceL(PrevBlocks, 30);
                    break;
                case 5:
                    NextPiece = new PieceS(PrevBlocks, 21);
                    break;
                case 6:
                    NextPiece = new PieceW(PrevBlocks, 21);
                    break;
                case 7:
                    NextPiece = new PieceZ(PrevBlocks, 21);
                    break;
            }

            switch (p)
            {
                case 1:
                    CurrentPiece = new PieceD(Blocks, 15);
                    break;
                case 2:
                    CurrentPiece = new PieceI(Blocks, 25);
                    break;
                case 3:
                    CurrentPiece = new PieceJ(Blocks, 25);
                    break;
                case 4:
                    CurrentPiece = new PieceL(Blocks, 25);
                    break;
                case 5:
                    CurrentPiece = new PieceS(Blocks, 15);
                    break;
                case 6:
                    CurrentPiece = new PieceW(Blocks, 15);
                    break;
                case 7:
                    CurrentPiece = new PieceZ(Blocks, 15);
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (Block b in Blocks)
                b.BlockColor = Color.Gray;
            GetNewPiece();
            timer1.Enabled = true;
            label1.Text = "Lines: 0";
            MyChecker.Reset();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Text = ((timer1.Enabled = !timer1.Enabled)? "&Pause": "&Resume");
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!timer1.Enabled)
                return;
            switch (Char.ToLower(Convert.ToChar(e.KeyCode)))
            {
                case 'a':
                    timer1.Stop();
                    MyChecker.AddRow(CurrentPiece.b1, CurrentPiece.b2, CurrentPiece.b3, CurrentPiece.b4);
                    timer1.Start();
                    break;
                case 'j':
                    CurrentPiece.Move(Direction.Left);
                    break;
                case 'k':
                    CurrentPiece.Move(Direction.Down);
                    break;
                case 'l':
                    CurrentPiece.Move(Direction.Right);
                    break;
                case 'p':

                    break;
                case 'i':
                    CurrentPiece.Roll();
                    break;
                case ' ':
                    e.SuppressKeyPress = true;
                    while (CurrentPiece.Move(Direction.Down)) ;
                    break;
            }
        }

    }

    public enum Direction { Left, Right, Down };
   
    public class Block
    {
        int Size;
        public readonly static Color Background = Color.Gray;
        Graphics g;
        Color Foreground; 
        Pen Outline;
        int Loc, StartingPoint;
        Rectangle drawArea;
 
        public Block(Graphics MyG, int Loc, int StartingPoint, int Size) 
        { 
            g = MyG; 
            this.Loc = Loc;
            this.StartingPoint = StartingPoint;
            this.Size = Size;
            Outline = new Pen(Background);
            Foreground = Background;
            drawArea = new Rectangle(StartingPoint + (Loc % 10 * Size) + 1, StartingPoint + (Loc / 10 * Size) + 1, Size - 2, Size - 2);
        }

        public void Draw()
        {
            drawArea.X = StartingPoint + (Loc % 10 * Size) + 1;
            drawArea.Y = StartingPoint + (Loc / 10 * Size) + 1;
            LinearGradientBrush linearBrush =
                new LinearGradientBrush(drawArea, (!Drawn ? Background : Color.Silver), BlockColor, LinearGradientMode.ForwardDiagonal);
            g.FillRectangle(linearBrush, drawArea);
            g.DrawRectangle(Outline, StartingPoint + (Loc % 10 * Size), StartingPoint + (Loc / 10 * Size), Size - 1, Size - 1);
        }

        public void Hide()
        {
            if (Foreground == Background) // drawing is costly, so make sure we need to
                return;
            Foreground = Background;
            Outline.Color = Background;
            Draw();
        }

        public bool Drawn
        {
            get 
            { 
                return (Foreground != Background); 
            }
        }

        public Color BlockColor
        {
            set
            {
                if (Foreground != value) // drawing is costly, so make sure we need to
                {
                    this.Foreground = value;
                    Outline.Color = (Foreground == Background ? Background : Color.Black);
                    Draw();
                }
            }
            get
            {
                return Foreground;
            }
        }
    }


    abstract public class Piece
    {
        public int b1, b2, b3, b4;
        protected int CornerStone, State;
        protected Block[] Blocks;
        protected Color Foreground;
        public Piece(Block[] Blocks, int b1, int b2, int b3, int b4, int CornerStone, Color Foreground)
        {
            this.b1 = b1;
            this.b2 = b2;
            this.b3 = b3;
            this.b4 = b4;
            this.CornerStone = CornerStone;
            this.Blocks = Blocks;
            this.Foreground = Foreground;
            State = 1;

            if (IsValid() == false)
                throw new Exception("Calling Piece constructor with invalid Block locations.");
            
            Visible = true;
        }

        public bool Visible
        {
            set
            {
                if (value)
                {
                    Blocks[b1].BlockColor = Foreground;
                    Blocks[b2].BlockColor = Foreground;
                    Blocks[b3].BlockColor = Foreground;
                    Blocks[b4].BlockColor = Foreground;
                }
                else
                {
                    Blocks[b1].BlockColor = Block.Background;
                    Blocks[b2].BlockColor = Block.Background;
                    Blocks[b3].BlockColor = Block.Background;
                    Blocks[b4].BlockColor = Block.Background;
                }
            }
            get
            {
                return (Blocks[b1].Drawn); // if one of them is drawn, the rest are; and vice versa
            }
        }

        public bool Move(Direction d)
        {
            Visible = false;
            if (IsValidMove(d) == false)
            {
                Visible = true;
                return false;
            }
            switch (d)
            {
                case Direction.Down:
                    b1 += 10;
                    b2 += 10;
                    b3 += 10;
                    b4 += 10;
                    CornerStone += 10;
                    break;
                case Direction.Left:
                    b1--;
                    b2--;
                    b3--;
                    b4--;
                    CornerStone--;
                    break;
                case Direction.Right:
                    b1++;
                    b2++;
                    b3++;
                    b4++;
                    CornerStone++;
                    break;
            }
            return (Visible = true);
        }

        // this method checks if a proposed block transfer warps it to the other end of the board or beyond board limits..
        protected bool Warping(int a, int b)
        {
            int HorizontalDistance = Math.Abs((CornerStone % 10) - (b % 10));
            return ((b < 0 || b > 199) || (HorizontalDistance > 2)); // if beyond board limits or blocks too far apart, then it's warping...
        }
        
        private bool IsValidMove(Direction d)
        {
            switch (d)
            {
                case Direction.Down:
                    if ((b1 + 10 > 199) || (b2 + 10 > 199) || (b3 + 10 > 199) || (b4 + 10 > 199)) // if we reached the bottom...
                        return false;
                    if (Blocks[b1 + 10].Drawn || Blocks[b2 + 10].Drawn || Blocks[b3 + 10].Drawn || Blocks[b4 + 10].Drawn)
                        return false;
                    break;
                case Direction.Left:
                    if ((b1 % 10 == 0) || (b2 % 10 == 0) || (b3 % 10 == 0) || (b4 % 10 == 0)) // if we reached the edge..
                        return false;
                    if (Blocks[b1 - 1].Drawn || Blocks[b2 - 1].Drawn || Blocks[b3 - 1].Drawn || Blocks[b4 - 1].Drawn)
                        return false;
                    break;
                case Direction.Right:
                    if ((b1 % 10 == 9) || (b2 % 10 == 9) || (b3 % 10 == 9) || (b4 % 10 == 9)) // if we reached the edge..
                        return false;
                    if (Blocks[b1 + 1].Drawn || Blocks[b2 + 1].Drawn || Blocks[b3 + 1].Drawn || Blocks[b4 + 1].Drawn)
                        return false;
                    break;
            }
            return true;
        }

        // Are all pieces adjacent to each other?
        private bool IsValid()
        {
            int a = b1 % 10, b = b2 % 10, c = b3 % 10, d = b4 % 10; // Smush all blocks into one line
            if ((b1 == b2) || (b1 == b3) || (b1 == b4) || (b2 == b3) || (b2 == b4) || (b3 == b4))
                return false;
            if ((Math.Abs(a - b) > 1) || ((Math.Abs(b - c) > 1)) || ((Math.Abs(c - d) > 1))) // make sure they're adjacent
                return false;
            if (b1 < 0 || b1 > 199 || b2 < 0 || b2 > 199 || b3 < 0 || b3 > 199 || b4 < 0 || b4 > 199)
                return false;
            return true;
        }

        abstract public void Roll();
    }
    
    public class PieceZ : Piece
    {
        public PieceZ(Block[] Blocks, int cs)
            : base(Blocks, cs - 11, cs - 10, cs, cs + 1, cs, Color.Blue) { }

        public override void Roll()
        {
            Visible = false;
            switch (State)
            {
                case 1:
                    if (Warping(b1, CornerStone - 9) || Warping(b2, CornerStone + 1) || Warping(b4, CornerStone + 10))
                        break;
                    if (Blocks[CornerStone - 9].Drawn || Blocks[CornerStone + 10].Drawn)
                        break;
                    b1 = CornerStone - 9;
                    b2 = CornerStone + 1;
                    b4 = CornerStone + 10;
                    State = 2;
                    break;
                case 2:
                    if (Warping(b1, CornerStone - 11) || Warping(b2, CornerStone - 10) || Warping(b4, CornerStone + 1))
                        break;
                    if (Blocks[CornerStone - 10].Drawn || Blocks[CornerStone + 11].Drawn)
                        break;
                    b1 = CornerStone - 11;
                    b2 = CornerStone - 10;
                    b4 = CornerStone + 1;
                    State = 1;
                    break;
            }
            Visible = true;
        }
    }

    public class PieceS : Piece
    {
        public PieceS(Block[] Blocks, int cs)
            : base(Blocks, cs - 1, cs, cs - 10, cs - 9, cs, Color.Red) { }

        public override void Roll()
        {
            Visible = false;
            switch (State)
            {
                case 1:
                    if (Warping(b1, CornerStone + 10) || Warping(b3, CornerStone - 1) || Warping(b4, CornerStone - 11))
                        break;
                    if (Blocks[CornerStone - 11].Drawn || Blocks[CornerStone + 10].Drawn)
                        break;
                    b1 = CornerStone + 10;
                    b3 = CornerStone - 1;
                    b4 = CornerStone - 11;
                    State = 2;
                    break;
                case 2:
                    if (Warping(b1, CornerStone - 1) || Warping(b3, CornerStone - 10) || Warping(b4, CornerStone - 9))
                        break;
                    if (Blocks[CornerStone - 9].Drawn || Blocks[CornerStone - 10].Drawn)
                        break;
                    b1 = CornerStone - 1;
                    b3 = CornerStone - 10;
                    b4 = CornerStone - 9;
                    State = 1;                    
                    break;
            }
            Visible = true;
        }
    }

    public class PieceJ : Piece
    {
        public PieceJ(Block[] Blocks, int cs)
            : base(Blocks, cs - 20, cs - 10, cs, cs - 1, cs, Color.Green) { }

        public override void Roll()
        {
            Visible = false;
            switch (State)
            {
                case 1:
                    if (Warping(b1, CornerStone + 1) || Warping(b2, CornerStone + 2) || Warping(b4, CornerStone - 10))
                        break;
                    if (Blocks[CornerStone + 1].Drawn || Blocks[CornerStone + 2].Drawn)
                        break;
                    b1 = CornerStone + 1;
                    b2 = CornerStone + 2;
                    b4 = CornerStone - 10;
                    State = 2;
                    break;
                case 2:
                    if (Warping(b1, CornerStone + 20) || Warping(b2, CornerStone + 10) || Warping(b4, CornerStone + 1))
                        break;
                    if (Blocks[CornerStone - 9].Drawn || Blocks[CornerStone + 10].Drawn)
                        break;
                    b1 = CornerStone + 20;
                    b2 = CornerStone + 10;
                    b4 = CornerStone + 1;
                    State = 3;
                    break;
                case 3:
                    if (Warping(b1, CornerStone - 2) || Warping(b2, CornerStone - 1) || Warping(b4, CornerStone + 10))
                        break;
                    if (Blocks[CornerStone - 1].Drawn || Blocks[CornerStone - 2].Drawn)
                        break;
                    b1 = CornerStone - 2;
                    b2 = CornerStone - 1;
                    b4 = CornerStone + 10;
                    State = 4;
                    break;
                case 4:
                    if (Warping(b1, CornerStone - 20) || Warping(b2, CornerStone - 10) || Warping(b4, CornerStone - 1))
                        break;
                    if (Blocks[CornerStone - 10].Drawn || Blocks[CornerStone - 20].Drawn)
                        break;
                    b1 = CornerStone - 20;
                    b2 = CornerStone - 10;
                    b4 = CornerStone - 1;
                    State = 1;
                    break;
            }
            Visible = true;
        }
    }

    public class PieceL : Piece
    {
        public PieceL(Block[] Blocks, int cs)
            : base(Blocks, cs - 20, cs - 10, cs, cs + 1, cs, Color.Purple) { }

        public override void Roll()
        {
            Visible = false;
            switch (State)
            {
                case 1:
                    if (Warping(b1, CornerStone + 2) || Warping(b2, CornerStone + 1) || Warping(b4, CornerStone + 10))
                        break;
                    if (Blocks[CornerStone + 2].Drawn || Blocks[CornerStone + 10].Drawn)
                        break;
                    b1 = CornerStone + 2;
                    b2 = CornerStone + 1;
                    b4 = CornerStone + 10;
                    State = 2;
                    break;
                case 2:
                    if (Warping(b1, CornerStone + 20) || Warping(b2, CornerStone + 10) || Warping(b4, CornerStone - 1))
                        break;
                    if (Blocks[CornerStone - 1].Drawn || Blocks[CornerStone + 20].Drawn)
                        break;
                    b1 = CornerStone + 20;
                    b2 = CornerStone + 10;
                    b4 = CornerStone - 1;
                    State = 3;
                    break;
                case 3:
                    if (Warping(b1, CornerStone - 2) || Warping(b2, CornerStone - 1) || Warping(b4, CornerStone - 10))
                        break;
                    if (Blocks[CornerStone - 2].Drawn || Blocks[CornerStone - 10].Drawn)
                        break;
                    b1 = CornerStone - 2;
                    b2 = CornerStone - 1;
                    b4 = CornerStone - 10;
                    State = 4;
                    break;
                case 4:
                    if (Warping(b1, CornerStone - 20) || Warping(b2, CornerStone - 10) || Warping(b4, CornerStone + 1))
                        break;
                    if (Blocks[CornerStone - 20].Drawn || Blocks[CornerStone + 1].Drawn)
                        break;
                    b1 = CornerStone - 20;
                    b2 = CornerStone - 10;
                    b4 = CornerStone + 1;
                    State = 1;
                    break;
            }
            Visible = true;
        }
    }

    public class PieceI : Piece
    {
        public PieceI(Block[] Blocks, int cs)
            : base(Blocks, cs - 20, cs - 10, cs, cs + 10, cs, Color.Navy) { }

        public override void Roll()
        {
            Visible = false;
            switch (State)
            {
                case 1:
                    if (Warping(b1, CornerStone + 2) || Warping(b2, CornerStone + 1) || Warping(b4, CornerStone - 1))
                        break;
                    if (Blocks[CornerStone - 1].Drawn || Blocks[CornerStone + 1].Drawn || Blocks[CornerStone + 2].Drawn)
                        break;
                    b1 = CornerStone + 2;
                    b2 = CornerStone + 1;
                    b4 = CornerStone - 1;
                    State = 2;                
                    break;
                case 2:
                    if (Warping(b1, CornerStone - 20) || Warping(b2, CornerStone - 10) || Warping(b4, CornerStone + 10))
                        break;
                    if (Blocks[CornerStone - 20].Drawn || Blocks[CornerStone - 10].Drawn || Blocks[CornerStone + 10].Drawn)
                        break;
                    b1 = CornerStone - 20;
                    b2 = CornerStone - 10;
                    b4 = CornerStone + 10;
                    State = 1;
                    break;
            }
            Visible = true;
        }
    }

    public class PieceD : Piece
    {
        public PieceD(Block[] Blocks, int cs)
            : base(Blocks, cs - 10, cs - 9, cs, cs + 1, cs, Color.Orange) { }

        public override void Roll() { }  // does not roll 
    }

    public class PieceW : Piece
    {
        public PieceW(Block[] Blocks, int cs)
            : base(Blocks, cs - 1, cs, cs - 10, cs + 1, cs, Color.Brown) { }

        public override void Roll()
        {
            Visible = false;
            switch (State)
            {
                case 1:
                    if (Warping(b1, CornerStone - 10) || Warping(b3, CornerStone + 1) || Warping(b4, CornerStone + 10))
                        break;
                    if (Blocks[CornerStone + 10].Drawn)
                        break;
                    b1 = CornerStone - 10;
                    b3 = CornerStone + 1;
                    b4 = CornerStone + 10;
                    State = 2;
                    break;
                case 2:
                    if (Warping(b1, CornerStone + 1) || Warping(b3, CornerStone + 10) || Warping(b4, CornerStone - 1))
                        break;
                    if (Blocks[CornerStone - 1].Drawn)
                        break;
                    b1 = CornerStone + 1;
                    b3 = CornerStone + 10;
                    b4 = CornerStone - 1;
                    State = 3;
                    break;
                case 3:
                    if (Warping(b1, CornerStone + 10) || Warping(b3, CornerStone - 1) || Warping(b4, CornerStone - 10))
                        break;
                    if (Blocks[CornerStone - 10].Drawn)
                        break;
                    b1 = CornerStone + 10;
                    b3 = CornerStone - 1;
                    b4 = CornerStone - 10;
                    State = 4;
                    break;
                case 4:
                    if (Warping(b1, CornerStone - 1) || Warping(b3, CornerStone - 10) || Warping(b4, CornerStone + 1))
                        break;
                    if (Blocks[CornerStone + 1].Drawn)
                        break;
                    b1 = CornerStone - 1;
                    b3 = CornerStone - 10;
                    b4 = CornerStone + 1;
                    State = 1;
                    break;
            }
            Visible = true;
        }
    }

    public class Checker
    {
        Block[] Blocks;
        Graphics g;
        int numLines;
        Random R;

        public Checker(Block[] Blocks, Graphics g)
        {
            this.Blocks = Blocks;
            this.g = g;
            numLines = 0;
            R = new Random();
        }

        public void CheckBoard()
        {
            for (int Line = 19; Line >= 0; Line--)
                if (Full(Line))
                {
                    DropDown(Line);
                    Line++;
                    numLines++;
                }
        }

        private bool Full(int Line)
        {
            for (int x = 0; x <= 9; x++)
                if (!Blocks[Line * 10 + x].Drawn)
                    return false;
            return true;        
        }

        private void DropDown(int Line)
        {
            for (int y = Line * 10 + 9; y >= 10; y--)
                Blocks[y].BlockColor = Blocks[y - 10].BlockColor;

            for (int i = 0; i <= 9; i++)
                Blocks[i].BlockColor = Block.Background;
        }

        public void AddRow(int b1, int b2, int b3, int b4)
        {
            for (int x = 0; x <= 189; x++)
                if (!((b1 == x) || (b1 == x + 10) || (b2 == x) || (b2 == x + 10) || (b3 == x) || (b3 == x + 10) || (b4 == x) || (b4 == x + 10)))
                    Blocks[x].BlockColor = Blocks[x + 10].BlockColor;

            for (int i = 190; i <= 199; i++)
                Blocks[i].BlockColor = (R.Next(101) < 70 ? Color.Red : Block.Background); // a 70% chance for each block to exist 
        }

        public int Count
        {
            get
            {
                return numLines;
            }
        }

        public void Reset()
        {
            numLines = 0;
        }
    
    }

}